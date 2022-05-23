//using System;
//using System.Buffers;
//using System.Net;
//using System.Net.Sockets;

//namespace Poly.Transport
//{
//    public enum ETranportEvent
//    {
//        DataEvent,
//        ConnectEvent,
//        DisconnectEvent,
//        ErrorEvent,
//    }

//    public struct TransportEventData : IDisposable
//    {
//        public ETranportEvent TransportEvent;
//        public long ConnectionId;
//        //public NetDataReader Reader;
//        public ArraySegment<byte> Segment;
//        public IPEndPoint EndPoint;
//        public SocketError SocketError;
//        public bool IsPooled;
//        //public bool IsClient;

//        public void Dispose()
//        {
//            if (IsPooled && Segment.Array != null)
//                ArrayPool<byte>.Shared.Return(Segment.Array);
//        }

//        public override string ToString()
//        {
//            return $"TransportEventData:{{{TransportEvent},{ConnectionId},{Segment}}}";
//        }
//    }
//}