using Shell.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientCore;
using ServerRealization;

namespace ShellModel.Test
{
    [TestClass]
    public class DBHelperTest
    {
        [DataTestMethod]
        [DataRow("templogin1", "password", "Dmitry")]
        [DataRow("mylogin", "herhdfhsdf22", "Tahoma")]
        public void RegistrationTest(string login, string password, string name)
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings("192.168.0.105", 11221, new int[] { 11222, 11224, 12550 }, 3000);
            ServerProgram server = new ServerProgram("192.168.0.105", 11221, new int[] { 11222, 11224, 12550 }, 3000);
            server.Run();

            DBHelper helper = new DBHelper(settings);
            bool isHappened = helper.Registration(login, password, name);
            Assert.AreEqual(true, isHappened);

            Client client = new Client(settings);
            string result = client.SendCommand("clp", new string[] { login, password });

            Assert.AreEqual("True", result);
        }
    }
}
