using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logger;
using System.Collections.Generic;
using ClientCore;

namespace ServerCore.Test
{
    [TestClass]
    public partial class ServerTest
    {
        [DataTestMethod]
        [DataRow("hello, server", 11222, "192.168.0.106", 11221, "hello, client")]
        [DataRow("hi, server", 11322, "127.0.0.1", 11321, "message is received")]
        public void ServerIsRunning(string message, int clientPort, string serverIP, int serverPort, string expectedResponce)
        {
            Server server = new Server(new TestCommands(), serverIP, serverPort, new int[] { clientPort }, 1000);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();

            TcpClient client = null;
            try
            {
                client = new TcpClient(serverIP, serverPort);
                Assert.AreEqual(expectedResponce, Send(message, client, serverIP, clientPort));
            }
            finally
            {
                if(client != null)
                    client.Dispose();
                server.Stop();
            }
        }

        [TestMethod]
        public void ServerIsStops()
        {
            Server server = new Server(new TestCommands());
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            Thread.Sleep(100);//time to start the server
            Thread serverThread2 = new Thread(new ThreadStart(server.Stop));
            serverThread2.Start();
            Thread.Sleep(31000);//time for safe server shutdown
            try
            {
                Assert.AreNotEqual("message is received",
                    Send("are you here?", new TcpClient(getLocalIP(), 11221), getLocalIP()));
            }
            catch(SocketException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void VoidCommands()
        {
            Server server = new Server(null);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            try
            {
                Assert.AreEqual("c0", 
                    Send("what you can?", new TcpClient(getLocalIP(), 11221), getLocalIP()));
            }
            finally
            {
                server.Stop();
            }
        }

        [DataTestMethod]
        [DataRow("192.168.0.106", 11221, 11222)]
        [DataRow("192.168.0.116", 11223, 11250)]
        public void IsSingleton(string ip, int serverPort, int clientPort)
        {
            Server server = new Server(null, new SocketSettings.SocketSettings(ip, serverPort, new int[] { clientPort }, 3000));
            Server server2 = new Server(null, new SocketSettings.SocketSettings(ip, serverPort, new int[] { clientPort }, 3000));
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
        public void ServerRespondsToAnotherPort()
        {
            Server server = new Server(null);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            Thread.Sleep(100);
            try
            {
                //standard port
                Task client1 = Task.Factory.StartNew(() => { 
                    Assert.AreEqual("c0",
                        Send("test1", new TcpClient(getLocalIP(), 11221), getLocalIP()));
                });
                client1.Wait(2000);
                Assert.IsTrue(client1.IsCompleted);
                //another ports
                Task client2 = Task.Factory.StartNew(() => {
                    Assert.AreEqual("c0",
                        Send("test1", new TcpClient(getLocalIP(), 11221), getLocalIP(), 12345));
                });
                client2.Wait(2000);
                Assert.IsTrue(client2.IsCompleted);

                Task client3 = Task.Factory.StartNew(() => {
                    Assert.AreEqual("c0",
                    Send("test1", new TcpClient(getLocalIP(), 11221), getLocalIP(), 18535));
                });
                client3.Wait(2000);
                Assert.IsTrue(client3.IsCompleted);
            }
            finally
            {
                if(server != null)
                    server.Stop();
            }
        }

        [TestMethod]
        public void ServerIsLogging()
        {
            Server server = new Server(null);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            Thread.Sleep(100);
            try
            {
                Logger.Logger serverLogger = server.GetLogger();

                Assert.IsTrue(serverLogger.Logs[0].Text.Contains("Server started"));
                Assert.AreEqual(EntryLevel.Server, serverLogger.Logs[0].Level);
                Assert.AreEqual("Commands slot is void", serverLogger.Logs[1].Text);
                Assert.AreEqual(EntryLevel.Server, serverLogger.Logs[1].Level);

                Task client = Task.Factory.StartNew(() => {
                    Send("test", new TcpClient(getLocalIP(), 11221), getLocalIP());
                });
                client.Wait(2000);

                Assert.AreEqual("CLIENT: " + getLocalIP() + ":11222 REQUEST: test RESPONSE: c0", serverLogger.Logs.LastByTime().Text);
                Assert.AreEqual(EntryLevel.User, serverLogger.Logs.LastByTime().Level);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }

        [DataTestMethod]
        [DataRow("smth", "192.168.0.106", 10000, 10001, 1000)]
        [DataRow("message", "127.0.0.1", 5067, 5066, 600)]
        [DataRow("smth", "192.168.0.106", 10000, -1, 1000)]
        [DataRow("message", "127.0.0.1", 5067, -1, 600)]
        public void ClientWaitResponseWithDelayTest(string message, string ip, int port, int clientPort, int mls)
        {
            if (clientPort == -1)
                clientPort = 11222;
            Server server = new Server(null, ip, port, new int[] { clientPort }, mls);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();

            try
            {
                string result = "";

                Task client = Task.Factory.StartNew(() => {
                    result = SendAndWaitResponseWithDelay(message, ip, port, clientPort, mls / 2);
                });
                client.Wait(mls / 2 + 1000);

                Assert.AreEqual("c0", result);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }

        [DataTestMethod]
        [DataRow("smth", "192.168.0.106", 10000, 10001, 1000)]
        [DataRow("message", "127.0.0.1", 5067, 5066, 600)]
        [DataRow("smth", "192.168.0.106", 10000, -1, 1000)]
        [DataRow("message", "127.0.0.1", 5067, -1, 600)]
        public void ClientNotWaitResponseTest(string message, string ip, int port, int clientPort, int mls)
        {
            List<int> ports = new List<int>();
            if (clientPort == -1)
            {
                ports.Add(123); ports.Add(234); ports.Add(456); ports.Add(567);
            }
            else
                ports.Add(clientPort);
            Server server = new Server(null, ip, port, ports.ToArray(), mls);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            Thread.Sleep(100);

            try
            {
                Task client = Task.Factory.StartNew(() => {
                    SendNoWaitResponse(message, ip, port, clientPort);
                });
                Thread.Sleep(3 * mls);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }

            Logger.Logger logger = server.GetLogger();
            Assert.AreEqual("CLIENT: " + ip + (clientPort != -1? ":" + clientPort.ToString() : "") + " didn't respond", logger.Logs.LastAdded().Text);
            Assert.AreEqual(EntryLevel.User, logger.Logs.LastAdded().Level);
            Assert.IsTrue(new TimeSpan(0, 0, 0, 5, 0) > DateTime.Now - logger.Logs.LastAdded().Time);
        }

        [DataTestMethod]
        [DataRow("192.168.192.2", 11011, 11111, "multiplyAll", new string[] { "2", "8", "4", "3" }, "192")]
        [DataRow("127.0.0.1", 11211, 11311, "multiplyAll", new string[] { "5", "5", "5", "3" }, "375")]
        public void ServerIsProcessesParameters(string ip, int port, int clientPort, string command, string[] parameters, string expectedResult)
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings(ip, port, new int[] { clientPort }, 1000);
            Server server = new Server(new TestCommands(), settings);
            Thread serverThread = new Thread(server.Run);
            serverThread.Start();

            try
            {
                Client client = new Client(settings);
                string message = command;
                if(parameters != null)
                    for (int i = 0; i < parameters.Length; ++i)
                        message += "|" + parameters[i];

                string result = "";
                Thread clientThread = new Thread(() => { result = client.Send(message); });
                clientThread.Start();
                Thread.Sleep(500);

                Assert.AreEqual(expectedResult, result);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
