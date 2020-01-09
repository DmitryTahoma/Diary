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
        [DataRow("192.168.192.2", 11221, 11222)]
        [DataRow("127.0.0.1", 11223, 11224)]
        public void ServerProgramRunTest(string serverIp, int serverPort, int clientPort)
        {
            ServerProgram server = new ServerProgram(serverIp, serverPort, new int[] { clientPort }, 3000);
            bool isAsync = false;
            Thread serverThread = new Thread(() => 
            {
                server.Run(); 
                isAsync = true; 
            });
            serverThread.Start();

            Client client = new Client(new SocketSettings.SocketSettings(serverIp, serverPort, new int[] { clientPort }, 3000));
            string result = "";
            Thread clientThread = new Thread(() => { result = client.Send("cc"); });
            clientThread.Start();

            Thread.Sleep(1000);

            Assert.IsTrue(isAsync);
            Assert.AreEqual("cs", result);
        }

        [DataTestMethod]
        [DataRow("192.168.192.2", 11221, 11222, 3000)]
        [DataRow("127.0.0.1", 11223, 11224, 4000)]
        public void ServerProgramStopTest(string serverIp, int serverPort, int clientPort, int mlsOfDelay)
        {
            ServerProgram server = new ServerProgram(serverIp, serverPort, new int[] { clientPort }, mlsOfDelay);
            server.Run();

            Client client = new Client(new SocketSettings.SocketSettings(serverIp, serverPort, new int[] { clientPort }, mlsOfDelay));
            string result = "";
            Thread clientThread = new Thread(() => { result = client.Send("cc"); });
            string result2 = "";
            Thread clientThread2 = new Thread(() => { result2 = client.Send("cc"); });

            clientThread.Start();
            Thread.Sleep(1000);

            int mlsToStop = server.Stop();
            Thread.Sleep(mlsOfDelay);

            clientThread2.Start();
            Thread.Sleep(1000);

            Assert.AreEqual(mlsOfDelay, mlsToStop);
            Assert.AreEqual("cs", result);
            Assert.AreEqual("", result2);
        }

        /*
        Before running test, I have next values ​​in the tables of the test database
        table 'names'
        id  name        surname         patronymic
        1 	TempName 	TempSurname 	TempPatronymic
	    2 	TestName 	TestSurname 	TestPatronymic
        table 'users'
        id  name_id     login           password            registration
        1 	1 	        TempLogin 	    temppassword 	    2020-01-01 18:52:23
	    2 	2 	        TestLogin 	    testpassword 	    2020-01-01 18:52:23
        */
        [DataTestMethod]
        [DataRow("192.168.0.106", 11001, 11002, "clp", new string[] { "nick", "name" }, "False")]
        [DataRow("192.168.0.106", 11003, 11004, "clp", new string[] { "TempName", "temppassword" }, "False")]
        [DataRow("192.168.192.2", 11005, 11006, "clp", new string[] { "TempLogin", "temppassword" }, "True")]
        [DataRow("192.168.192.2", 11007, 11008, "clp", new string[] { "TestLogin", "temppassword" }, "False")]
        [DataRow("127.0.0.1", 11009, 11010, "clp", new string[] { "TestLogin", "testpassword" }, "True")]
        [DataRow("127.0.0.1", 11011, 11012, "clp", new string[] { "TempSurname", "TestPatronymic" }, "False")]
        public void ExecuteCommandTest(string ip, int port, int clientPort, string command, string[] args, string expectedResult)
        {
            ServerProgram server = new ServerProgram(ip, port, new int[] { clientPort }, 1000);
            server.Run();

            try
            {
                string result = server.ExecuteCommand(command, args);

                Assert.AreEqual(expectedResult, result);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
