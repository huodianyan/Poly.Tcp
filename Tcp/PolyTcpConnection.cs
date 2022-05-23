using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Poly.Tcp
{
    public class PolyTcpConnection
    {
        internal TcpClient client;
        internal long connectionId;
        private APolyTcpBase driver;

        //private ConcurrentQueue<ArraySegment<byte>> receiveQueue;
        private ConcurrentQueue<ArraySegment<byte>> sendQueue;
        private ManualResetEvent sendPending = new ManualResetEvent(false);

        private readonly Thread sendThread;
        private readonly Thread receiveThread;

        private readonly byte[] header;
        private readonly byte[] sendHeader;
        private readonly byte[] receiveBuffer;

        internal bool IsConnected { get; private set; }

        internal PolyTcpConnection(APolyTcpBase driver, TcpClient client, long connectionId)
        {
            this.driver = driver;
            this.client = client;
            this.connectionId = connectionId;

            sendHeader = new byte[4];
            sendQueue = new ConcurrentQueue<ArraySegment<byte>>();
            sendThread = new Thread(() => SendLoop());
            sendThread.IsBackground = true;
            sendThread.Start();

            header = new byte[4];
            receiveBuffer = new byte[driver.MaxMessageSize];
            //receiveQueue = new ConcurrentQueue<ArraySegment<byte>>();
            receiveThread = new Thread(() => ReceiveLoop());
            receiveThread.IsBackground = true;
            receiveThread.Start();

            IsConnected = true;
        }
        internal void Send(ArraySegment<byte> data)
        {
            var count = data.Count;
            var dest = driver.arrayPool.Rent(count);
            Array.Copy(data.Array, data.Offset, dest, 0, count);
            sendQueue.Enqueue(new ArraySegment<byte>(dest, 0, count));
            // interrupt SendThread WaitOne()
            //Console.WriteLine($"SimpleConnection.Send: {data.Length}, {offset}, {count}");
            sendPending.Set();
        }
        internal void Disconnect()
        {
            if (!IsConnected) return;
            IsConnected = false;
            try { client.GetStream().Close(); } catch { }
            client.Close();
            //client = null;
            driver.OnConnectionDisconnect(this);
            client = null;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ReadExactly(NetworkStream stream, byte[] buffer, int amount)
        {
            int bytesRead = 0;
            while (bytesRead < amount)
            {
                // read up to 'remaining' bytes with the 'safe' read extension
                int result = stream.Read(buffer, bytesRead, amount - bytesRead);
                // .Read returns 0 if disconnected
                if (result == 0) return false;
                bytesRead += result;
            }
            return true;
        }
        private void ReceiveLoop()
        {
            try
            {
                NetworkStream stream = client.GetStream();
                var arrayPool = driver.arrayPool;
                while (true)
                {
                    // read exactly 4 bytes for header (blocking)
                    if (!ReadExactly(stream, header, 4)) break;
                    // convert to int
                    int size = (header[0] << 24) | (header[1] << 16) | (header[2] << 8) | header[3];
                    if (size > driver.MaxMessageSize || size <= 0)
                    {
                        Console.Error.WriteLine($"ReceiveLoop: size error {size}");
                        break;
                    }
                    //var buffer = arrayPool.Rent(size);
                    if (!ReadExactly(stream, receiveBuffer, size)) break;
                    //Console.WriteLine($"ReceiveLoop: {data.Length}, {size}");
                    //receiveQueue.Enqueue(new ArraySegment<byte>(buffer, 0, size));
                    driver.OnConnectionRecieve(this, new ArraySegment<byte>(receiveBuffer, 0, size));
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Connection recieve thread exception: " + exception);
            }
            finally
            {
                sendThread.Interrupt();
                Disconnect();
            }
        }
        private void SendLoop()
        {
            try
            {
                NetworkStream stream = client.GetStream();
                while (client.Connected)
                {
                    sendPending.Reset();
                    while (sendQueue.TryDequeue(out var segment))
                    {
                        var count = segment.Count;
                        sendHeader[0] = (byte)(count >> 24);
                        sendHeader[1] = (byte)(count >> 16);
                        sendHeader[2] = (byte)(count >> 8);
                        sendHeader[3] = (byte)count;
                        stream.Write(sendHeader, 0, 4);
                        stream.Write(segment.Array, segment.Offset, count);
                        driver.arrayPool.Return(segment.Array);
                        //logger.LogTrace($"SendLoop: {segment.Count}");
                    }
                    sendPending.WaitOne();
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Connection send thread exception: " + exception);
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
