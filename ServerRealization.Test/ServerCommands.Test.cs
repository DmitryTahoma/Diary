using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerRealization.Test
{
    [TestClass]
    public partial class ServerCommandsTest
    {
        [DataTestMethod]
        [DataRow("clp", new string[] { }, "ae")]
        [DataRow("clp", new string[] { "", "", "", "" }, "ae")]
        [DataRow("clp", new string[] { "" }, "ae")]
        [DataRow("clp", new string[] { "Alex92" }, "ae")]
        [DataRow("clp", new string[] { "Alex", "92" }, "False")]
        [DataRow("clp", new string[] { "Alex92", "pass1234" }, "True")]
        [DataRow("clp", new string[] { "Alex92", "pass1234", "hello" }, "True")]
        [DataRow("clp", new string[] { "", "Alex92", "pass1234", "" }, "ae")]
        [DataRow("cnn", new string[] { "Alex92", "pass1234", "", "" }, "ae")]
        [DataRow("cnn", new string[] { "", "", "", "TextOfNote" }, "ae")]
        [DataRow("cnn", new string[] { "Alex93", "pass1234", "NameOfNote", "TextOfNote" }, "False")]
        public void SimpleTest(string commandName, string[] args, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            string result = server.ExecuteCommand(commandName, args);

            Assert.AreEqual(expectedResult, result);
        }
        
        [DataTestMethod]
        [DataRow(new string[] { }, "ae")]
        [DataRow(new string[] { "" }, "ae")]
        [DataRow(new string[] { "", "", "" }, "ae")]
        [DataRow(new string[] { "User", "Password" }, "ae")]
        [DataRow(new string[] { "", "Password", "Name" }, "ae")]
        [DataRow(new string[] { "User", "", "Name" }, "ae")]
        [DataRow(new string[] { "User", "Password", "" }, "ae")]
        [DataRow(new string[] { "User", "Password", "Name" }, "True")]
        public void RegisterNewUserTest(string[] args, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            if (args != null)
                if (args.Length != 0)
                    if(args[0] != "")
                        args[0] += DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

            DateTime before = DateTime.Now;

            string result = server.ExecuteCommand("rnu", args);

            DateTime after = DateTime.Now;

            Assert.AreEqual(expectedResult, result);

            if(expectedResult == "True")
            {
                result = server.ExecuteCommand("clp", args);
                Assert.AreEqual(expectedResult, result);

                User registeredUser = DBContext.Users.Where(x => x.Id == DBContext.Users.Max(y => y.Id)).First();
                Assert.AreEqual(args[0], registeredUser.Login);
                Assert.AreEqual(args[1], registeredUser.Password);
                Assert.AreEqual(args[2], registeredUser.Name);
                Assert.IsTrue(before <= registeredUser.Registration && registeredUser.Registration <= after);
            }
        }
        
        [TestMethod]
        public void UniqueCheckerTest()
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);

            string result = server.ExecuteCommand("rnu", new string[] { 
                DBContext.Users.Where(x => x.Id == DBContext.Users.Max(y => y.Id)).First().Login,
                "password", "name"
            });

            Assert.AreEqual("uc", result);
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", "Name", "Text", "id")]
        [DataRow("", "pass1234", "Name", "Text", "ae")]
        [DataRow("Alex92", "", "Name", "Text", "ae")]
        [DataRow("Alex92", "pass1234", "", "Text", "id")]
        [DataRow("Alex92", "pass1234", "Name", "", "id")]
        [DataRow("Alex93", "pass1234", "Name", "Text", "False")]
        [DataRow("Alex92", "pass12345", "Name", "", "False")]
        [DataRow("Alex934", "pass12345", "Name", "Text", "False")]
        public void CreateNewNoteTest(string login, string password, string name, string text, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            DateTime before = DateTime.Now;
            string result = server.ExecuteCommand("cnn", new string[] { login, password, name, text });
            DateTime after = DateTime.Now;

            if(expectedResult != "id")
                Assert.AreEqual(expectedResult, result);
            else if (int.TryParse(result, out int id))
            {
                Assert.IsTrue(id > 0);

                Assert.IsTrue(DBContext.Notes.Where(x => x.Id == id).Count() != 0);
                Note createdNote = DBContext.Notes.Where(x => x.Id == id).First();

                Assert.AreEqual(DBContext.Users
                    .Where(x => x.Login == login && x.Password == password)
                    .First().Id, createdNote.UserId);
                Assert.IsTrue(createdNote.Created.Equals(createdNote.LastChanged)
                    && createdNote.Created >= before
                    && createdNote.Created <= after);
                Assert.AreEqual(name, createdNote.Name);
                Assert.AreEqual(text, createdNote.Text);
            }
            else
                Assert.Fail();
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", "Name", "Text", 1, 2, 2020, "id")]
        [DataRow("", "pass1234", "Name", "Text", 1, 3, 2020, "ae")]
        [DataRow("Alex92", "", "Name", "Text", 1, 1, 2020, "ae")]
        [DataRow("Alex92", "pass1234", "", "Text", 2, 1, 2020, "id")]
        [DataRow("Alex92", "pass1234", "Name", "", 10, 10, 2020, "id")]
        [DataRow("Alex93", "pass1234", "Name", "Text", 1, 1, 2020, "False")]
        [DataRow("Alex92", "pass12345", "Name", "", 1, 1, 2020, "False")]
        [DataRow("Alex934", "pass12345", "Name", "Text", 1, 1, 2020, "False")]
        [DataRow("Alex92", "pass1234", "", "", 19, 11, 2020, "id")]
        [DataRow("Alex92", "pass1234", "", "", 68, 10, 2020, "id")]
        [DataRow("Alex92", "pass1234", "", "", -68, 10, 2020, "id")]
        [DataRow("Alex92", "pass1234", "", "", 10, 13, 2020, "id")]
        [DataRow("Alex92", "pass1234", "", "", 10, -13, 2020, "id")]
        [DataRow("Alex92", "pass1234", "", "", 10, 10, -700, "id")]
        public void CreateNewNoteTest2(string login, string password, string name, string text, int day, int month, int year, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.107", 11221, new int[] { 11222 }, 300);
            DateTime before = DateTime.Now;
            string result = server.ExecuteCommand("cnn", new string[] { login, password, name, text, day.ToString(), month.ToString(), year.ToString() });
            DateTime after = DateTime.Now;

            if (expectedResult != "id")
                Assert.AreEqual(expectedResult, result);
            else if (int.TryParse(result, out int id))
            {
                Assert.IsTrue(id > 0);
                Note createdNote = DBContext.Notes.Where(x => x.Id == id).First();

                Assert.AreEqual(DBContext.Users
                    .Where(x => x.Login == login && x.Password == password)
                    .First().Id, createdNote.UserId);
                Assert.IsTrue(createdNote.LastChanged >= before
                    && createdNote.LastChanged <= after);

                bool isCreatableDateTime = false;
                try
                {
                    DateTime _ = new DateTime(year, month, day);
                    isCreatableDateTime = true;
                }
                catch(ArgumentOutOfRangeException) { }

                Assert.IsTrue(isCreatableDateTime ? createdNote.Created == new DateTime(year, month, day)
                    : createdNote.Created >= before && createdNote.Created <= after);

                Assert.AreEqual(name, createdNote.Name);
                Assert.AreEqual(text, createdNote.Text);
            }
            else
                Assert.Fail();
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", "Name", "Hello, ", "world", "True")]
        [DataRow("", "pass1234", "Name", "Hello, ", "world", "ae")]
        [DataRow("Alex92", "", "Name", "Hello, ", "world", "ae")]
        [DataRow("Alex92", "pass1234", "Name", "Hello, wor", "ld", "True")]
        [DataRow("Alex93", "pass1234", "Name", "Hello, wor", "ld", "False")]
        [DataRow("Alex92", "pass12345", "Name", "Hello, wor", "ld", "False")]
        [DataRow("Alex93", "pass12345", "Name", "Hello, wor", "ld", "False")]
        public void AddTextToNoteTest(string login, string password, string name, string text, string addedText, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            string result = server.ExecuteCommand("cnn", new string[] { correctLogin, correctPassword, name, text });

            Assert.IsTrue(int.TryParse(result, out int id));
            result = server.ExecuteCommand("attn", new string[] { login, password, result, addedText });
            Assert.AreEqual(expectedResult, result);
            if(expectedResult == "True")
                Assert.AreEqual(text + addedText, DBContext.Notes
                    .Where(x => x.Id == id)
                    .First().Text);
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", "Name", "Hello, world!123", 3, "True")]
        [DataRow("Alex92", "pass1234", "Name", "Hello, world!123", 10, "True")]
        [DataRow("Alex92", "pass1234", "Name", "Hello, world!123", 13, "True")]
        [DataRow("", "pass1234", "Name", "Hello, world!123", 3, "ae")]
        [DataRow("Alex92", "", "Name", "Hello, world!123", 3, "ae")]
        [DataRow("Alex92", "pass1234", "Name", "Hello, world!123", 0, "ae")]
        [DataRow("Alex92", "pass1234", "Name", "Hello, world!123", 30, "ae")]
        [DataRow("Alex93", "pass1234", "Name", "Hello, world!123", 3, "False")]
        [DataRow("Alex92", "pass12345", "Name", "Hello, world!123", 3, "False")]
        [DataRow("Alex923", "pass12344", "Name", "Hello, world!123", 3, "False")]
        public void RemoveTextFromNoteTest(string login, string password, string name, string text, int removeCount, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            string result = server.ExecuteCommand("cnn", new string[] { correctLogin, correctPassword, name, text });

            Assert.IsTrue(int.TryParse(result, out int id));
            result = server.ExecuteCommand("rtfn", new string[] { login, password, result, removeCount.ToString() });
            Assert.AreEqual(expectedResult, result);
            if (expectedResult == "True")
                Assert.AreEqual(text.Substring(0, text.Length - removeCount), DBContext.Notes
                    .Where(x => x.Id == id)
                    .First().Text);
        }

        [DataTestMethod]
        [DataRow("Alex92",  "pass1234",  "Name", "Hello, wordl!", 3,  "ld!", "True")]
        [DataRow("Alex92",  "pass1234",  "Name", "Hello, wordl!", 8,  "ld!", "True")]
        [DataRow("Alex92",  "pass1234",  "Name", "Hello, wordl!", 10, "ld!", "True")]
        [DataRow("",        "pass1234",  "Name", "Hello, wordl!", 3,  "ld!", "ae")]
        [DataRow("Alex92",  "",          "Name", "Hello, wordl!", 3,  "ld!", "ae")]
        [DataRow("Alex92",  "pass1234",  "Name", "Hello, wordl!", 0,  "ld!", "ae")]
        [DataRow("Alex92",  "pass1234",  "Name", "Hello, wordl!", -3, "ld!", "ae")]
        [DataRow("Alex92",  "pass1234",  "Name", "Hello, wordl!", 28, "ld!", "ae")]
        [DataRow("Alex932", "pass1234",  "Name", "Hello, wordl!", 3,  "ld!", "False")]
        [DataRow("Alex92",  "pass12334", "Name", "Hello, wordl!", 3,  "ld!", "False")]
        [DataRow("Alex392", "pass13234", "Name", "Hello, wordl!", 3,  "ld!", "False")]
        public void InsertTextToNoteTest(string login, string password, string name, string text, int removeCount, string addedText, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            string result = server.ExecuteCommand("cnn", new string[] { correctLogin, correctPassword, name, text });

            Assert.IsTrue(int.TryParse(result, out int id));
            result = server.ExecuteCommand("ittn", new string[] { login, password, result, removeCount.ToString(), addedText });
            Assert.AreEqual(expectedResult, result);
            if (expectedResult == "True")
                Assert.AreEqual(text.Substring(0, text.Length - removeCount) + addedText, DBContext.Notes
                    .Where(x => x.Id == id)
                    .First().Text);
        }

        [DataTestMethod]
        [DataRow("Name", "Text", "NewText", 5, 3, "wYork")]
        [DataRow("Some name", "Done projetc", "ccccccc", 4, 5, "ct")]
        public void ChangeNoteByUserTest(string name, string text, string addText, int removeCount, int insertCount, string instertText)
        {
            string correctLogin2 = "Tahoma", correctPassword2 = "password";
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            string result = server.ExecuteCommand("cnn", new string[] { correctLogin, correctPassword, name, text });
            Assert.IsTrue(int.TryParse(result, out int id));

            result = server.ExecuteCommand("attn", new string[] { correctLogin, correctPassword, result, addText });
            Assert.AreNotEqual("ane", result);
            result = server.ExecuteCommand("attn", new string[] { correctLogin2, correctPassword2, id.ToString(), addText });
            Assert.AreEqual("ane", result);

            result = server.ExecuteCommand("rtfn", new string[] { correctLogin, correctPassword, id.ToString(), removeCount.ToString() });
            Assert.AreNotEqual("ane", result);
            result = server.ExecuteCommand("rtfn", new string[] { correctLogin2, correctPassword2, id.ToString(), removeCount.ToString() });
            Assert.AreEqual("ane", result);

            result = server.ExecuteCommand("ittn", new string[] { correctLogin, correctPassword, id.ToString(), insertCount.ToString(), instertText });
            Assert.AreNotEqual("ane", result);
            result = server.ExecuteCommand("ittn", new string[] { correctLogin2, correctPassword2, id.ToString(), insertCount.ToString(), instertText });
            Assert.AreEqual("ane", result);
        }

        [DataTestMethod]
        [DataRow("Tahoma", "password", "Name", "Text", 2020, 12, 12, "id")]
        [DataRow("Tahoma", "password", "Name", "", 2020, 12, 12, "id")]
        [DataRow("Tahoma", "password", "Name", "Text", -1, -1, -1, "id")]
        [DataRow("Tahoma", "password", "Name", "", -1, -1, -1, "id")]
        [DataRow("Alex92", "pass1234", "NameOfNote", "Some text about this note which have paragraph", 2038, 1, 12, "id")]
        [DataRow("Alex92", "pass1234", "NameOfNote", "", 2038, 1, 12, "id")]
        [DataRow("Alex92", "pass1234", "NameOfNote", "Some text about this note which have paragraph", -1, -1, -1, "id")]
        [DataRow("Alex92", "pass1234", "NameOfNote", "", -1, -1, -1, "id")]
        [DataRow("Tahoma", "password", "Name", "Text", 2020, 12, 12, "id")]
        [DataRow("Tahoma", "password", "Name", "Text", 2020, 12, 120500, "id")]
        [DataRow("Tahoma", "password", "", "", 2020, 12, 12, "id")]
        [DataRow("", "password", "Name", "Text", 2020, 12, 12, "ae")]
        [DataRow("Tahoma", "", "Name", "Text", 2020, 12, 12, "ae")]
        [DataRow("DmitryTahoma", "password", "Name", "Text", 2020, 12, 12, "False")]
        [DataRow("Tahoma", "noCorrectPassword", "Name", "Text", 2020, 12, 12, "False")]
        [DataRow("Tahoma", "pass1234", "Name", "Text", 2020, 12, 12, "False")]
        [DataRow("Alex92", "password", "Name", "Text", 2020, 12, 12, "False")]
        public void CreateNewParagraphMissionTest(string login, string password, string name, string text, int year, int month, int day, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            DateTime before = DateTime.Now;
            string result = server.ExecuteCommand("cnpm", new string[] { login, password, name, text, day.ToString(), month.ToString(), year.ToString() });
            DateTime after = DateTime.Now;

            if(expectedResult != "id")
                Assert.AreEqual(expectedResult, result);
            else
            {
                string[] ids = result.Split('|');
                Assert.IsTrue(ids.Length == 4);
                Assert.IsTrue(int.TryParse(ids[0], out int noteId));
                Assert.IsTrue(int.TryParse(ids[1], out int actionId));
                Assert.IsTrue(int.TryParse(ids[2], out int missionId));
                Assert.IsTrue(int.TryParse(ids[3], out int paragraphId));
                Assert.IsTrue(DBContext.Notes.Where(x => x.Id == noteId).Count() == 1);
                Assert.IsTrue(DBContext.Actions.Where(x => x.Id == actionId).Count() == 1);
                Assert.IsTrue(DBContext.Missions.Where(x => x.Id == missionId).Count() == 1);
                Assert.IsTrue(DBContext.Collections.Where(x => x.Id == paragraphId).Count() == 1);

                Mission mission = DBContext.Missions.Where(x => x.Id == missionId).First();
                Assert.AreEqual(mission.Action.NoteId, noteId);
                Assert.AreEqual(mission.Action.Id, actionId);
                Assert.AreEqual(mission.ContextId, paragraphId);

                Assert.IsFalse(mission.IsProgressType);
                Assert.AreEqual(0, ((Collection)mission.Context).Count);

                Database.Context.Action action = mission.Action;
                Assert.AreEqual(DateTime.MinValue, action.Start);
                Assert.AreEqual(DateTime.MaxValue, action.End);

                Note note = action.Note;
                Assert.AreEqual(name, note.Name);
                Assert.AreEqual(text, note.Text);
                bool hasCreated = false;
                DateTime created = DateTime.Now;
                try 
                {
                    created = new DateTime(day, month, year); 
                    hasCreated = true; 
                }
                catch { }
                if (hasCreated)
                    Assert.IsTrue(new TimeSpan(0, 0, 1) < (created > note.Created ? created - note.Created : note.Created - created));
                else
                    Assert.IsTrue(before <= note.LastChanged && note.LastChanged <= after);
                Assert.IsTrue(before <= note.LastChanged && note.LastChanged <= after);

                User user = note.User;
                Assert.AreEqual(login, user.Login);
                Assert.AreEqual(password, user.Password);
            }
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", "Name", "Text", new string[] { "Do first" }, "id")]
        [DataRow("Alex92", "pass1234", "Hello, world", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", new string[] { "Do first", "Ut enim ad minim veniam", "quis nostrud exercitation ullamco" }, "id")]
        [DataRow("", "pass1234", "Name", "Text", new string[] { "Do first" }, "ae")]
        [DataRow("Alex92", "", "Name", "Text", new string[] { "Do first" }, "ae")]
        [DataRow("Alex92", "pass1234", "Name", "Text", new string[] { "" }, "ae")]
        [DataRow("Alex92", "pass1234", "Name", "Text", new string[] { "", "", "" }, "ae")]
        [DataRow("Alex92", "pass1234", "Name", "Text", new string[] { "", "", "", "", "", "" }, "ae")]
        [DataRow("Alex92", "pass123456", "Name", "Text", new string[] { "Do first" }, "False")]
        [DataRow("Alex93", "pass1234", "Name", "Text", new string[] { "Do first" }, "False")]
        [DataRow("Tahoma", "password", "Name", "Text", new string[] { "Do first" }, "False")]
        [DataRow("Alex92", "password", "Name", "Text", new string[] { "Do first" }, "False")]
        [DataRow("Tahoma", "pass1234", "Name", "Text", new string[] { "Do first" }, "False")]
        public void AddPointToParagraphMissionTest(string login, string password, string name, string text, string[] points, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            int id = int.Parse(server.ExecuteCommand("cnpm", new string[] { correctLogin, correctPassword, name, text }));

            for (int i = 0; i < points.Length; ++i)
            {
                string result = server.ExecuteCommand("aptpm", new string[] { login, password, id.ToString(), points[i] });

                if (expectedResult == "id")
                {
                    Assert.IsTrue(int.TryParse(result, out int idp));
                    Mission mission = DBContext.Missions.Where(x => x.Id == id).First();
                    Collection collection = (Collection)mission.Context;
                    Assert.AreEqual(i + 1, collection.Count);
                    Point point = DBContext.Points.Where(x => x.Id == idp).First();
                    Assert.AreEqual(point.ParagraphId, collection.Id);
                }
                else
                    Assert.AreEqual(expectedResult, result);
            }
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", "New point text", "True")]
        [DataRow("Alex92", "pass1234", "point text Lorem ipsum", "True")]
        [DataRow("", "pass1234", "New point text", "ae")]
        [DataRow("Alex92", "", "New point text", "ae")]
        [DataRow("Alex92", "pass1234", "", "ae")]
        [DataRow("Tahoma", "pass1234", "point text Lorem ipsum", "False")]
        [DataRow("Alex92", "pass12345", "point text Lorem ipsum", "False")]
        [DataRow("Tahoma", "password", "point text Lorem ipsum", "False")]
        public void ChangePointTextTest(string login, string password, string newPointText, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            int id = int.Parse(server.ExecuteCommand("cnpm", new string[] { correctLogin, correctPassword, "Name of note", "Text of note" }));
            string result = server.ExecuteCommand("aptpm", new string[] { correctLogin, correctPassword, id.ToString(), "First text of point" });
            Assert.IsTrue(int.TryParse(result, out int idp));
            result = server.ExecuteCommand("cpt", new string[] { login, password, idp.ToString(), newPointText });

            if (expectedResult == "True")
                Assert.AreEqual(newPointText, DBContext.Points.Where(x => x.Id == idp).First().Name);
            else
                Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", "New point text", "True")]
        [DataRow("Alex92", "pass1234", "point text Lorem ipsum", "True")]
        [DataRow("", "pass1234", "New point text", "ae")]
        [DataRow("Alex92", "", "New point text", "ae")]
        [DataRow("Alex92", "pass1234", "", "ae")]
        [DataRow("Tahoma", "pass1234", "point text Lorem ipsum", "False")]
        [DataRow("Alex92", "pass12345", "point text Lorem ipsum", "False")]
        [DataRow("Tahoma", "password", "point text Lorem ipsum", "ane")]
        public void ChangeNoteNameTest(string login, string password, string newName, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            int id = int.Parse(server.ExecuteCommand("cnn", new string[] { correctLogin, correctPassword, "Name of note", "Text of note" }));
            string result = server.ExecuteCommand("chnn", new string[] { login, password, id.ToString(), newName });

            if (expectedResult == "True")
                Assert.AreEqual(newName, DBContext.Notes.Where(x => x.Id == id).First().Name);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", true, "True")]
        [DataRow("Alex92", "pass1234", false, "True")]
        [DataRow("", "pass1234", true, "ae")]
        [DataRow("Alex92", "", false, "ae")]
        [DataRow("Alex923", "pass1234", true, "False")]
        [DataRow("Alex92", "pass12345", false, "False")]
        [DataRow("Tahoma", "password", true, "False")]
        public void SetCheckedPointTest(string login, string password, bool val, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            int id = int.Parse(server.ExecuteCommand("cnpm", new string[] { correctLogin, correctPassword, "Name of note", "Text of note" }));
            int idp = int.Parse(server.ExecuteCommand("aptpm", new string[] { correctLogin, correctPassword, id.ToString(), "First text of point" }));
            string result = server.ExecuteCommand("scp", new string[] { login, password, idp.ToString(), val.ToString() });

            if (expectedResult == "True")
                Assert.AreEqual(val, DBContext.Points.Where(x => x.Id == idp).First().IsChecked);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", 50, 1, 1, 3, 2020, "data")]
        [DataRow("Tahoma", "password", 1000, 2, 29, 2, 2020, "data")]
        [DataRow("Tahoma92", "pass1234", 0, 1, 28, 2, 2020, "False")]
        [DataRow("Tahoma", "password1234", 0, 1, 28, 2, 2020, "False")]
        [DataRow("Alex92", "pass1234", 0, 1, 38, 2, 2020, "ae")]
        [DataRow("Alex92", "pass1234", 0, 1, 28, 23, 2020, "ae")]
        [DataRow("Alex92", "pass1234", 0, 1, 28, 2, -17, "ae")]
        public void GetDayTest(string login, string password, int countNotes, int dispDays, int day, int month, int year, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            if (expectedResult == "data")
            {
                GenerateNotes(login, password, countNotes, dispDays);

                string splitter = "\b<sgd>\b";
                expectedResult = splitter;
                List<Note> notes = DBContext.Notes.Where(x => x.User.Login == login && x.User.Password == password && x.Created.Day == day && x.Created.Month == month && x.Created.Year == year).ToList();
                foreach(Note note in notes)
                    if (DBContext.Actions.Where(x => x.NoteId == note.Id).Count() != 0)
                        expectedResult += DBContext.Missions.Where(x => x.ActionId ==
                                                DBContext.Actions.Where(y => y.NoteId == note.Id)
                                                    .First().Id).First().ToString() + splitter;
                    else
                        expectedResult += note.ToString() + splitter;
            }
            Assert.AreEqual(expectedResult, server.ExecuteCommand("gd", new string[] { login, password, day.ToString(), month.ToString(), year.ToString() }));
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", "Name", "Text", 10, 10, 2020, new string[] { "hello", ",!", "world" }, "True")]
        [DataRow("Alex92", "pass1234", "Temp name of note or paragraph note", "Some text in temp note", 1, 10, 2007, new string[] { "hello", ",!", "my", "2007" }, "True")]
        [DataRow("Alex92", "pass1234", "Name", "", 10, 10, 2020, new string[] { "hello", ",!", "world" }, "True")]
        [DataRow("Alex92", "pass1234", "Name", "Text", 10, 10, 2020, new string[] { "hello", ",!", "world" }, "True")]
        [DataRow("Alex92", "pass1234", "Name", "Text", 10, 10, 2020, new string[] { }, "True")]
        [DataRow("", "pass1234", "Name", "Text", 10, 10, 2020, new string[] { "hello", ",!", "world" }, "ae")]
        [DataRow("Alex92", "", "Name", "Text", 10, 10, 2020, new string[] { "hello", ",!", "world" }, "ae")]
        [DataRow("Alex952", "pass1234", "Name", "Text", 10, 10, 2020, new string[] { "hello", ",!", "world" }, "False")]
        [DataRow("Alex92", "pass41234", "Name", "Text", 10, 10, 2020, new string[] { "hello", ",!", "world" }, "False")]
        [DataRow("Tahoma", "password", "Name", "Text", 10, 10, 2020, new string[] { "hello", ",!", "world" }, "ane")]
        public void RemoveNoteCascadeTest(string login, string password, string name, string text, int day, int month, int year, string[] points, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 300);
            int id = int.Parse(server.ExecuteCommand("cnn", new string[] { correctLogin, correctPassword, name, text, day.ToString(), month.ToString(), year.ToString() }));

            string result = server.ExecuteCommand("rnc", new string[] { login, password, id.ToString() });
            if (expectedResult == "True")
                Assert.IsTrue(DBContext.Notes.Where(x => x.Id == id).Count() == 0);
            else
                Assert.AreEqual(expectedResult, result);

            id = int.Parse(server.ExecuteCommand("cnpm", new string[] { correctLogin, correctPassword, name, text, day.ToString(), month.ToString(), year.ToString() }));
            int[] pointsid = new int[points.Length];
            for(int i = 0; i < points.Length; ++i)            
                pointsid[i] = int.Parse(server.ExecuteCommand("aptpm", new string[] { correctLogin, correctPassword, id.ToString(), points[i] }));

            Mission mission = DBContext.Missions.Where(x => x.Id == id).First();
            int actionId = mission.ActionId;
            int noteId = mission.Action.NoteId;
            int collectionId = mission.ContextId;

            result = server.ExecuteCommand("rnc", new string[] { login, password, noteId.ToString() });
            if(expectedResult == "True")
            {
                Assert.IsTrue(DBContext.Notes.Where(x => x.Id == noteId).Count() == 0);
                Assert.IsTrue(DBContext.Actions.Where(x => x.Id == actionId).Count() == 0);
                Assert.IsTrue(DBContext.Missions.Where(x => x.Id == id).Count() == 0);
                Assert.IsTrue(DBContext.Collections.Where(x => x.Id == collectionId).Count() == 0);
                foreach (int pointid in pointsid)
                    Assert.IsTrue(DBContext.Points.Where(x => x.Id == pointid).Count() == 0);
            }
            else
                Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("Alex92", "pass1234", "TempText", "True")]
        [DataRow("Alex92", "pass1234", "TextTeNamehehe", "True")]
        [DataRow("", "pass1234", "TextTeNamehehe", "ae")]
        [DataRow("Alex92", "", "TextTeNamehehe", "ae")]
        [DataRow("Alex923", "pass1234", "TextTeNamehehe", "False")]
        [DataRow("Alex92", "passd1234", "TextTeNamehehe", "False")]
        [DataRow("Tahoma", "password", "TextTeNamehehe", "ane")]
        public void RemovePointTest(string login, string password, string name, string expectedResult)
        {
            ServerProgram server = new ServerProgram("192.168.0.106", 11221, new int[] { 11222 }, 100);
            int missionId = int.Parse(server.ExecuteCommand("cnpm", new string[] { correctLogin, correctPassword, "Name", "Text" }));
            int pointId = int.Parse(server.ExecuteCommand("aptpm", new string[] { correctLogin, correctPassword, missionId.ToString(), name }));

            string result = server.ExecuteCommand("rp", new string[] { login, password, pointId.ToString() });
            if(expectedResult == "True")
                Assert.IsTrue(DBContext.Points.Where(x => x.Id == pointId).Count() == 0);
            Assert.AreEqual(expectedResult, result);
        }
    }
}