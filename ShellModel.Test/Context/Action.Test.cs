using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class ActionTest
    {
        [DataTestMethod]
        [DataRow(13, 31, 331, "Name", "TestText")]
        [DataRow(130, 140, 18000, "ShellModel", "ShellModel/Context/Action.cs")]
        public void EqualsTest(int id, int noteId, int stereotypeId, string name, string text)
        {
            Random r = new Random();
            int y1 = r.Next(2000, 2020), y2 = r.Next(2000, 2020), y3 = r.Next(2000, 2020), y4 = r.Next(2000, 2020),
                m1 = r.Next(1, 12),      m2 = r.Next(1, 12),      m3 = r.Next(1, 12),      m4 = r.Next(1, 12),
                d1 = r.Next(1, 28),      d2 = r.Next(1, 28),      d3 = r.Next(1, 28),      d4 = r.Next(1, 28),
                h1 = r.Next(1, 23),      h2 = r.Next(1, 23),      h3 = r.Next(1, 23),      h4 = r.Next(1, 23),
                n1 = r.Next(1, 59),      n2 = r.Next(1, 59),      n3 = r.Next(1, 59),      n4 = r.Next(1, 59),
                s1 = r.Next(1, 59),      s2 = r.Next(1, 59),      s3 = r.Next(1, 59),      s4 = r.Next(1, 59);

            Action action1 = new Action(id, noteId, stereotypeId, name, text, new DateTime(y1, m1, d1, h1, n1, s1), new DateTime(y2, m2, d2, h2, n2, s2), new DateTime(y3, m3, d3, h3, n3, s3), new DateTime(y4, m4, d4, h4, n4, s4));
            Action action2 = new Action(id, noteId, stereotypeId, name, text, new DateTime(y1, m1, d1, h1, n1, s1), new DateTime(y2, m2, d2, h2, n2, s2), new DateTime(y3, m3, d3, h3, n3, s3), new DateTime(y4, m4, d4, h4, n4, s4));

            Assert.AreEqual(action1.GetHashCode(), action2.GetHashCode());
            Assert.AreEqual(action1, action2);
        }
    }
}