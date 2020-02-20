using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;

namespace ServerRealization.Context.Test
{
    [TestClass]
    public class NoteTest
    {
        [DataTestMethod]
        [DataRow(100, "Lorem", "ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        [DataRow(12339, "Malorum", "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.")]
        public void InitializationTest(int id, string name, string text)
        {
            User user = new User("Name", "Login", "Password", DateTime.Now);
            DBContext.Users.Add(user);

            Random random = new Random();
            DateTime created = DateTime.Now.AddDays(random.NextDouble() * -1);
            DateTime lastChanged = DateTime.Now;
            Note note1 = new Note(id, user, new Collection(), name, text, created, lastChanged);
            Note note2 = new Note(user, new Collection(), name, text, created, lastChanged);
            DBContext.Notes.Add(note2);

            Assert.AreEqual(id, note1.Id);
            Assert.AreEqual(name, note1.Name);
            Assert.AreEqual(name, note2.Name);
            Assert.AreEqual(text, note1.Text);
            Assert.AreEqual(text, note2.Text);
        }

        [DataTestMethod]
        [DataRow(100, "Lorem", "ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        [DataRow(12339, "Malorum", "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.")]
        public void ToStringTest(int id, string name, string text)
        {
            User user = new User("Name", "Login", "Password", DateTime.Now);
            DBContext.Users.Add(user);

            Random random = new Random();
            DateTime created = DateTime.Now.AddDays(random.NextDouble() * -1);
            DateTime lastChanged = DateTime.Now;
            Note note = new Note(id, user, new Collection(), name, text, created, lastChanged);

            Assert.AreEqual("\b<sn>\b" + id.ToString() + "\b<sn>\b" + name + "\b<sn>\b" + text + "\b<sn>\b" + (created - DateTime.MinValue).TotalDays.ToString() + "\b<sn>\b" + (lastChanged - DateTime.MinValue).TotalDays.ToString() + "\b<sn>\b", note.ToString());
        }
    }
}