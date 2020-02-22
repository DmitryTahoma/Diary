using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class PointTest
    {
        [DataTestMethod]
        [DataRow(102057, "Lorem ipsum", false)]
        [DataRow(1103245, "Sed ut perspiciatis", true)]
        [DataRow(333070, "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem.", false)]
        [DataRow(2117660545, "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?", true)]
        public void InitializationTest(int id, string text, bool isChecked)
        {
            Point point1 = new Point(id, text, isChecked);
            ServerRealization.Database.Context.Point dbPoint = new ServerRealization.Database.Context.Point(id, new ServerRealization.Database.Context.Collection(), text, isChecked);
            Point point2 = new Point(dbPoint.ToString());

            Assert.AreEqual(id, point1.Id);
            Assert.AreEqual(id, point2.Id);
            Assert.AreEqual(text, point1.Text);
            Assert.AreEqual(text, point2.Text);
            Assert.AreEqual(isChecked, point1.IsChecked);
            Assert.AreEqual(isChecked, point2.IsChecked);
        }
    }
}