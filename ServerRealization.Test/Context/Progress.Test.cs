using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;

namespace ServerRealization.Context.Test
{
    [TestClass]
    public class ProgressTest
    {
        [DataTestMethod]
        [DataRow(10, 0, 5, 40, false)]
        [DataRow(100, 10, 50, 400, false)]
        [DataRow(-1, 0, 5, 40, true)]
        [DataRow(-110, 0, 5, 40, true)]
        [DataRow(10, 110, 5, 40, true)]
        [DataRow(10, 0, 5, 0, true)]
        [DataRow(0, 0, 5, 40, true)]
        public void InitializationTest(int id, int start, int current, int end, bool isAe)
        {
            DBContext.Progresses.Clear();

            Progress progress1 = null;
            try
            {
                progress1 = new Progress(start, current, end);
                if (isAe && id > 0)
                    Assert.Fail();
                DBContext.Progresses.Add(progress1);
            }
            catch(ArgumentException) { }

            IDBObject progress2 = null;
            try
            {
                progress2 = new Progress(id, start, current, end);
                if (isAe)
                    Assert.Fail();
                DBContext.Progresses.Add((Progress)progress2);
            }
            catch(ArgumentException) { }

            if (!isAe)
            {
                Assert.AreEqual(id, progress2.Id);
                Assert.AreEqual(start, progress1.Start);
                Assert.AreEqual(start, ((Progress)progress2).Start);
                Assert.AreEqual(current, progress1.Current);
                Assert.AreEqual(current, ((Progress)progress2).Current);
                Assert.AreEqual(end, progress1.End);
                Assert.AreEqual(end, ((Progress)progress2).End);
            }
        }

        [DataTestMethod]
        [DataRow(1, 2, 3, 4)]
        [DataRow(100, 250, 348, 432534)]
        public void ToStringTest(int id, int start, int current, int end)
        {
            Progress progress = new Progress(id, start, current, end);
            Assert.AreEqual(id.ToString() + 'z' + start.ToString() + 'z' + current.ToString() + 'z' + end.ToString(), progress.ToString());
        }
    }
}