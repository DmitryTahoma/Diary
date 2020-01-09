using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ClientCore.Test
{
    [TestClass]
    public partial class ClientTest
    {
        [DataTestMethod]
        [DataRow("192.168.0.106", 4000, new int[] { 1001, 2002, 3003 }, 4004, "hello, server", "hello, client")]
        [DataRow("127.0.0.1", 4500, new int[] { 1201, 2302, 3403 }, 4504, "hello", "message is received")]
        public void SendTest(string ip, int port, int[] busyPortsForClient, int freeClientPort, string message, string expectedRespnse)
        {
            try
            {
                for (int i = 0; i < busyPortsForClient.Length; ++i)
                    PortsHelper.OccupyPort(busyPortsForClient[i]);

                List<int> clientPorts = new List<int>(busyPortsForClient);
                clientPorts.Add(freeClientPort);

                Server server = new Server(new TestCommands(), new SocketSettings.SocketSettings(ip, port, new int[] { freeClientPort }, 5000));
                Thread serverThread = new Thread(new ThreadStart(server.Run));
                serverThread.Start();
                Thread.Sleep(100);

                Client client = new Client(new SocketSettings.SocketSettings(ip, port,  clientPorts.ToArray(), 3000));

                for (int i = 0; i < 25; ++i)
                    Assert.AreEqual(expectedRespnse, client.Send(message));
            }
            catch(Exception exc)
            {
                Assert.Fail(exc.ToString());
            }
            finally
            {
                PortsHelper.Dispose();
            }
        }

        [TestMethod]
        public void ServerIsMissingTest()
        {
            Client client = new Client(new SocketSettings.SocketSettings());
            string result = "";
            bool isThrown = false;
            Task connection = Task.Factory.StartNew(() => {
                try
                {
                    result = client.Send("test");
                }
                catch(SocketException)
                {
                    isThrown = true;
                }
            });
            connection.Wait(10000);

            Assert.AreEqual("", result);
            Assert.IsTrue(isThrown);
        }

        [DataTestMethod]
        [DataRow("192.168.192.2", 3036, 3037, 1500, "showMyParams", "hello", "param", "param3", "hello,param,param3")]
        [DataRow("192.168.192.2", 3038, 3039, 5000, "showMyParams", "hi", "by", "ggwqas", "hi,by,ggwqas")]
        [DataRow("127.0.0.1", 10101, 10102, 1000, "plus", "12", "4", "5", "21")]
        [DataRow("127.0.0.1", 1360, 1370, 3000, "plus", "10", "40", "15", "65")]
        [DataRow("127.0.0.1", 1011, 1102, 4000, "plus", "one", "8", "500", "ArgumentError")]
        [DataRow("127.0.0.1", 1012, 1103, 4000, "plus", "1", "eight", "500", "ArgumentError")]
        [DataRow("127.0.0.1", 1013, 1104, 4000, "plus", "1", "8", "five hundred", "ArgumentError")]
        public void SendCommandTest(string ip, int port, int clientPort, int mls, string cmd, string param1, string param2, string param3, string expectedResult)
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings(ip, port, new int[] { clientPort }, mls);
            Server server = new Server(new TestCommands(), settings);
            Thread serverThread = new Thread(server.Run);
            serverThread.Start();

            try
            {
                Client client = new Client(settings);
                string result = "";
                Thread clientThread = new Thread(() =>
                {
                    result = client.SendCommand(cmd, new string[] { param1, param2, param3 }); 
                });
                clientThread.Start();
                Thread.Sleep(1000);

                Assert.AreEqual(expectedResult, result);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
