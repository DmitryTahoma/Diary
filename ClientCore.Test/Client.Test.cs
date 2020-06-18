using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerCore;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ClientCore.Test
{
    [TestClass]
    public partial class ClientTest
    {
        SocketSettings.SocketSettings correctSettings = null;

        [TestInitialize]
        public void TestInitialize()
        {
            correctSettings = new SocketSettings.SocketSettings("192.168.0.107", 11221, 1000);
        }

        [DataTestMethod]
        [DataRow("192.168.0.107", 4000, "hello, server", "hello, client")]
        [DataRow("127.0.0.1", 4500, "hello", "message is received")]
        public void SendTest(string ip, int port, string message, string expectedRespnse)
        {
            correctSettings = new SocketSettings.SocketSettings(ip, port, 1000);
            Server server = new Server(new TestCommands(), correctSettings);
            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();
            Thread.Sleep(100);
            try
            {
                Client client = new Client(correctSettings);

                Task clients = new Task(new Action(() => { 
                    for (int i = 0; i < 10; ++i)
                        Assert.AreEqual(expectedRespnse, client.Send(message));
                }));
                clients.Start();
                clients.Wait(5000);
                Assert.IsTrue(clients.IsCompleted);
            }
            catch(Exception exc)
            {
                Assert.Fail(exc.ToString());
            }
            finally
            {
                server.Stop();
            }
        }

        [TestMethod]
        public void ServerIsMissingTest()
        {
            Client client = new Client(correctSettings);
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
        [DataRow("192.168.192.2", 3036, 150, "showMyParams", "hello", "param", "param3", "hello,param,param3")]
        [DataRow("192.168.192.2", 3038, 500, "showMyParams", "hi", "by", "ggwqas", "hi,by,ggwqas")]
        [DataRow("127.0.0.1", 10101, 100, "plus", "12", "4", "5", "21")]
        [DataRow("127.0.0.1", 1360, 300, "plus", "10", "40", "15", "65")]
        [DataRow("127.0.0.1", 1011, 400, "plus", "one", "8", "500", "ArgumentError")]
        [DataRow("127.0.0.1", 1012, 400, "plus", "1", "eight", "500", "ArgumentError")]
        [DataRow("127.0.0.1", 1013, 400, "plus", "1", "8", "five hundred", "ArgumentError")]
        public void SendCommandTest(string ip, int port, int mls, string cmd, string param1, string param2, string param3, string expectedResult)
        {
            correctSettings = new SocketSettings.SocketSettings(ip, port, mls);
            Server server = new Server(new TestCommands(), correctSettings);
            Thread serverThread = new Thread(server.Run);
            serverThread.Start();
            Thread.Sleep(100);
            try
            {
                Client client = new Client(correctSettings);
                string result = "";
                Thread clientThread = new Thread(() =>
                {
                    result = client.SendCommand(cmd, new string[] { param1, param2, param3 }); 
                });
                clientThread.Start();
                Thread.Sleep(600);

                Assert.AreEqual(expectedResult, result);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}