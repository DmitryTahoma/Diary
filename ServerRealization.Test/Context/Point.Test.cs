using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;

namespace ServerRealization.Context.Test
{
    [TestClass]
    public class PointTest
    {
        [DataTestMethod]
        [DataRow(10, "Name", true, false)]
        [DataRow(14, "Some text", false, false)]
        [DataRow(177, "", true, false)]
        [DataRow(0, "Name", true, true)]
        [DataRow(-10, "Name", true, true)]
        public void InitializationTest(int id, string name, bool isChecked, bool isAe)
        {
            DBContext.Collections.Clear();
            DBContext.Points.Clear();

            Collection collection = new Collection();
            DBContext.Collections.Add(collection);

            Point point1 = null, point2 = null;

            try
            {
                point1 = new Point(collection, name, isChecked);
                if (isAe && id > 0)
                    Assert.Fail();
                DBContext.Points.Add(point1);
            }
            catch (ArgumentException) { }

            try
            {
                point2 = new Point(id, collection, name, isChecked);
                if (isAe)
                    Assert.Fail();
                DBContext.Points.Add(point2);
            }
            catch (ArgumentException) { }

            if(!isAe)
            {
                Assert.AreEqual(point2.Id, id);
                Assert.AreEqual(point1.Name, name);
                Assert.AreEqual(point2.Name, name);
                Assert.AreEqual(point1.IsChecked, isChecked);
                Assert.AreEqual(point2.IsChecked, isChecked);
            }
        }

        [DataTestMethod]
        [DataRow(10, "Name", true)]
        [DataRow(14, "Some text", false)]
        [DataRow(177, "", true)]
        public void ToStringTest(int id, string name, bool isChecked)
        {
            Point point = new Point(id, new Collection(), name, isChecked);
            Assert.AreEqual("\b<sp>\b" + id.ToString() + "\b<sp>\b" + name + "\b<sp>\b" + (isChecked? "1\b<sp>\b" : ""), point.ToString());
        }

        [DataTestMethod]
        [DataRow(10, "Name", true)]
        [DataRow(14, "Some text", false)]
        [DataRow(177, "", true)]
        public void EqualsTest(int id, string name, bool isChecked)
        {
            Collection collection = new Collection();
            Point point1 = new Point(id, collection, name, isChecked);
            Point point2 = new Point(id, collection, name, isChecked);
            Assert.AreEqual(point1, point2);

            point2.Name += "test";
            Assert.AreNotEqual(point1, point2);
            point2.Name = name;

            point1.IsChecked = !isChecked;
            Assert.AreNotEqual(point1, point2);

            point1 = new Point(id + 1, collection, name, isChecked);
            Assert.AreNotEqual(point1, point2);
        }
    }
}