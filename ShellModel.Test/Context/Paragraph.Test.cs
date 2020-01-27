using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class ParagraphTest
    {
        [DataTestMethod]
        [DataRow(0, new string[] { }, 0, new string[] { }, true)]
        [DataRow(10, new string[] { "NULL" }, 10, new string[] { "NULL" }, true)]
        [DataRow(0, new string[] { "", "123", "-r23tgef-sdf" }, 0, new string[] { "", "123", "-r23tgef-sdf" }, true)]
        [DataRow(10, new string[] { }, 0, new string[] { }, false)]
        [DataRow(0, new string[] { "" }, 0, new string[] { }, false)]
        [DataRow(0, new string[] { "", "123", "fgsdghsdfh" }, 0, new string[] { }, false)]
        [DataRow(0, new string[] { "fasdgsdfhfd", "asfgs" }, 0, new string[] { "fasdg", "dsgsd" }, false)]
        public void EqualsTest(int id1, string[] values1, int id2, string[] values2, bool expectedValue)
        {
            IMissionContext paragraph1 = new Paragraph(id1, values1.ToList());
            IMissionContext paragraph2 = new Paragraph(id2, values2.ToList());

            Assert.AreEqual(expectedValue, paragraph1.Equals(paragraph2));
        }
    }
}