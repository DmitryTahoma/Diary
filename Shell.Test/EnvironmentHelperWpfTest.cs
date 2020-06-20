using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace ShellTest
{
    [TestClass]
    public class EnvironmentHelperWpfTest
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

        [TestMethod]
        public void CheckSignDataTest()
        {
            ShellModel.IEnvironmentHelper environmentHelper = new Shell.Models.EnvironmentHelperWpf();
            environmentHelper.SaveSignData("Login", "Password");

            Assert.IsTrue(environmentHelper.CheckSignData());

            RegistryKey registryKey = Registry.CurrentUser;
            RegistryKey dehd = registryKey.CreateSubKey("DiaryEHData", true);

            dehd.SetValue("time", DateTime.Now.AddDays(-29).ToString());
            Assert.IsTrue(environmentHelper.CheckSignData());

            dehd.SetValue("time", DateTime.Now.AddDays(-30.1).ToString());
            Assert.IsFalse(environmentHelper.CheckSignData());

            dehd.SetValue("time", DateTime.Now.AddHours(0.5).ToString());
            Assert.IsFalse(environmentHelper.CheckSignData());

            dehd.SetValue("time", DateTime.Now.AddDays(2).ToString());
            Assert.IsFalse(environmentHelper.CheckSignData());

            dehd.SetValue("login", "");
            dehd.SetValue("password", "");
            dehd.SetValue("time", DateTime.Now.ToString());
            Assert.IsFalse(environmentHelper.CheckSignData());
        }

        [DataTestMethod]
        [DataRow("Login", "Password")]
        [DataRow("dghshfasf", "agsdgadghassf254Sf-f")]
        public void GetSignDataTest(string login, string password)
        {
            ShellModel.IEnvironmentHelper environmentHelper = new Shell.Models.EnvironmentHelperWpf();
            environmentHelper.SaveSignData(login, password);

            KeyValuePair<string, string> signData = environmentHelper.GetSignData();
            Assert.AreEqual(login, signData.Key);
            Assert.AreEqual(password, signData.Value);
        }
    }
}