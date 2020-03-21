using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization;
using ServerRealization.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class ParagraphMissionTest
    {
        [DataTestMethod]
        [DataRow(0, 1, 2, 3, 4, "Name", "Text", new int[] { 1, 2, 3 }, new string[] { "Lorem ipsum dolor", "sit amet, consectetur", "adipiscing elit, sed do eiusmod" }, new bool[] { true, true, false })]
        [DataRow(10, 21, 32, 443, 53524, "gdsshdfh", "dgsdhsdfg", new int[] { 235, 47, 75, 5763 }, new string[] { "tempor incididunt ut labore", "et dolore magna aliqua. Ut enim", "ad minim veniam, quis nostrud exercitation", "ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." }, new bool[] { false, true, true, false })]
        public void InitializationTest(int id, int cid, int aid, int nid, int sid, string name, string text, int[] ids, string[] items, bool[] isCheckeds)
        {
            ParagraphMission mission = new ParagraphMission(id, cid, aid, nid, sid, name, text, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            Assert.AreEqual(id, mission.Id);
            Assert.AreEqual(cid, mission.ContextId);
            Assert.AreEqual(aid, mission.ActionId);
            Assert.AreEqual(nid, mission.NoteId);
            Assert.AreEqual(sid, mission.StereotypeId);
            Assert.AreEqual(name, mission.Name);
            Assert.AreEqual(text, mission.Text);
            Assert.IsTrue(mission is IMission && mission is Mission && mission.Type == MissionType.Paragraph);

            List<Point> points = new List<Point>();
            for(int i = 0; i < items.Length; ++i)
            {
                points.Add(new Point(ids[i], items[i], isCheckeds[i]));
            }
            Paragraph paragraph = new Paragraph(cid, points);
            ParagraphMission mission2 = new ParagraphMission(id, paragraph, aid, nid, sid, name, text, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);

            Assert.AreEqual(items.Length, mission2.Context.Count);
            Assert.AreEqual(paragraph, mission2.Context);
            Assert.AreEqual(paragraph, mission2.Paragraph);

            for(int i = 0; i < items.Length; ++i)
            {
                Assert.AreEqual(ids[i], ((Paragraph)mission2.Context).Items[i].Id);
                Assert.AreEqual(items[i], ((Paragraph)mission2.Context).Items[i].Text);
                Assert.AreEqual(isCheckeds[i], ((Paragraph)mission2.Context).Items[i].IsChecked);
            }
        }

        [DataTestMethod]
        [DataRow(-1, 1)]
        [DataRow(0, 2)]
        [DataRow(1, 3)]
        [DataRow(4, 5)]
        public void IdSetTest(int id, int newId)
        {
            ParagraphMission mission = new ParagraphMission(id, new Paragraph(), 0, 0, 0, "", "", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            mission.Id = newId;
            Assert.AreEqual(id > 0 ? id : newId, mission.Id);
        }

        [DataTestMethod]
        [DataRow("Name", "Text", 1, 1, 2020)]
        [DataRow("farfghsdfsdgh", "shsdfgafhasdfgh", 10, 6, 1999)]
        public void AutoCreatingTest(string name, string text, int day, int month, int year)
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings("192.168.0.107", 100, new int[] { 200, 300, 400 }, 300);
            ServerProgram server = new ServerProgram(settings);
            server.Run();
            DBHelper helper = new DBHelper(settings);
            helper.Registration("Login", "Password", "Name");
            DBHelper.Login = "Login";
            DBHelper.Password = "Password";

            ParagraphMission mission = new ParagraphMission(name, text, new DateTime(year, month, day));
            Assert.IsTrue(mission.Id < 1);

            mission = new ParagraphMission(name, text, new DateTime(year, month, day), true);
            Thread.Sleep(500);
            Assert.IsTrue(mission.Id > 0);
            Assert.IsTrue(DBContext.Missions.Where(x => x.Id == mission.Id).Count() == 1);
            ServerRealization.Database.Context.Mission dbMission = DBContext.Missions.Where(x => x.Id == mission.Id).First();

            Assert.AreEqual(mission.ActionId, dbMission.ActionId);
            Assert.AreEqual(mission.NoteId, dbMission.Action.NoteId);
            Assert.AreEqual(mission.Paragraph.Id, dbMission.ContextId);
            Assert.AreEqual(mission.Name, dbMission.Action.Note.Name);
            Assert.AreEqual(mission.Text, dbMission.Action.Note.Text);
            Assert.AreEqual(mission.Created, dbMission.Action.Note.Created);
        }
    }
}