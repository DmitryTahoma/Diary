using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class ProgressMissionTest
    {
        [DataTestMethod]
        [DataRow(1, 2, 3, 4, 5, "Hello", "World", 1, 2, 3, 4, 5, "Hello", "World", true)]
        [DataRow(6, 7, 7, 5, 85, "123456", "qwerty", 6, 7, 7, 5, 85, "123456", "qwerty", true)]
        [DataRow(12, 2, 3, 4, 5, "Hello", "World", 1, 2, 3, 4, 5, "Hello", "World", false)]
        [DataRow(1, 22, 3, 4, 5, "Hello", "World", 1, 2, 3, 4, 5, "Hello", "World", false)]
        [DataRow(1, 2, 33, 4, 5, "Hello", "World", 1, 2, 3, 4, 5, "Hello", "World", false)]
        [DataRow(1, 2, 3, 48, 5, "Hello", "World", 1, 2, 3, 4, 5, "Hello", "World", false)]
        [DataRow(1, 2, 3, 4, 59, "Hello", "World", 1, 2, 3, 4, 5, "Hello", "World", false)]
        [DataRow(1, 2, 3, 4, 5, "Heello", "World", 1, 2, 3, 4, 5, "Hello", "World", false)]
        [DataRow(1, 2, 3, 4, 5, "Hello", "Worldd", 1, 2, 3, 4, 5, "Hello", "World", false)]
        public void EqualsTest(int id1, int id2, int id3, int id4, int id5, string name, string text, int id1_2, int id2_2, int id3_2, int id4_2, int id5_2, string name_2, string text_2, bool expectedResult)
        {
            DateTime t = DateTime.Now;
            ProgressMission mission1 = new ProgressMission(id1, id2, id3, id4, id5, name, text, t, t, t, t);
            ProgressMission mission2 = new ProgressMission(id1_2, id2_2, id3_2, id4_2, id5_2, name_2, text_2, t, t, t, t);

            Assert.AreEqual(expectedResult, mission1.Equals(mission2));
        }
    }
}