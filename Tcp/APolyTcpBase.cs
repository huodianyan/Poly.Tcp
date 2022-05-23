using System;

namespace Poly.Tcp
{
    public abstract class APolyTcpBase
    {
        internal IByteArrayPool arrayPool;

        public bool NoDelay = true;
        public int MaxMessageSize = 64 * 1024;
        public int SendTimeout = 5000;
        //public IByteArrayPool ArrayPool => arrayPool;

        public event Action<long> OnConnectEvent;
        public event Action<long> OnDisconnectEvent;
        public event Action<long, ArraySegment<byte>> OnRecieveEvent;

        public APolyTcpBase(IByteArrayPool arrayPool = null)
        {
            this.arrayPool = arrayPool ?? new ByteArrayPool();
        }
        internal virtual void OnConnectionConnect(PolyTcpConnection connection)
        {
            OnConnectEvent?.Invoke(connection.connectionId);
        }
        internal virtual void OnConnectionRecieve(PolyTcpConnection connection, ArraySegment<byte> segment)
        {
            OnRecieveEvent?.Invoke(connection.connectionId, segment);
        }
        internal virtual void OnConnectionError(PolyTcpConnection connection, string error)
        {

        }
        internal virtual void OnConnectionDisconnect(PolyTcpConnection connection)
        {
            OnDisconnectEvent?.Invoke(connection.connectionId);
        }
    }
}
