//using System;
//using System.Threading.Tasks;

//namespace Poly.Transport
//{
//    public enum EDeliveryMethod : byte
//    {
//        /// <summary>
//        /// Unreliable. Packets can be dropped, can be duplicated, can arrive without order.
//        /// </summary>
//        Unreliable = 4,

//        /// <summary>
//        /// Reliable. Packets won't be dropped, won't be duplicated, can arrive without order.
//        /// </summary>
//        ReliableUnordered = 0,

//        /// <summary>
//        /// Unreliable. Packets can be dropped, won't be duplicated, will arrive in order.
//        /// </summary>
//        Sequenced = 1,

//        /// <summary>
//        /// Reliable and ordered. Packets won't be dropped, won't be duplicated, will arrive in order.
//        /// </summary>
//        ReliableOrdered = 2,

//        /// <summary>
//        /// Reliable only last packet. Packets can be dropped (except the last one), won't be duplicated, will arrive in order.
//        /// </summary>
//        ReliableSequenced = 3
//    }

//    public interface ITransport : IDisposable
//    {
//        //bool IsClientStarted();
//        //bool Connect(string address, int port, Action<bool> callback);
//        bool Connect(string address, int port);
//        void Disconnect();
//        //bool IsServerStarted();
//        bool StartListen(int port, int maxConnections);
//        void CloseConnection(long connectionId);
//        void StopListen();
//        //void Dispose();
//        //int GetServerPeersCount();

//        bool Send(long connectionId, EDeliveryMethod deliveryMethod, ArraySegment<byte> data);
//        bool PollEvent(out TransportEventData eventData);
//    }

//    public interface ITransportFactory
//    {
//        bool CanUseWithWebGL { get; }
//        ITransport Build();
//    }

//    public static class TransportExtensions
//    {
//        //public static async Task<bool> StartClientAsync(this ITransport transport, string address, int port)
//        //{
//        //    var task = new TaskCompletionSource<bool>();
//        //    transport.Connect(address, port, (ok) =>
//        //    {
//        //        task.SetResult(ok);
//        //    });
//        //    return await task.Task;
//        //}

//    }
//}