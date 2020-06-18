using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System;

namespace ShellTest
{
    [TestClass]
    public class ShellTest
    {
        [DataTestMethod]
        [DataRow("Login", "Password")]
        [DataRow("dghshfasf", "agsdgadghassf254Sf-f")]
        public void SaveSignDataTest(string login, string password)
        {
            DateTime before = DateTime.Now;
            ShellModel.IEnvironmentHelper environmentHelper = new Shell.Models.EnvironmentHelperWpf();
            environmentHelper.SaveSignData(login, password);
            DateTime after = DateTime.Now;

            RegistryKey registryKey = Registry.CurrentUser;
            RegistryKey dehd = registryKey.OpenSubKey("DiaryEHData", false);
            string rlogin = dehd.GetValue("login").ToString();
            string rpassword = dehd.GetValue("password").ToString();
            DateTime time = DateTime.Parse(dehd.GetValue("time").ToString());
            dehd.Close();

            Assert.AreEqual(login, rlogin);
            Assert.AreEqual(password, rpassword);
            Assert.IsTrue(before.Date <= time.Date && time.Date <= after.Date);
            Assert.IsTrue(before.Hour <= time.Hour && time.Hour <= after.Hour);
            Assert.IsTrue(before.Minute <= time.Minute && time.Minute <= after.Minute);
            Assert.IsTrue(before.Second <= time.Second && time.Second <= after.Second);
        }
    }
}