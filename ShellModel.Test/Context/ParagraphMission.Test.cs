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
        SocketSettings.ISocketSettings settings = null;
        ServerProgram server = null;

        [TestInitialize]
        public void TestInitialize()
        {
            settings = new SocketSettings.SocketSettings("192.168.0.107", 11223, new int[] { 11221, 11222 }, 10000);
            server = new ServerProgram(settings);
            server.Run();
            DBHelper helper = new DBHelper(settings);
            helper.Registration("Login", "Password", "Name");
            DBHelper.Login = "Login";
            DBHelper.Password = "Password";
        }

        [TestCleanup]
        public void TestCleanup()
        {
            settings = null;
            Thread.Sleep(server.Stop() + 400);
            server = null;
        }

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

        [DataTestMethod]
        [DataRow(new string[] { "", "hello" })]
        [DataRow(new string[] { "lorem", "ipsum", "hello", "by", "direct" })]
        public void AutoAddingPointTest(string[] points)
        {
            ParagraphMission mission = new ParagraphMission("name", "text", DateTime.Now, true);
            Thread.Sleep(500);
            ServerRealization.Database.Context.Mission dbMission = DBContext.Missions.Where(x => x.Id == mission.Id).First();

            for(int i = 0; i < points.Length; ++i)
            {
                mission.Paragraph.AddPoint(new Point(points[i], true));
                Thread.Sleep(500);
                Assert.AreEqual(i + 1, mission.Paragraph.Count);
                Assert.AreEqual(i + 1, ((ServerRealization.Database.Context.Collection)dbMission.Context).Count);
                Point point = mission.Paragraph.Items.Last();
                Assert.IsTrue(point.Id > 0);
                Assert.AreEqual(points[i], point.Text);
                Assert.AreEqual(false, point.IsChecked);
                ServerRealization.Database.Context.Point dbPoint = DBContext.Points.Where(x => x.Id == point.Id).First();
                Assert.AreEqual(points[i], dbPoint.Name);
                Assert.AreEqual(false, dbPoint.IsChecked);
            }
        }

        [DataTestMethod]
        [DataRow("name", "text", new string[] { "first", "second", "third" }, new bool[] { false, false, true }, "name", "text", new string[] { "first", "scenof", "third", }, new bool[] { false, true, true })]
        [DataRow("", "", new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" }, new bool[] { false, false, true, false, false, false, true, false, true }, "name", "text", new string[] { "first", "2", "third", "five?", "or no?", "or it is five", "I MUST", "FIX IT", "9" }, new bool[] { false, true, false, true, true, true, true, false, false })]
        public void GetChangesTest(string name, string text, string[] points, bool[] isCheckeds, string name2, string text2, string[] points2, bool[] isCheckeds2)
        {
            ParagraphMission oldPm = new ParagraphMission(name, text, DateTime.Now, true);
            ParagraphMission newPm = new ParagraphMission(name2, text2, DateTime.Now);
            for (int i = 0; i < points.Length; ++i)
            {
                Point point = new Point(points[i], true);
                oldPm.Paragraph.AddPoint(point);
                Thread.Sleep(100);
                point.IsChecked = isCheckeds[i];

                newPm.Paragraph.Items.Add(new Point(point.Id, point.Text, point.IsChecked));
            }

            for(int i = 0; i < points.Length; ++i)
            {
                newPm.Paragraph.Items[i].Text = points2[i];
                newPm.Paragraph.Items[i].IsChecked = isCheckeds2[i];
            }

            List<KeyValuePair<string, string[]>> expectedResult = new List<KeyValuePair<string, string[]>>();
            for (int i = 0; i < oldPm.Paragraph.Count; ++i)
            {
                if (newPm.Paragraph.Items
                        .Where((x) =>
                        {
                            Point p = oldPm.Paragraph.Items[i];
                            return x.Id == p.Id && x.Text != p.Text;
                        })
                        .Count() == 1)
                    expectedResult.Add(new KeyValuePair<string, string[]>("cpt", new string[] { newPm.Paragraph.Items[i].Id.ToString(), newPm.Paragraph.Items[i].Text }));
                if (newPm.Paragraph.Items
                        .Where((x) =>
                        {
                            Point p = oldPm.Paragraph.Items[i];
                            return x.Id == p.Id && x.IsChecked != p.IsChecked;
                        })
                        .Count() == 1)
                    expectedResult.Add(new KeyValuePair<string, string[]>("scp", new string[] { oldPm.Paragraph.Items[i].Id.ToString(), newPm.Paragraph.Items[i].IsChecked.ToString() }));
            }

            List<KeyValuePair<string, string[]>> result = ParagraphMission.GetChanges(newPm, oldPm);

            Assert.AreEqual(expectedResult.Count, result.Count);
            for (int i = 0; i < expectedResult.Count; ++i)
            {
                Assert.AreEqual(expectedResult[i].Key, result[i].Key);
                Assert.AreEqual(expectedResult[i].Value.Length, result[i].Value.Length);
                for (int j = 0; j < expectedResult[i].Value.Length; ++j)
                    Assert.AreEqual(expectedResult[i].Value[j], result[i].Value[j]);
            }
        }

        [DataTestMethod]
        [DataRow(new string[] { "1", "2", "3" }, 0)]
        [DataRow(new string[] { "sdg", "sdgsdg", "sdgsdg", "gdsgd" }, 2)]
        [DataRow(new string[] { "sdg", "sdgsdg", "sdgsdg", "gdsgd" }, 3)]
        public void AutoRemovingPointTest(string[] points, int id)
        {
            ParagraphMission mission = new ParagraphMission("name", "text", DateTime.Now, true);
            ServerRealization.Database.Context.Mission dbMission = DBContext.Missions.Where(x => x.Id == mission.Id).First();

            int dbId = -1;
            for (int i = 0; i < points.Length; ++i)
            {
                mission.Paragraph.AddPoint(new Point(points[i], true));
                if (id == i)
                    dbId = mission.Paragraph.Items.Last().Id;
            }

            Assert.AreEqual(1, DBContext.Points.Where(x => x.Id == dbId).Count());
            Assert.AreEqual(points.Length, mission.Paragraph.Count);
            Assert.AreEqual(points.Length, DBContext.Collections.Where(x => x.Id == mission.Paragraph.Id).First().Count);
            mission.Paragraph.RemovePoint(dbId);
            Assert.AreEqual(0, DBContext.Points.Where(x => x.Id == dbId).Count());
            Assert.AreEqual(points.Length - 1, mission.Paragraph.Count);
            Assert.AreEqual(points.Length - 1, DBContext.Collections.Where(x => x.Id == mission.Paragraph.Id).First().Count);
        }

        [DataTestMethod]
        [DataRow("Name", "Text", 1, 1, 2020, new string[] { "point1", "point2", "point3" }, new bool[] { false, false, true }, "Name", "TextText", new string[] { "point", "hello", "point" }, new bool[] { false, true, true })]
        [DataRow("Name2", "Lorem", 10, 12, 2019, new string[] { "setgs", "ssdf", "sdfhgdfh", "sdfhdfh", "dfhdfh" }, new bool[] { false, false, false, false, false }, "Name", "Lores impmum", new string[] { "asdgasd", "asdgh", "asdg", "sdgsda", "gdsgsd" }, new bool[] { true, true, false, false, true })]
        [DataRow("", "", 19, 4, 2020, new string[] { }, new bool[] { }, "", "", new string[] { }, new bool[] { })]
        public void AutoTimingTest(string name, string text, int day, int month, int year, string[] points, bool[] isCheckeds,
            string newName, string newText, string[] newPoints, bool[] newIsCheckeds)
        {
            ParagraphMission mission = new ParagraphMission(name, text, new DateTime(year, month, day), true);
            for (int i = 0; i < points.Length; ++i)
                mission.Paragraph.AddPoint(new Point(points[i], isCheckeds[i], true));

            ServerRealization.Database.Context.Mission dbMission = DBContext.Missions.Where(x => x.Id == mission.Id).First();
            Assert.AreEqual(mission.Name, dbMission.Action.Note.Name);
            Assert.AreEqual(mission.Text, dbMission.Action.Note.Text);
            Assert.AreEqual(mission.Created, dbMission.Action.Note.Created);
            List<Point> paragraph = mission.Paragraph.Items;
            List<ServerRealization.Database.Context.Point> dbParagraph = DBContext.Points
                .Where(y => y.ParagraphId ==
                    DBContext.Collections
                    .Where(x => x.Id == mission.Paragraph.Id)
                    .First().Id)
                .ToList();
            for (int i = 0; i < points.Length; ++i)
            {
                Point point = paragraph[i];
                ServerRealization.Database.Context.Point dbPoint = dbParagraph.Where(x => x.Id == point.Id).First();
                Assert.AreEqual(point.Text, dbPoint.Name);
                Assert.AreEqual(point.IsChecked, dbPoint.IsChecked);
            }

            mission.Name = newName;
            mission.Text = newText;
            for (int i = 0; i < points.Length; ++i)
            {
                mission.Paragraph.Items[i].Text = newPoints[i];
                mission.Paragraph.Items[i].IsChecked = newIsCheckeds[i];
            }

            Thread.Sleep(11000);

            dbMission = DBContext.Missions.Where(x => x.Id == mission.Id).First();
            Assert.AreEqual(mission.Name, dbMission.Action.Note.Name);
            Assert.AreEqual(mission.Text, dbMission.Action.Note.Text);
            Assert.AreEqual(mission.Created, dbMission.Action.Note.Created);
            paragraph = mission.Paragraph.Items;
            dbParagraph = DBContext.Points
                .Where(y => y.ParagraphId ==
                    DBContext.Collections
                    .Where(x => x.Id == mission.Paragraph.Id)
                    .First().Id)
                .ToList();
            for (int i = 0; i < points.Length; ++i)
            {
                Point point = paragraph[i];
                ServerRealization.Database.Context.Point dbPoint = dbParagraph.Where(x => x.Id == point.Id).First();
                Assert.AreEqual(point.Text, dbPoint.Name);
                Assert.AreEqual(point.IsChecked, dbPoint.IsChecked);
            }
        }

        [DataTestMethod]
        [DataRow(1000)]
        [DataRow(5000)]
        [DataRow(10000)]
        [DataRow(20000)]
        public void SetIntervalTimingTest(int mls)
        {
            ParagraphMission mission = new ParagraphMission("", "", DateTime.Now, true);
            ServerRealization.Database.Context.Mission dbMission = DBContext.Missions.Where(x => x.Id == mission.Id).First();
            mission.SetIntervalTiming(mls);
            DateTime start = DateTime.Now;
            mission.Name = "test";
            while (mission.Name != dbMission.Action.Note.Name)
                Thread.Sleep(100);
            DateTime end = DateTime.Now;
            Assert.IsTrue(mls > (int)(end - start).TotalMilliseconds - 150);
            Assert.IsTrue(mls < (int)(end - start).TotalMilliseconds + 150);
        }
    }
}