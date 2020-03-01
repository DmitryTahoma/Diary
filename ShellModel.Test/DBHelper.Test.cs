using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientCore;
using ServerRealization;
using System;
using System.Collections.Generic;
using ShellModel.Context;
using System.Threading;

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

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", true)]
        [DataRow("Alex92", "pass12345", false)]
        [DataRow("Alex926", "pass1234", false)]
        public void SignInTest(string login, string password, bool expectedResult)
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings("192.168.0.105", 11221, new int[] { 11222, 11224, 12550 }, 3000);
            ServerProgram server = new ServerProgram("192.168.0.105", 11221, new int[] { 11222, 11224, 12550 }, 3000);
            server.Run();

            DBHelper helper = new DBHelper(settings);
            bool isHappened = helper.SignIn(login, password);
            Assert.AreEqual(expectedResult, isHappened);

            Client client = new Client(settings);
            string result = client.SendCommand("clp", new string[] { login, password });

            Assert.AreEqual(result, isHappened.ToString());
        }

        [TestMethod]
        public void ActionsIsLockedTest()
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings("192.168.0.105", 11221, new int[] { 11222, 11224, 12550 }, 3000);
            ServerProgram server = new ServerProgram("192.168.0.105", 11221, new int[] { 11222, 11224, 12550 }, 3000);
            server.Run();

            DBHelper helper = new DBHelper(settings);
            DateTime before = DateTime.Now;
            helper.Registration("Temp", "TempTemp", "Temp");
            helper.SignIn("Temp", "Temp");
            DateTime after = DateTime.Now;

            Assert.IsTrue((after - before).TotalMilliseconds > 200);
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", 1, 3, 2020)]
        [DataRow("Tahoma", "password", 27, 2, 2020)]
        [DataRow("", "pass1234", 1, 3, 2020)]
        [DataRow("Alex92", "", 1, 3, 2020)]
        [DataRow("Alex92", "pass1234", 1000, 3, 2020)]
        [DataRow("Alex92", "pass1234", 1, 32, 2020)]
        [DataRow("Alex92", "pass1234", 1, 3, -1)]
        public void GetDayTest(string login, string password, int day, int month, int year)
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings("192.168.0.107", 11221, new int[] { 11222 }, 3000);
            ServerProgram server = new ServerProgram(settings);
            server.ExecuteCommand("generate1000notes", new string[] { });
            server.Run();
            string expectedResultString = server.ExecuteCommand("gd", new string[] { login, password, day.ToString(), month.ToString(), year.ToString() });

            DBHelper helper = new DBHelper(settings);
            try
            {
                List<Note> result = helper.GetDay(login, password, day, month, year);

                List<Note> expectedResult = new List<Note>();
                string[] erss = StringsHelper.Split("\b<sgd>\b", expectedResultString);
                for (int i = 0; i < erss.Length; ++i)
                {
                    if (Mission.IsStringMission(erss[i]))
                        expectedResult.Add((ParagraphMission)Mission.CreateNew(erss[i]));
                    else
                        expectedResult.Add(new Note(erss[i]));
                }

                Assert.AreEqual(expectedResult.Count, result.Count);
                for(int i = 0; i < result.Count; ++i)
                {
                    if(expectedResult[i] is ParagraphMission)
                    {
                        Assert.IsTrue(result[i] is ParagraphMission);
                        ParagraphMission expectedMission = (ParagraphMission)expectedResult[i],
                            mission = (ParagraphMission)result[i];

                        Assert.AreEqual(expectedMission.Context.Count, mission.Context.Count);
                        Assert.AreEqual(expectedMission.ContextId, mission.ContextId);
                        for(int j = 0; j < mission.Context.Count; ++j)
                        {
                            Assert.AreEqual(expectedMission.Paragraph.Items[j].Id, mission.Paragraph.Items[j].Id);
                            Assert.AreEqual(expectedMission.Paragraph.Items[j].Text, mission.Paragraph.Items[j].Text);
                            Assert.AreEqual(expectedMission.Paragraph.Items[j].IsChecked, mission.Paragraph.Items[j].IsChecked);
                        }

                        Assert.AreEqual(expectedMission.Id, mission.Id);
                        Assert.AreEqual(expectedMission.ActionId, mission.ActionId);
                        Assert.AreEqual(expectedMission.Start, expectedMission.Start);
                        Assert.AreEqual(expectedMission.End, expectedMission.End);
                    }
                    Assert.AreEqual(expectedResult[i].Id, result[i].Id);
                    Assert.AreEqual(expectedResult[i].Name, result[i].Name);
                    Assert.AreEqual(expectedResult[i].Text, result[i].Text);
                    Assert.AreEqual(expectedResult[i].LastChanged, result[i].LastChanged);
                    Assert.AreEqual(expectedResult[i].Created, result[i].Created);
                    Assert.AreEqual(expectedResult[i].StereotypeId, result[i].StereotypeId);
                }
            }
            catch(ArgumentException)
            {
                Assert.IsTrue(expectedResultString == "ae");
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
