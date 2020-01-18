using Shell.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientCore;
using ServerRealization;

namespace Shell.Test
{
    [TestClass]
    public class TBHelperTest
    {
        [DataTestMethod]
        public void RegistrationTest(string login, string password, string name)
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings("192.168.0.102", 11221, new int[] { 11222, 11224, 12550 }, 3000);
            ServerProgram server = new ServerProgram("192.168.0.102", 11221, new int[] { 11222, 11224, 12550 }, 3000);
            server.Run();

            DBHelper helper = new DBHelper(settings);
            helper.Registration(login, password, name);

            Client client = new Client(settings);
            string result = client.SendCommand("clp", new string[] { login, password });

            Assert.AreEqual("True", result);
        }
    }
}
