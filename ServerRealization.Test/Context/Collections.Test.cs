using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;
using System.Collections.Generic;

namespace ServerRealization.Context.Test
{
    [TestClass]
    public class CollectionTest
    {
        [DataTestMethod]
        [DataRow(0, 10)]
        [DataRow(-15, 10)]
        [DataRow(10, -17)]
        [DataRow(20, 13)]
        public void InitializationTest(int id, int count)
        {
            Collection collection1 = null, collection2 = new Collection(count), collection3 = new Collection();

            try
            {
                collection1 = new Collection(id, count);
                if (id < 1)
                    Assert.Fail();
            }
            catch(ArgumentException) { }

            if(collection1 != null)
            {
                Assert.AreEqual(id, collection1.Id);
                Assert.AreEqual(count < 0 ? 0 : count, collection1.Count);
            }
            Assert.AreEqual(count < 0 ? 0 : count, collection2.Count);
            Assert.AreEqual(0, collection3.Count);
        }

        [DataTestMethod]
        [DataRow(new string[] { "", "" }, new bool[] { true, false })]
        [DataRow(new string[] { "hello", "world" }, new bool[] { false, true })]
        [DataRow(new string[] { "Lorem ipsum dolor sit amet,", " consectetur adipiscing elit, sed do eiusmod ", "tempor incididunt ut labore et dolore magna aliqua.", " Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", " Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu", " fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia", " deserunt mollit anim id est laborum." }, new bool[] { true, false, false, true, true, true, true })]
        public void ToStringTest(string[] names, bool[] isCheckeds)
        {
            Collection collection = new Collection(names.Length);
            List<Point> points = new List<Point>();
            DBContext.Collections.Add(collection);
            for (int i = 0; i < names.Length; ++i)
            {
                Point p = new Point(collection, names[i], isCheckeds[i]);
                DBContext.Points.Add(p);
                points.Add(p);
            }

            string result = "\b<sc>\b" + names.Length.ToString() + "\b<sc>\b";
            for(int i = 0; i < names.Length; ++i)
            {
                result += points[i].ToString() + "\b<sc>\b";
            }
            Assert.AreEqual(result, collection.ToString());
        }
    }
}