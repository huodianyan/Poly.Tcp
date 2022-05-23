//using System;

//namespace Poly.Tcp
//{
//    public enum ENetEvent
//    {
//        DataEvent,
//        ConnectEvent,
//        DisconnectEvent,
//        ErrorEvent,
//    }
//    public struct PolyTcpMessage
//    {
//        public ENetEvent NetEvent;
//        public long ConnectionId;
//        public ArraySegment<byte> Segment;

//        public PolyTcpMessage(long connectionId, ENetEvent netEvent, ArraySegment<byte> segment)
//        {
//            ConnectionId = connectionId;
//            NetEvent = netEvent;
//            Segment = segment;
//        }
//    }
//}
