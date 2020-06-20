using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientCore;
using ServerRealization;
using System;
using System.Collections.Generic;
using ShellModel.Context;
using System.Threading;
using ServerRealization.Database;
using System.Linq;

namespace ShellModel.Test
{
    [TestClass]
    public class DBHelperTest
    {
        SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings("192.168.0.107", 11221, 1000);
        ServerProgram server = null;
        DBHelper helper = null;

        [TestInitialize]
        public void TestInitialize()
        {
            server = new ServerProgram(settings);
            server.Run();
            server.ExecuteCommand("rnu", new string[] { "Login", "Password", "Name" });
            helper = new DBHelper(settings);
            DBHelper.Login = "Login";
            DBHelper.Password = "Password";
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Thread.Sleep(server.Stop() + 333);
            server = null;
            helper = null;
        }

        public void GenerateTestNotes() => server.ExecuteCommand("generate1000notes", new string[] { });

        [DataTestMethod]
        [DataRow("templogin1", "password", "Dmitry")]
        [DataRow("mylogin", "herhdfhsdf22", "Tahoma")]
        public void RegistrationTest(string login, string password, string name)
        {
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
            bool isHappened = helper.SignIn(login, password);
            Assert.AreEqual(expectedResult, isHappened);

            Client client = new Client(settings);
            string result = client.SendCommand("clp", new string[] { login, password });

            Assert.AreEqual(result, isHappened.ToString());
        }

        [TestMethod]
        public void ActionsIsLockedTest()
        {
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
            GenerateTestNotes();
            string expectedResultString = server.ExecuteCommand("gd", new string[] { login, password, day.ToString(), month.ToString(), year.ToString() });

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

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", 1, 3, 2020)]
        [DataRow("Tahoma", "password", 27, 2, 2020)]
        [DataRow("", "pass1234", 1, 3, 2020)]
        [DataRow("Alex92", "", 1, 3, 2020)]
        [DataRow("Alex92", "pass1234", 1000, 3, 2020)]
        [DataRow("Alex92", "pass1234", 1, 32, 2020)]
        [DataRow("Alex92", "pass1234", 1, 3, -1)]
        public void GetDayAsyncTest(string login, string password, int day, int month, int year)
        {
            GenerateTestNotes();
            List<Note> expectedResult = null;
            try { expectedResult = helper.GetDay(login, password, day, month, year); }
            catch (ArgumentException) { expectedResult = new List<Note>(); }

            List<Note> result = null;
            try { result = DBHelper.GetDayAsync(login, password, day, month, year).Result; }
            catch (ArgumentException) { result = new List<Note>(); }
            Assert.AreEqual(expectedResult.Count, result.Count);
            for (int i = 0; i < result.Count; ++i)
            {
                if (expectedResult[i] is ParagraphMission)
                {
                    Assert.IsTrue(result[i] is ParagraphMission);
                    ParagraphMission expectedMission = (ParagraphMission)expectedResult[i],
                        mission = (ParagraphMission)result[i];

                    Assert.AreEqual(expectedMission.Context.Count, mission.Context.Count);
                    Assert.AreEqual(expectedMission.ContextId, mission.ContextId);
                    for (int j = 0; j < mission.Context.Count; ++j)
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

        [DataTestMethod]
        [DataRow(new string[] { "Alex92", "gsdhasf" }, new string[] { "pass1234", "fgsdghegggg" }, new bool[] { true, false })]
        [DataRow(new string[] { "Tahoma", "Tahoma", "Tahoma", "Alex92", "Alex93" }, new string[] { "Tahoma", "password", "gsdgsdasfhdf", "pass1234", "pass1234" }, new bool[] { false, true, false, true, false })]
        [DataRow(new string[] { "fsdgdfhdfh", "sdtyjnbsd54", "3w6tiyjsdht", "ety6ehrstjhdft", "gdeuse46tedtujk", "yedrikdrtyg", "sykdhrydrtsn", "sdtyjnbsd54", "jrdtgwfsvtyrss46", "hjsrd5gyawft" }, new string[] { "", "3w6tiyjsdht", "fsdgdfhdfh", "43e7syse4cye4", "e4s7yacsetbe", "s56u8nsevtesueys5", "idsr5vsf", "se5uynsegv4y", "a4hbatyerstv", "awegb6a4yct" }, new bool[] { false, false, false, false, false, false, false, false, false, false })]
        public void QueueTest(string[] logins, string[] passwords, bool[] results)
        {
            Random random = new Random();
            List<Thread> threads = new List<Thread>();
            List<Exception> errors = new List<Exception>();
            for (int j = 0, t = random.Next(5, 10); j < t; ++j)
                threads.Add(new Thread(() => {
                    for (int i = 0; i < logins.Length; ++i)
                        try
                        {
                            Assert.AreEqual(results[i], helper.SignIn(logins[i], passwords[i]));
                        }
                        catch(Exception e) { errors.Add(e); }
                    }));                

            foreach (Thread thread in threads)
                thread.Start();
            Thread.Sleep(10000);
            
            if(errors.Count != 0)
                Assert.Fail(errors.Count.ToString() + "errors thrown");
        }

        [DataTestMethod]
        [DataRow("name", "text")]
        [DataRow("create", "notetest")]
        public void CreateNoteTest(string name, string text)
        {
            Note note = new Note(-1, name, text, DateTime.Now, DateTime.Now);
            int id = helper.CreateNote(note);
            Thread.Sleep(500);

            ServerRealization.Database.Context.Note dbNote = DBContext.Notes.Where(x => x.Id == id).First();
            Assert.AreEqual(name, dbNote.Name);
            Assert.AreEqual(text, dbNote.Text);
        }

        [DataTestMethod]
        [DataRow("name", "text", "name", "texttext")]
        [DataRow("name", "texttext", "name", "text")]
        [DataRow("name", "text", "name", "teeext")]
        [DataRow("name", "text", "new name", "text")]
        [DataRow("old name", "text is string", "new name", "text is new")]
        public void SaveChangesTest(string name, string text, string newName, string newText)
        {
            Note note = new Note(-1, name, text, DateTime.Now, DateTime.Now);
            int id = helper.CreateNote(note);
            Thread.Sleep(500);

            List<KeyValuePair<string, string[]>> changes = Note.GetChanges(new Note(id, -1, newName, newText, DateTime.Now, DateTime.Now), note);
            Assert.IsTrue(helper.SaveChanges(changes));
            Thread.Sleep(500);

            ServerRealization.Database.Context.Note dbNote = DBContext.Notes.Where(x => x.Id == id).First();
            Assert.AreEqual(newName, dbNote.Name);
            Assert.AreEqual(newText, dbNote.Text);

            server.Stop();
            Thread.Sleep(1000);
        }

        [DataTestMethod]
        [DataRow("Name", "Text")]
        [DataRow("old name", "text is string")]
        public void RemoveNoteCascadeTest(string name, string text)
        {
            Note note = new Note(name, text, true);
            int id = note.Id;
            Assert.IsTrue(helper.RemoveNoteCascade(note));
            Assert.IsTrue(DBContext.Notes.Where(x => x.Id == id).Count() == 0);
        }

        [DataTestMethod]
        [DataRow("", "", 1, 1, 2020)]
        [DataRow("Name", "", 2, 1, 2020)]
        [DataRow("", "Text", 1, 3, 2020)]
        [DataRow("Name", "Text", 5, 1, 2020)]
        public void CreateNewParagraphMissionTest(string name, string text, int day, int month, int year)
        {
            ParagraphMission paragraphMission = new ParagraphMission(name, text, new DateTime(year, month, day));
            int[] ids = helper.CreateParagraphMission(paragraphMission);
            Thread.Sleep(500);

            Assert.IsTrue(ids.Length == 4);
            Assert.IsTrue(DBContext.Notes.Where(x => x.Id == ids[0]).Count() == 1);
            Assert.IsTrue(DBContext.Actions.Where(x => x.Id == ids[1]).Count() == 1);
            Assert.IsTrue(DBContext.Missions.Where(x => x.Id == ids[2]).Count() == 1);
            Assert.IsTrue(DBContext.Collections.Where(x => x.Id == ids[3]).Count() == 1);

            ServerRealization.Database.Context.Mission mission = DBContext.Missions.Where(x => x.Id == ids[2]).First();
            Assert.AreEqual(mission.Action.NoteId, ids[0]);
            Assert.AreEqual(mission.Action.Id, ids[1]);
            Assert.AreEqual(mission.ContextId, ids[3]);
            
            Assert.AreEqual(name, mission.Action.Note.Name);
            Assert.AreEqual(text, mission.Action.Note.Text);
            Assert.AreEqual(new DateTime(year, month, day), mission.Action.Note.Created);
        }

        [DataTestMethod]
        [DataRow(new string[] { "", "hello" })]
        [DataRow(new string[] { "lorem", "ipsum", "hello", "by", "direct" })]
        public void AddPointToParagraphMissionTest(string[] points)
        {
            ParagraphMission paragraphMission = new ParagraphMission("name", "text", DateTime.Now, true);

            for (int i = 0; i < points.Length; ++i)
            {
                int id = helper.AddPointToParagraphMission(paragraphMission, new Point(points[i], false));
                Assert.AreEqual(points[i], DBContext.Points.Where(x => x.Id == id).First().Name);

                id = DBHelper.AddPointToParagraphMissionStatic(paragraphMission, new Point(points[i], false));
                Assert.AreEqual(points[i], DBContext.Points.Where(x => x.Id == id).First().Name);
            }

            Thread.Sleep(server.Stop() * 2);
        }

        [DataTestMethod]
        [DataRow(new string[] { "1", "2", "3" }, 0)]
        [DataRow(new string[] { "sdg", "sdgsdg", "sdgsdg", "gdsgd" }, 2)]
        [DataRow(new string[] { "sdg", "sdgsdg", "sdgsdg", "gdsgd" }, 3)]
        public void RemovePointTest(string[] points, int id)
        {
            ParagraphMission paragraphMission = new ParagraphMission("", "", DateTime.Now, true);

            int dbId = -1;
            for (int i = 0; i < points.Length; ++i)
            {
                Point point = new Point(points[i], true);
                point.Id = helper.AddPointToParagraphMission(paragraphMission, point);
                paragraphMission.Paragraph.Items.Add(point);
                if (id == i)
                    dbId = point.Id;
            }

            Assert.AreEqual(1, DBContext.Points.Where(x => x.Id == dbId).Count());
            Assert.AreEqual(points.Length, paragraphMission.Paragraph.Count);
            Assert.AreEqual(points.Length, DBContext.Collections.Where(x => x.Id == paragraphMission.Paragraph.Id).First().Count);
            Assert.IsTrue(DBHelper.RemovePointStatic(paragraphMission.Paragraph.Items.Where(x => x.Id == dbId).First()));
            Assert.AreEqual(0, DBContext.Points.Where(x => x.Id == dbId).Count());
            Assert.AreEqual(points.Length - 1, DBContext.Collections.Where(x => x.Id == paragraphMission.Paragraph.Id).First().Count);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void SetCheckedPointTest(bool isChecked)
        {
            ParagraphMission mission = new ParagraphMission("", "", DateTime.Now, true);
            mission.Paragraph.AddPoint(new Point("", true));
            Assert.IsFalse(mission.Paragraph.Items.First().IsChecked);
            Assert.IsTrue(DBHelper.SetCheckedPointStatic(mission.Paragraph.Items.First(), isChecked));
            Assert.AreEqual(isChecked, DBContext.Points.Where(x => x.Id == mission.Paragraph.Items.First().Id).First().IsChecked);
            Assert.IsTrue(helper.SetCheckedPoint(mission.Paragraph.Items.First(), !isChecked));
            Assert.AreEqual(!isChecked, DBContext.Points.Where(x => x.Id == mission.Paragraph.Items.First().Id).First().IsChecked);
        }

        [DataTestMethod]
        [DataRow("Name", "Text", "13", "5", "2020", "13", "4", "2020")]
        [DataRow("Lorem", "ipsum", "1", "1", "2019", "17", "5", "2020")]
        public void DuplicateNoteTest(string name, string text, string createdDay, string createdMonth, string createdYear, string newDay, string newMonth, string newYear)
        {
            Note note = new Note(-1, -1, name, text, new DateTime(int.Parse(createdYear), int.Parse(createdMonth), int.Parse(createdDay)), DateTime.Now, true);

            DateTime before = DateTime.Now;
            Note dNote = helper.DuplicateNote(note, new DateTime(int.Parse(newYear), int.Parse(newMonth), int.Parse(newDay)));
            DateTime after = DateTime.Now;

            Assert.AreNotEqual(note.Id, dNote.Id);
            ServerRealization.Database.Context.Note dbNote = DBContext.Notes.Where(x => x.Id == note.Id).First();
            ServerRealization.Database.Context.Note DbDNote = DBContext.Notes.Where(x => x.Id == dNote.Id).First();

            Assert.AreEqual(name, dNote.Name);
            Assert.AreEqual(name, DbDNote.Name);
            Assert.AreEqual(text, dNote.Text);
            Assert.AreEqual(text, DbDNote.Text);
            Assert.AreEqual(dbNote.UserId, DbDNote.UserId);
            Assert.AreEqual(newDay, dNote.Created.Day.ToString());
            Assert.AreEqual(newDay, DbDNote.Created.Day.ToString());
            Assert.AreEqual(newMonth, dNote.Created.Month.ToString());
            Assert.AreEqual(newMonth, DbDNote.Created.Month.ToString());
            Assert.AreEqual(newYear, dNote.Created.Year.ToString());
            Assert.AreEqual(newYear, DbDNote.Created.Year.ToString());
            Assert.IsTrue(before <= dNote.LastChanged && dNote.LastChanged <= after);
            Assert.IsTrue(before <= DbDNote.LastChanged && DbDNote.LastChanged <= after);
        }

        [DataTestMethod]
        [DataRow("Name", "Text", new string[] { "first", "second" }, new bool[] { true, false }, "1", "1", "2020", "10", "2", "2020", "[id]")]
        [DataRow("qwetyuiop", "l,nbvcx", new string[] { "asdghdfjh", "sszdgsdzfhdfjh", "ssdgsdfhsdfh", "asfhsdfjsdfj", "dfzsjhdfjdfgj" }, new bool[] { true, false, true, true, false }, "15", "3", "2019", "12", "8", "2018", "[id]")]
        public void DuplicateParagraphMissionTest(string name, string text, string[] points, bool[] isCheckeds, string createdDay, string createdMonth, string createdYear, string newDay, string newMonth, string newYear, string expectedResult)
        {
            ParagraphMission paragraphMission = new ParagraphMission(name, text, DateTime.Parse(createdDay + "." + createdMonth + "." + createdYear), true);
            int[] ids = new int[] { paragraphMission.NoteId, paragraphMission.ActionId, paragraphMission.Id, paragraphMission.Paragraph.Id };
            for (int i = 0; i < points.Length; ++i)
                helper.AddPointToParagraphMission(paragraphMission, new Point(points[i], isCheckeds[i]));

            DateTime before = DateTime.Now;
            ParagraphMission dParagraphMission = helper.DuplicateParagraphMission(paragraphMission, DateTime.Parse(newDay + "." + newMonth + "." + newYear));
            DateTime after = DateTime.Now;
            int[] newIds = new int[] { dParagraphMission.NoteId, dParagraphMission.ActionId, dParagraphMission.Id, dParagraphMission.Paragraph.Id };

            ServerRealization.Database.Context.Note noteDb = DBContext.Notes.Where(x => x.Id == ids[0]).First();
            ServerRealization.Database.Context.Note newNoteDb = DBContext.Notes.Where(x => x.Id == newIds[0]).First();
            ServerRealization.Database.Context.Action actionDb = DBContext.Actions.Where(x => x.Id == ids[1]).First();
            ServerRealization.Database.Context.Action newActionDb = DBContext.Actions.Where(x => x.Id == newIds[1]).First();
            ServerRealization.Database.Context.Mission missionDb = DBContext.Missions.Where(x => x.Id == ids[2]).First();
            ServerRealization.Database.Context.Mission newMissionDb = DBContext.Missions.Where(x => x.Id == newIds[2]).First();
            ServerRealization.Database.Context.Collection paragraphDb = DBContext.Collections.Where(x => x.Id == ids[3]).First();
            ServerRealization.Database.Context.Collection newParagraphDb = DBContext.Collections.Where(x => x.Id == newIds[3]).First();
            List<ServerRealization.Database.Context.Point> pointsDb = DBContext.Points.Where(x => x.ParagraphId == ids[3]).ToList();
            List<ServerRealization.Database.Context.Point> newPointsDb = DBContext.Points.Where(x => x.ParagraphId == newIds[3]).ToList();

            Assert.AreNotEqual(noteDb.Id, newNoteDb.Id);
            Assert.AreEqual(noteDb.UserId, newNoteDb.UserId);
            Assert.AreEqual(noteDb.Name, newNoteDb.Name);
            Assert.AreEqual(noteDb.Text, newNoteDb.Text);
            Assert.AreNotEqual(noteDb.Created, newNoteDb.Created);
            Assert.AreEqual(newDay, newNoteDb.Created.Day.ToString());
            Assert.AreEqual(newMonth, newNoteDb.Created.Month.ToString());
            Assert.AreEqual(newYear, newNoteDb.Created.Year.ToString());
            Assert.IsTrue(before <= newNoteDb.LastChanged && newNoteDb.LastChanged <= after);

            Assert.AreNotEqual(actionDb.Id, newActionDb.Id);
            Assert.AreNotEqual(actionDb.NoteId, newActionDb.NoteId);
            Assert.AreEqual(newNoteDb.Id, newActionDb.NoteId);
            Assert.AreEqual(actionDb.Start, newActionDb.Start);
            Assert.AreEqual(actionDb.End, newActionDb.End);

            Assert.AreNotEqual(missionDb.Id, newMissionDb.Id);
            Assert.AreNotEqual(missionDb.ActionId, newMissionDb.ActionId);
            Assert.AreEqual(newActionDb.Id, newMissionDb.Id);
            Assert.AreEqual(false, newMissionDb.IsProgressType);
            Assert.AreNotEqual(missionDb.ContextId, newMissionDb.ContextId);
            Assert.AreEqual(newParagraphDb.Id, newMissionDb.ContextId);

            Assert.AreNotEqual(paragraphDb.Id, newParagraphDb.Id);
            Assert.AreEqual(paragraphDb.Count, newParagraphDb.Count);
            Assert.AreEqual(pointsDb.Count, newPointsDb.Count);
            for (int i = 0; i < pointsDb.Count; ++i)
            {
                Assert.AreNotEqual(pointsDb[i].Id, newPointsDb[i].Id);
                Assert.AreEqual(newParagraphDb.Id, newPointsDb[i].ParagraphId);
                Assert.AreEqual(pointsDb[i].Name, newPointsDb[i].Name);
                Assert.AreEqual(pointsDb[i].IsChecked, newPointsDb[i].IsChecked);
            }         
        }
    }
}