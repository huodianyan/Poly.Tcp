using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Poly.Tcp
{
    public class PolyTcpServer : APolyTcpBase
    {
        public TcpListener listener;
        Thread listenerThread;
        private readonly ConcurrentDictionary<long, PolyTcpConnection> connectionDict = new ConcurrentDictionary<long, PolyTcpConnection>();
        public ICollection<PolyTcpConnection> Connections => connectionDict.Values;
        
        //private ILogger logger;
        private long counter = 0;
        private int port;

        //public bool Active => listenerThread != null && listenerThread.IsAlive;
        public bool IsStarted => listener != null && listener.Server.IsBound;// listenerThread != null && listenerThread.IsAlive;

        public PolyTcpServer(IByteArrayPool arrayPool = null) : base(arrayPool)
        {
        }
        private long NextConnectionId()
        {
            long id = Interlocked.Increment(ref counter);
            if (id == long.MaxValue)
                throw new Exception("connection id limit reached: " + id);
            return (long)id;
        }
        
        private void Listen(int port)
        {
            try
            {
                while (listener !=null)
                {
                    TcpClient client = listener.AcceptTcpClient();

                    // set socket options
                    client.NoDelay = NoDelay;
                    client.SendTimeout = SendTimeout;

                    // generate the next connection id (thread safely)
                    long connectionId = NextConnectionId();
                    PolyTcpConnection connection = new PolyTcpConnection(this, client, connectionId);
                    connectionDict[connectionId] = connection;

                    OnConnectionConnect(connection);
                }
            }
            catch (ThreadInterruptedException) { }
            catch (ThreadAbortException) { }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Server Exception: " + exception);
            }
        }

        internal override void OnConnectionDisconnect(PolyTcpConnection connection)
        {
            base.OnConnectionDisconnect(connection);
            connectionDict.TryRemove(connection.connectionId, out var _);
        }

        public bool Start(int port)
        {
            // not if already started
            if (IsStarted)
                return true;

            listener = TcpListener.Create(port);
            listener.Server.NoDelay = NoDelay;
            listener.Server.SendTimeout = SendTimeout;
            listener.Start();
            Console.WriteLine("Server: listening port=" + port);

            listenerThread = new Thread(() => { Listen(port); });
            listenerThread.IsBackground = true;
            listenerThread.Priority = ThreadPriority.BelowNormal;
            listenerThread.Start();

            this.port = port;
            return true;
        }

        public void Stop()
        {
            if (!IsStarted)
                return;

            Console.WriteLine($"Server[{port}]: stopping...");

            listener?.Stop();
            listener = null;

            listenerThread?.Interrupt();
            listenerThread = null;

            foreach (KeyValuePair<long, PolyTcpConnection> kvp in connectionDict)
            {
                kvp.Value.Disconnect();
                //TcpClient client = kvp.Value.client;
                //try { client.GetStream().Close(); } catch { }
                //client.Close();
            }

            connectionDict.Clear();
            counter = 0;
            Console.WriteLine($"Server[{port}]: stopped");
        }

        // send message to client using socket connection.
        public bool Send(long connectionId, ArraySegment<byte> data)
        {
            if (!IsStarted)
                return false;
            if (data.Count > MaxMessageSize)
            {
                Console.Error.WriteLine("Client.Send: message too big: " + data.Count + ". Limit: " + MaxMessageSize);
                return false;
            }
            if (connectionDict.TryGetValue(connectionId, out var connection))
            {
                connection.Send(data);
                return true;
            }
            return false;
        }

        public bool Disconnect(long connectionId)
        {
            if (connectionDict.TryGetValue(connectionId, out var token))
            {
                token.client.Close();
                Console.Error.WriteLine("Server.Disconnect connectionId:" + connectionId);
                return true;
            }
            return false;
        }
    }
}
