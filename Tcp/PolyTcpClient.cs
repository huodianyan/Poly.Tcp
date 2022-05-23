using System;
using System.Net.Sockets;

namespace Poly.Tcp
{
    public class PolyTcpClient : APolyTcpBase
    {
        private TcpClient client;
        private volatile bool connecting;
        private PolyTcpConnection connection;

        public bool IsConnected => connection != null && connection.IsConnected;

        public PolyTcpClient(IByteArrayPool arrayPool = null) : base(arrayPool)
        {
        }
        public bool Connect(string ip, int port)
        {
            if (connecting || IsConnected)
                return IsConnected;
            connecting = true;
            try
            {
                client = new TcpClient();
                client.Connect(ip, port);
                //connecting = false;

                client.NoDelay = NoDelay;
                client.SendTimeout = SendTimeout;

                connection = new PolyTcpConnection(this, client, 0);
                OnConnectionConnect(connection);
            }
            catch (Exception exception)
            {
                Disconnect();
                Console.Error.WriteLine("Client Exception: " + exception);
            }
            finally
            {
                connecting = false;
            }
            return IsConnected;
        }
        public void Disconnect()
        {
            if (connecting)
            {
                client.Close();
                client = null;
                connecting = false;
            }
            if (IsConnected)
            {
                connection.Disconnect();
                connection = null;
            }
        }
        public bool Send(ArraySegment<byte> data)
        {
            if (!IsConnected) return false;
            if (data.Count > MaxMessageSize)
            {
                Console.Error.WriteLine($"Client.Send: message too big: {data.Count}. Limit: {MaxMessageSize}");
                return false;
            }
            connection.Send(data);
            return true;
        }
    }
}
