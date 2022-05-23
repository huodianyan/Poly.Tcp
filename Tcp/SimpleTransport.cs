//using System;
//using System.Buffers;
//using System.Collections.Concurrent;
//using System.Collections.Generic;

//namespace Poly.Transport
//{

//    public sealed class SimpleTransport : ITransport
//    {
//        private SimpleClient client;
//        private SimpleServer server;
//        private ConcurrentQueue<TransportEventData> clientEventQueue;
//        private ConcurrentQueue<TransportEventData> serverEventQueue;

//        public SimpleTransport()
//        {
//        }

//        private bool IsClientStarted()
//        {
//            return client != null && client.IsConnected;
//        }
//        public bool Connect(string address, int port)//, Action<bool> callback
//        {
//            if (IsClientStarted())
//                return false;

//            client = new SimpleClient();
//            clientEventQueue = new ConcurrentQueue<TransportEventData>();
//            client.OnConnectEvent += (connection) =>
//            {
//                clientEventQueue.Enqueue(new TransportEventData()
//                {
//                    TransportEvent = ETranportEvent.ConnectEvent,
//                    ConnectionId = connection.ConnectionId,
//                });
//            };
//            client.OnDisconnectEvent += (connection) =>
//            {
//                clientEventQueue.Enqueue(new TransportEventData()
//                {
//                    TransportEvent = ETranportEvent.DisconnectEvent,
//                    ConnectionId = connection.ConnectionId,
//                });
//            };
//            client.OnRecieveEvent += (connection, segment) =>
//            {
//                var dest = ArrayPool<byte>.Shared.Rent(segment.Count);
//                Array.Copy(segment.Array, segment.Offset, dest, 0, segment.Count);
//                //Console.WriteLine($"SimpleTransport.OnRecieveEvent: {dest.Length}, {segment.Count}");

//                clientEventQueue.Enqueue(new TransportEventData()
//                {
//                    TransportEvent = ETranportEvent.DataEvent,
//                    ConnectionId = connection.ConnectionId,
//                    IsPooled = true,
//                    Segment = segment
//                    //Reader = new NetDataReader(dest, 0, segment.Count, 0),
//                });
//            };
//            var ok = client.Connect(address, port);
//            if (!ok)
//                Disconnect();
//            return ok;
//        }

//        //public bool ClientSend(EDeliveryMethod deliveryMethod, byte[] data, int start, int length)
//        //{
//        //    if (!IsClientStarted())
//        //        return false;
//        //    //Console.WriteLine($"SimpleTransport.ClientSend: {data.Length}, {start}, {length}");
//        //    return client.Send(data, start, length);
//        //}

//        //public bool ClientReceive(out TransportEventData eventData)
//        //{
//        //    eventData = default(TransportEventData);
//        //    if (client == null)
//        //        return false;
//        //    //client.PollEvents();
//        //    if (clientEventQueue == null || clientEventQueue.Count == 0)
//        //        return false;
//        //    return clientEventQueue.TryDequeue(out eventData);
//        //}
//        public void Disconnect()
//        {
//            client?.Disconnect();
//            //clientEventQueue = null;
//            client = null;
//        }

//        private bool IsServerStarted()
//        {
//            return server != null && server.IsStarted;
//        }
//        public bool StartListen(int port, int maxConnections)
//        {
//            if (IsServerStarted())
//                return false;
//            server = new SimpleServer();
//            serverEventQueue = new ConcurrentQueue<TransportEventData>();
//            if (!server.Start(port))
//                return false;
//            server.OnConnectEvent += (connection) =>
//            {
//                serverEventQueue.Enqueue(new TransportEventData()
//                {
//                    TransportEvent = ETranportEvent.ConnectEvent,
//                    ConnectionId = connection.ConnectionId
//                });
//            };
//            server.OnDisconnectEvent += (connection) =>
//            {
//                serverEventQueue.Enqueue(new TransportEventData()
//                {
//                    TransportEvent = ETranportEvent.DisconnectEvent,
//                    ConnectionId = connection.ConnectionId
//                });
//            };
//            server.OnRecieveEvent += (connection, segment) =>
//            {
//                var dest = ArrayPool<byte>.Shared.Rent(segment.Count);
//                Array.Copy(segment.Array, segment.Offset, dest, 0, segment.Count);
//                serverEventQueue.Enqueue(new TransportEventData()
//                {
//                    TransportEvent = ETranportEvent.DataEvent,
//                    ConnectionId = connection.ConnectionId,
//                    IsPooled = true,
//                    Segment = segment
//                    //Reader = new NetDataReader(dest, 0, segment.Count, 0)
//                });
//            };
//            return true;
//        }
//        //public bool ServerSend(long connectionId, EDeliveryMethod deliveryMethod, byte[] data, int start, int length)
//        //{
//        //    if (!IsServerStarted())
//        //        return false;
//        //    //Console.WriteLine($"SimpleTransport.ServerSend: [{connectionId}] {data.Length}, {start}, {length}");
//        //    return server.Send((int)connectionId, data, start, length);
//        //}
//        //public bool ServerReceive(out TransportEventData eventData)
//        //{
//        //    eventData = default(TransportEventData);
//        //    if (server == null)
//        //        return false;
//        //    //client.PollEvents();
//        //    if (serverEventQueue == null || serverEventQueue.Count == 0)
//        //        return false;
//        //    return serverEventQueue.TryDequeue(out eventData);
//        //}
//        public void CloseConnection(long connectionId)
//        {
//            if (server == null)
//                return;
//            server.Disconnect(connectionId);
//        }
//        public void StopListen()
//        {
//            server?.Stop();
//            server = null;
//            //serverEventQueue = null;
//        }
//        public void Dispose()
//        {
//            Disconnect();
//            StopListen();
//        }
//        //public int GetServerPeersCount()
//        //{
//        //    if (server == null)
//        //        return 0;
//        //    return server.Connections.Count;
//        //}

//        public bool Send(long connectionId, EDeliveryMethod deliveryMethod, ArraySegment<byte> data)
//        {
//            if (connectionId == 0)
//            {
//                if (!IsClientStarted())
//                    return false;
//                return client.Send(data);
//            }
//            else
//            {
//                if (!IsServerStarted())
//                    return false;
//                return server.Send(connectionId, data);
//            }
//            //if (IsClientStarted())
//            //    return client.Send(data);
//            //else if (IsServerStarted())
//            //    return server.Send(connectionId, data);
//            //return false;
//        }

//        public bool PollEvent(out TransportEventData eventData)
//        {
//            eventData = default(TransportEventData);
//            //if (IsServerStarted())
//            {
//                if (serverEventQueue != null && serverEventQueue.Count > 0)
//                {
//                    return serverEventQueue.TryDequeue(out eventData);
//                }
//            }
//            //if (IsClientStarted())
//            {
//                if (clientEventQueue != null && clientEventQueue.Count > 0)
//                {
//                    return clientEventQueue.TryDequeue(out eventData);
//                }
//            }
//            return false;
//        }

//    }
//}