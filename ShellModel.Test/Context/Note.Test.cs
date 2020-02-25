using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class NoteTest
    {
        [DataTestMethod]
        [DataRow(282, "Lorem ipsum dolor sit amet", "consectetur adipiscing elit")]
        [DataRow(883, "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.", "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.")]
        public void InitializationTest(int id, string name, string text)
        {
            Random random = new Random();
            DateTime created = DateTime.Now.AddDays(random.NextDouble() * -1);
            DateTime lastChanged = DateTime.Now;

            Note note1 = new Note(id, 0, name, text, created, lastChanged);
            Note note2 = new Note(0, name, text, created, lastChanged);

            ServerRealization.Database.Context.User user = new ServerRealization.Database.Context.User("Name", "Login", "Password", DateTime.Now);
            ServerRealization.Database.Context.Note dbNote = new ServerRealization.Database.Context.Note(id, user, new ServerRealization.Database.Context.Collection(), name, text, created, lastChanged);

            Note note3 = new Note(dbNote.ToString());

            Assert.AreEqual(id, note1.Id);
            Assert.AreEqual(id, note3.Id);
            Assert.AreEqual(name, note1.Name);
            Assert.AreEqual(name, note2.Name);
            Assert.AreEqual(name, note3.Name);
            Assert.AreEqual(text, note1.Text);
            Assert.AreEqual(text, note2.Text);
            Assert.AreEqual(text, note3.Text);
            Assert.AreEqual(created, note1.Created);
            Assert.AreEqual(created, note2.Created);

            if (created > note3.Created)
                Assert.IsTrue((created - note3.Created).TotalSeconds < 1);
            else if (note3.Created > created)
                Assert.IsTrue((note3.Created - created).TotalSeconds < 1);

            Assert.AreEqual(lastChanged, note1.LastChanged);
            Assert.AreEqual(lastChanged, note2.LastChanged);

            if (lastChanged > note3.LastChanged)
                Assert.IsTrue((lastChanged - note3.LastChanged).TotalSeconds < 1);
            else if (note3.LastChanged > lastChanged)
                Assert.IsTrue((note3.LastChanged - lastChanged).TotalSeconds < 1);
        }
    }
}