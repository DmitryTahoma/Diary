using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class ProgressTest
    {
        [DataTestMethod]
        [DataRow(1, 2, 3, 4)]
        [DataRow(15, 28, 39, 43)]
        public void InitializationTest(int id, int start, int current, int end)
        {
            IMissionContext progress1 = new Progress(id, start, current, end);
            ServerRealization.Database.Context.Progress progressDB = new ServerRealization.Database.Context.Progress(id, start, current, end);
            IMissionContext progress2 = new Progress(progressDB.ToString());

            Assert.AreEqual(id, progress1.Id);
            Assert.AreEqual(id, progress2.Id);
            Assert.AreEqual(start, ((Progress)progress1).Start);
            Assert.AreEqual(start, ((Progress)progress2).Start);
            Assert.AreEqual(current, progress1.Count);
            Assert.AreEqual(current, progress2.Count);
            Assert.AreEqual(end, ((Progress)progress1).End);
            Assert.AreEqual(end, ((Progress)progress2).End);
        }

        [DataTestMethod]
        [DataRow(1, 2, 3, 4)]
        [DataRow(15, 28, 39, 43)]
        public void EqualsTest(int id, int start, int count, int end)
        {
            IMissionContext progress1 = new Progress(id, start, count, end);
            IMissionContext progress2 = new Progress(id, start, count, end);

            Assert.AreEqual(progress1.GetHashCode(), progress2.GetHashCode());
            Assert.AreEqual(progress1, progress2);
        }
    }
}