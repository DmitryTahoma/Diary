using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using System;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class ActionTest
    {
        [DataTestMethod]
        [DataRow(282, 420, "Lorem ipsum dolor sit amet", "consectetur adipiscing elit")]
        [DataRow(883, 923, "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.", "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.")]
        public void InitializationTest(int id, int noteId, string name, string text)
        {
            Random random = new Random();
            DateTime created = DateTime.Now.AddDays(random.NextDouble() * -1);
            DateTime lastChanged = DateTime.Now;
            DateTime start = new DateTime(random.Next());
            DateTime end = new DateTime(random.Next());
            if(start > end)
            {
                DateTime t = start;
                start = end;
                end = t;
            }

            Action action1 = new Action(id, noteId, 0, name, text, created, lastChanged, start, end);

            Note note2 = new Note(noteId, 0, name, text, created, lastChanged);
            Action action2 = new Action(id, note2, start, end);

            ServerRealization.Database.Context.User user = new ServerRealization.Database.Context.User("Name", "Login", "Password", DateTime.Now);
            ServerRealization.Database.Context.Note dbNote = new ServerRealization.Database.Context.Note(noteId, user, new ServerRealization.Database.Context.Collection(), name, text, created, lastChanged);
            ServerRealization.Database.Context.Action dbAction = new ServerRealization.Database.Context.Action(id, dbNote, start, end);
            Action action3 = new Action(dbAction.ToString());

            Assert.AreEqual(id, action1.Id);
            Assert.AreEqual(id, action2.Id);
            Assert.AreEqual(id, action3.Id);
            Assert.AreEqual(noteId, action1.NoteId);
            Assert.AreEqual(noteId, action2.NoteId);
            Assert.AreEqual(noteId, action3.NoteId);
            Assert.AreEqual(name, action1.Name);
            Assert.AreEqual(name, action2.Name);
            Assert.AreEqual(name, action3.Name);
            Assert.AreEqual(text, action1.Text);
            Assert.AreEqual(text, action2.Text);
            Assert.AreEqual(text, action3.Text);
            Assert.AreEqual(created, action1.Created);
            Assert.AreEqual(created, action2.Created);
            Assert.AreEqual(lastChanged, action1.LastChanged);
            Assert.AreEqual(lastChanged, action2.LastChanged);
            Assert.AreEqual(start, action1.Start);
            Assert.AreEqual(start, action2.Start);
            Assert.AreEqual(end, action1.End);
            Assert.AreEqual(end, action2.End);

            if (created > action3.Created)
                Assert.IsTrue((created - action3.Created).TotalSeconds < 1);
            else if (action3.Created > created)
                Assert.IsTrue((action3.Created - created).TotalSeconds < 1);

            if (lastChanged > action3.LastChanged)
                Assert.IsTrue((lastChanged - action3.LastChanged).TotalSeconds < 1);
            else if (action3.LastChanged > lastChanged)
                Assert.IsTrue((action3.LastChanged - lastChanged).TotalSeconds < 1);

            if (start > action3.Start)
                Assert.IsTrue((start - action3.Start).TotalSeconds < 1);
            else if (action3.Start > start)
                Assert.IsTrue((action3.Start - start).TotalSeconds < 1);

            if (end > action3.End)
                Assert.IsTrue((end - action3.End).TotalSeconds < 1);
            else if (action3.End > end)
                Assert.IsTrue((action3.End - end).TotalSeconds < 1);
        }
    }
}