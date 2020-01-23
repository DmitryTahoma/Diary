using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;
using System.Linq;

namespace ServerRealization.Test
{
    [TestClass]
    public class ServerCommandsTest
    {
        private static string correctLogin = "Alex92", correctPassword = "pass1234";

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
        [DataRow("Alex92", "pass1234", "", "Text", "ae")]
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
        public void InsertTextToNote(string login, string password, string name, string text, int removeCount, string addedText, string expectedResult)
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
    }
}