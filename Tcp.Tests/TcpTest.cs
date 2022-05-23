using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Poly.Tcp.Tests
{
    [TestClass]
    public partial class TcpTest
    {
        private string address = "localhost";
        private int port = 9000;
        private PolyTcpServer server;
        private PolyTcpClient client;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }
        [ClassCleanup]
        public static void ClassCleanup()
        {
        }
        [TestInitialize]
        public void TestInitialize()
        {
        }
        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public async Task ConnectTest()
        {
            var tcs = new TaskCompletionSource<bool>();

            server = new PolyTcpServer();
            server.OnConnectEvent += (connId) => Console.WriteLine($"OnServerConnect: {connId}");
            server.OnDisconnectEvent += (connId) => Console.WriteLine($"OnServerDisconnect: {connId}");
            server.OnRecieveEvent += (connId, segment) =>
            {
                Assert.AreEqual(3, segment.Count);
                Assert.AreEqual(0x37, segment.Array[2]);
                Console.WriteLine($"OnServerRecieve: {connId}, {segment.Count}");
                segment.Array[2] = 0x47;
                server.Send(connId, segment);
            };
            var ok = server.Start(port);
            Assert.IsTrue(server.IsStarted);

            client = new PolyTcpClient();
            client.OnConnectEvent += (connId) => Console.WriteLine($"OnClientConnect: {connId}");
            client.OnDisconnectEvent += (connId) => Console.WriteLine($"OnClientDisconnect: {connId}");
            client.OnRecieveEvent += (connId, segment) =>
            {
                Assert.AreEqual(3, segment.Count);
                Assert.AreEqual(0x47, segment.Array[2]);
                tcs.SetResult(true);
            };

            ok = client.Connect(address, port);

            Assert.IsTrue(client.IsConnected);
            byte[] message = new byte[] { 0x42, 0x13, 0x37 };
            ok = client.Send(new ArraySegment<byte>(message));
            Assert.IsTrue(ok);

            await tcs.Task;

            client.Disconnect();
            Assert.IsFalse(client.IsConnected);
            client = null;

            //server.OnConnectEvent -= OnServerConnect;
            //server.OnDisconnectEvent -= OnServerDisconnect;
            //server.OnRecieveEvent -= OnServerRecieve;
            server.Stop();
            Assert.IsFalse(server.IsStarted);
            server = null;
        }
    }
}