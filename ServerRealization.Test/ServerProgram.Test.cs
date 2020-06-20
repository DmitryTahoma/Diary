using ClientCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace ServerRealization.Test
{
    [TestClass]
    public class ServerProgramTest
    {
        [DataTestMethod]
        [DataRow("192.168.192.2", 11221)]
        [DataRow("127.0.0.1", 11223)]
        public void ServerProgramRunTest(string serverIp, int serverPort)
        {
            ServerProgram server = new ServerProgram(serverIp, serverPort, 300);
            bool isAsync = false;
            Thread serverThread = new Thread(() => 
            {
                server.Run(); 
                isAsync = true; 
            });
            serverThread.Start();

            Client client = new Client(new SocketSettings.SocketSettings(serverIp, serverPort, 300));
            string result = "";
            Thread clientThread = new Thread(() => { result = client.Send("cc"); });
            clientThread.Start();

            Thread.Sleep(1000);

            Assert.IsTrue(isAsync);
            Assert.AreEqual("cs", result);
            Thread.Sleep(server.Stop());
        }

        [DataTestMethod]
        [DataRow("192.168.192.2", 11221, 300)]
        [DataRow("127.0.0.1", 11223, 400)]
        public void ServerProgramStopTest(string serverIp, int serverPort, int mlsOfDelay)
        {
            ServerProgram server = new ServerProgram(serverIp, serverPort, mlsOfDelay);
            server.Run();

            Client client = new Client(new SocketSettings.SocketSettings(serverIp, serverPort, mlsOfDelay));
            
            string result1 = "";
            Task client1 = Task.Factory.StartNew(() => { result1 = client.Send("cc"); });
            client1.Wait(1000);

            int mlsToStop = server.Stop();
            Thread.Sleep(mlsOfDelay);

            string result2 = "";
            Task client2 = Task.Factory.StartNew(() => { result2 = client.Send("cc"); });
            client2.Wait(1000);

            Assert.AreEqual(mlsOfDelay, mlsToStop);
            Assert.AreEqual("cs", result1);
            Assert.AreEqual("", result2);
        }

        /*
        Before running test, I have next values ​​in the tables of the test database
        table 'users'
        id | name | login  | password | registration       
        1  | Alex | Alex92 | pass1234 | 2020-01-10 21:33:11
        */
        [DataTestMethod]
        [DataRow("192.168.0.106", 11001, 11002, "clp", new string[] { "nick", "name" }, "False")]
        [DataRow("192.168.0.106", 11003, 11004, "clp", new string[] { "Alex92", "temppassword" }, "False")]
        [DataRow("192.168.192.2", 11005, 11006, "clp", new string[] { "TempLogin", "pass1234" }, "False")]
        [DataRow("192.168.192.2", 11007, 11008, "clp", new string[] { "Alex92", "pass1234" }, "True")]
        [DataRow("127.0.0.1", 11009, 11010, "clp", new string[] { "TestLogin", "testpassword" }, "False")]
        [DataRow("127.0.0.1", 11011, 11012, "clp", new string[] { "Alex", "pass1234" }, "False")]
        public void ExecuteCommandTest(string ip, int port, int clientPort, string command, string[] args, string expectedResult)
        {
            ServerProgram server = new ServerProgram(ip, port, 100);
            server.Run();

            try
            {
                string result = server.ExecuteCommand(command, args);

                Assert.AreEqual(expectedResult, result);
            }
            finally
            {
                Thread.Sleep(server.Stop());
            }
        }
    }
}