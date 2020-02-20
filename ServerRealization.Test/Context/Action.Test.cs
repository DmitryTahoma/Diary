using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;

namespace ServerRealization.Context.Test
{
    [TestClass]
    public class ActionTest
    {
        [DataTestMethod]
        [DataRow(250, 300, "Lorem", "ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        [DataRow(567, 8328, "Malorum", "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.")]
        public void InitializationTest(int id, int noteId, string name, string text)
        {
            DBContext.Actions.Clear();
            User user = new User("Name", "Login", "Password", DateTime.Now);
            DBContext.Users.Add(user);

            Random random = new Random();
            DateTime created = DateTime.Now.AddDays(random.NextDouble() * -1);
            DateTime lastChanged = DateTime.Now;
            Note note = new Note(noteId, user, new Collection(), name, text, created, lastChanged);
            DBContext.Notes.Add(note);

            DateTime start = new DateTime(random.Next());
            DateTime end = new DateTime(random.Next());
            if(start > end)
            {
                DateTime t = start;
                start = end;
                end = t;
            }

            Database.Context.Action action1 = new Database.Context.Action(note, start, end);
            DBContext.Actions.Add(action1);
            try
            {
                Database.Context.Action _ = new Database.Context.Action(note, end, start);
                Assert.Fail();
            }
            catch(ArgumentException) { }

            Database.Context.Action action2 = new Database.Context.Action(id, note, start, end);
            try
            {
                Database.Context.Action _ = new Database.Context.Action(id, note, end, start);
                Assert.Fail();
            }
            catch(ArgumentException) { }

            Assert.AreEqual(id, action2.Id);
            Assert.AreEqual(start, action1.Start);
            Assert.AreEqual(start, action2.Start);
            Assert.AreEqual(end, action1.End);
            Assert.AreEqual(end, action2.End);
        }

        [DataTestMethod]
        [DataRow(250, 300, "Lorem", "ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        [DataRow(567, 8328, "Malorum", "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.")]
        public void ToStringTest(int id, int noteId, string name, string text)
        {
            User user = new User("Name", "Login", "Password", DateTime.Now);
            DBContext.Users.Add(user);

            Random random = new Random();
            DateTime created = DateTime.Now.AddDays(random.NextDouble() * -1);
            DateTime lastChanged = DateTime.Now;
            Note note = new Note(noteId, user, new Collection(), name, text, created, lastChanged);
            DBContext.Notes.Add(note);

            DateTime start = new DateTime(random.Next());
            DateTime end = new DateTime(random.Next());
            if (start > end)
            {
                DateTime t = start;
                start = end;
                end = t;
            }
            Database.Context.Action action = new Database.Context.Action(id, note, start, end);

            Assert.AreEqual("\b<sa>\b" + id.ToString() + "\b<sa>\b" + note.ToString() + "\b<sa>\b" + (start - DateTime.MinValue).TotalDays.ToString() + "\b<sa>\b" + (end - DateTime.MinValue).TotalDays.ToString() + "\b<sa>\b", action.ToString());
        }
    }
}