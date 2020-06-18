using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logger;
using System.Text;
using System.Text.RegularExpressions;

namespace ServerCore.Test
{
    [TestClass]
    public partial class ServerTest
    {
        SocketSettings.SocketSettings correctSettings = null;

        [TestInitialize]
        public void TestInitialize()
        {
            correctSettings = new SocketSettings.SocketSettings("192.168.0.107", 11221, 1000);
        }

        [DataTestMethod]
        [DataRow("hello, server", "192.168.0.107", 11221, "hello, client")]
        [DataRow("hi, server", "127.0.0.1", 11321, "message is received")]
        public void ServerIsRunning(string message, string serverIP, int serverPort, string expectedResponce)
        {
            correctSettings = new SocketSettings.SocketSettings(serverIP, serverPort, 1000);
            Server server = new Server(new TestCommands(), serverIP, serverPort, 100);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            try
            {
                Assert.AreEqual(expectedResponce, Send(message));
            }
            finally
            {
                server.Stop();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SocketException))]
        public void ServerIsStops()
        {
            Server server = new Server(new TestCommands(), correctSettings);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            Thread.Sleep(100);

            Thread serverThread2 = new Thread(new ThreadStart(server.Stop));
            serverThread2.Start();
            Thread.Sleep(1100);

            TcpClient client = new TcpClient();
            client.Connect(correctSettings.ServerIP, correctSettings.ServerPort);

            byte[] msg = Encoding.Unicode.GetBytes("are you here?");
            client.GetStream().Write(msg, 0, msg.Length);

            if(client != null)
                client.Close();
        }

        [TestMethod]
        public void VoidCommands()
        {
            Server server = new Server(null, correctSettings);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            try
            {
                Assert.AreEqual("c0", Send("what you can?"));
            }
            finally
            {
                server.Stop();
            }
        }

        [DataTestMethod]
        [DataRow("192.168.0.107", 11221)]
        [DataRow("192.168.0.117", 11223)]
        public void IsSingleton(string ip, int serverPort)
        {
            Server server = new Server(null, new SocketSettings.SocketSettings(ip, serverPort, 300));
            Server server2 = new Server(null, new SocketSettings.SocketSettings(ip, serverPort, 300));
            try
            {
                Thread serverThread = new Thread(new ThreadStart(() => { server.Run(); }));
                Thread server2Thread = new Thread(new ThreadStart(() => { server.Run(); }));
                serverThread.Start();
                server2Thread.Start();
                Thread.Sleep(100);
                Assert.IsFalse(Server.IsFreePort(serverPort, ip));
                Assert.IsTrue(server.IsStarted);
                Assert.IsFalse(server2.IsStarted);
            }
            finally
            {
                if (server.IsStarted)
                    server.Stop();
                if (server2.IsStarted)
                    server2.Stop();
            }
        }

        [TestMethod]
        public void ServerIsLogging()
        {
            Server server = new Server(new TestCommands(), correctSettings);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            Thread.Sleep(100);
            try
            {
                Logger.Logger serverLogger = server.GetLogger();

                Assert.IsTrue(serverLogger.Logs[0].Text.Contains("Server started"));
                Assert.AreEqual(EntryLevel.Server, serverLogger.Logs[0].Level);
                Assert.AreEqual(1, serverLogger.Logs.Count());
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }

            server = new Server(null, correctSettings);
            serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            Thread.Sleep(100);
            try
            {
                Logger.Logger serverLogger = server.GetLogger();

                Assert.IsTrue(serverLogger.Logs[0].Text.Contains("Server started"));
                Assert.AreEqual(EntryLevel.Server, serverLogger.Logs[0].Level);
                Assert.AreEqual("Commands slot is void", serverLogger.Logs[1].Text);
                Assert.AreEqual(EntryLevel.Server, serverLogger.Logs[1].Level);

                Send("test");

                Regex regex = new Regex("CLIENT-T-\\d+ REQUEST: test RESPONSE: c0");
                Assert.IsTrue(regex.IsMatch(serverLogger.Logs.LastByTime().Text));
                Assert.AreEqual(EntryLevel.User, serverLogger.Logs.LastByTime().Level);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }

        [DataTestMethod]
        [DataRow("smth", "192.168.0.107", 10000, 5000, 4500)]
        [DataRow("test", "127.0.0.1", 5067, 3000, 2750)]
        public void ClientWaitResponseWithDelayTest(string message, string ip, int port, int serverWait, int clientWait)
        {
            correctSettings = new SocketSettings.SocketSettings(ip, port, serverWait);
            Server server = new Server(null, correctSettings);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            try
            {
                Assert.AreEqual("c0", Send(message, clientWait));
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }
        
        [DataTestMethod]
        [DataRow("multiplyAll", new string[] { "2", "8", "4", "3" }, "192")]
        [DataRow("multiplyAll", new string[] { "5", "5", "5", "3" }, "375")]
        public void ServerIsProcessesParameters(string command, string[] parameters, string expectedResult)
        {
            Server server = new Server(new TestCommands(), correctSettings);
            Thread serverThread = new Thread(server.Run);
            serverThread.Start();
            Thread.Sleep(100);
            try
            {
                string message = command;
                if(parameters != null)
                    for (int i = 0; i < parameters.Length; ++i)
                        message += "|" + parameters[i];

                Assert.AreEqual(expectedResult, Send(message));
            }
            finally
            {
                server.Stop();
            }
        }
    }
}