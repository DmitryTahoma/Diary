using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class ProgressTest
    {
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