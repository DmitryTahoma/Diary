using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class NoteTest
    {
        [DataTestMethod]
        [DataRow(12, 59, "NoteName", "NoteText", 2020, 2, 20, 20, 20, 20, 2021, 3, 21, 21, 21, 21)]
        [DataRow(127, 512, "Do smth", "01010001100101010010", 2010, 1, 10, 23, 20, 0, 2013, 4, 22, 23, 25, 10)]
        public void EqualsTest(int id, int stereotypeId, string name, string text,
            int createdYear, int createdMonth, int createdDay, int createdHour, int createdMinute, int createdSecond,
            int changedYear, int changedMonth, int changedDay, int changedHour, int changedMinute, int changedSecond)
        {
            Note note1 = new Note(id, stereotypeId, name, text,
                new DateTime(createdYear, createdMonth, createdDay, createdHour, createdMinute, createdSecond),
                new DateTime(changedYear, changedMonth, changedDay, changedHour, changedMinute, changedSecond));
            Note note2 = new Note(id, stereotypeId, name, text,
                new DateTime(createdYear, createdMonth, createdDay, createdHour, createdMinute, createdSecond),
                new DateTime(changedYear, changedMonth, changedDay, changedHour, changedMinute, changedSecond));

            Assert.AreEqual(note1.GetHashCode(), note2.GetHashCode());
            Assert.AreEqual(note1, note2);
        }
    }
}