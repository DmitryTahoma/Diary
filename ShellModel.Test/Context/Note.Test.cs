using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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

        [DataTestMethod]
        [DataRow(10, "name", "text", 11)]
        [DataRow(-28, "qwerty", "asfds", 29)]
        public void IdSetTest(int id, string name, string text, int newId)
        {
            Note note = new Note(id, 0, name, text, DateTime.Now, DateTime.Now);
            note.Id = newId;
            if (id > 0)
                Assert.AreEqual(id, note.Id);
            else
                Assert.AreEqual(newId, note.Id);
        }

        [DataTestMethod]
        [DataRow(1, "name", "text", 1, "name", "text123")]
        [DataRow(123, "name", "text111", 123, "name", "text")]
        [DataRow(581, "name", "text", 581, "name", "texToo")]
        [DataRow(5858, "adffffffff", "text", 5858, "namedfh", "text")]
        [DataRow(9854, "oldname", "123456789123456", 9854, "realname", "1234123")]
        public void GetChangesTest(int id1, string name1, string text1, int id2, string name2, string text2)
        {
            Note note1 = new Note(id1, 0, name1, text1, DateTime.Now, DateTime.Now);
            Note note2 = new Note(id2, 0, name2, text2, DateTime.Now, DateTime.Now);

            List<KeyValuePair<string, string[]>> expectedResult = new List<KeyValuePair<string, string[]>>();
            if (name1 != name2)
                expectedResult.Add(new KeyValuePair<string, string[]>("chnn", new string[] { id1.ToString(), name2 }));
            if(text1 != text2)
            {
                bool isEqualsStart = true;
                int idEqualEnd = 0;
                for(int i = 0; i < text1.Length && i < text2.Length; ++i)
                    if(text1[i] != text2[i])
                    {
                        isEqualsStart = false;
                        idEqualEnd = i;
                        break;
                    }

                if (isEqualsStart && text1.Length > text2.Length)
                    expectedResult.Add(new KeyValuePair<string, string[]>("rtfn", new string[] { id1.ToString(), (text1.Length - text2.Length).ToString() }));
                else if (isEqualsStart && text2.Length > text1.Length)
                    expectedResult.Add(new KeyValuePair<string, string[]>("attn", new string[] { id1.ToString(), text2.Substring(text1.Length, text2.Length - text1.Length) }));
                else if (!isEqualsStart)
                    expectedResult.Add(new KeyValuePair<string, string[]>("ittn", new string[] { id1.ToString(), (text1.Length - idEqualEnd).ToString(), text2.Substring(idEqualEnd, text2.Length - idEqualEnd) }));
            }

            List<KeyValuePair<string, string[]>> result = Note.GetChanges(note2, note1);

            Assert.AreEqual(expectedResult.Count, result.Count);
            for(int i = 0; i < expectedResult.Count; ++i)
            {
                Assert.AreEqual(expectedResult[i].Key, result[i].Key);
                Assert.AreEqual(expectedResult[i].Value.Length, result[i].Value.Length);
                for (int j = 0; j < expectedResult[i].Value.Length; ++j)
                    Assert.AreEqual(expectedResult[i].Value[j], result[i].Value[j]);
            }
        }
    }
}