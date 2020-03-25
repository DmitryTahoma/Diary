using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class ParagraphTest
    {
        [DataTestMethod]
        [DataRow(73507, new int[] { 429403, 328177 }, new string[] { "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat." }, new bool[] { true, true })]
        [DataRow(964832, new int[] { 996491, 333150, 485311, 581062, 988271, 48390 }, new string[] { "Duis aute irure dolor", "in reprehenderit in", "voluptate velit esse cillum dolore", "eu fugiat nulla pariatur.", "Excepteur sint occaecat cupidatat non proident", "sunt in culpa qui officia deserunt mollit anim id est laborum" }, new bool[] { true, false, false, false, false, true })]
        public void InitializationTest(int id, int[] ids, string[] points, bool[] isCheckeds)
        {
            IMissionContext paragraph1 = new Paragraph();
            Paragraph paragraph2 = new Paragraph(id);

            List<Point> pointsL = new List<Point>();
            for (int i = 0; i < points.Length; ++i)
                pointsL.Add(new Point(ids[i], points[i], isCheckeds[i]));
            Paragraph paragraph3 = new Paragraph(id, pointsL);

            ServerRealization.Database.Context.Collection pointsDb = new ServerRealization.Database.Context.Collection(points.Length);
            DBContext.Collections.Add(pointsDb);
            for (int i = 0; i < points.Length; ++i)
                DBContext.Points.Add(new ServerRealization.Database.Context.Point(ids[i], pointsDb, points[i], isCheckeds[i]));
            Paragraph paragraph4 = new Paragraph(pointsDb.ToString());

            Assert.AreEqual(-1, paragraph1.Id);
            Assert.AreEqual(id, paragraph2.Id);
            Assert.AreEqual(id, paragraph3.Id);
            Assert.AreEqual(points.Length, paragraph3.Count);
            Assert.AreEqual(points.Length, paragraph4.Count);
            Assert.AreEqual(points.Length, paragraph3.Items.Count);
            Assert.AreEqual(points.Length, paragraph4.Items.Count);
            for (int i = 0; i < points.Length; ++i)
            {
                Assert.AreEqual(ids[i], paragraph3.Items[i].Id);
                Assert.AreEqual(points[i], paragraph3.Items[i].Text);
                Assert.AreEqual(isCheckeds[i], paragraph3.Items[i].IsChecked);

                Assert.AreEqual(ids[i], paragraph4.Items[i].Id);
                Assert.AreEqual(points[i], paragraph4.Items[i].Text);
                Assert.AreEqual(isCheckeds[i], paragraph4.Items[i].IsChecked);
            }
        }

        [DataTestMethod]
        [DataRow("text")]
        [DataRow("asdfghjklkjh")]
        public void AddPointTest(string text)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.AddPoint(new Point(text, false));

            Assert.AreEqual(text, paragraph.Items[0].Text);
            Assert.AreEqual(paragraph.Count, paragraph.Items.Count);
        }

        [DataTestMethod]
        [DataRow(-1, 1)]
        [DataRow(0, 2)]
        [DataRow(1, 3)]
        [DataRow(4, 5)]
        public void IdSetTest(int id, int newId)
        {
            Paragraph paragraph = new Paragraph(id);
            paragraph.Id = newId;
            Assert.AreEqual(id > 0 ? id : newId, paragraph.Id);
        }

        [DataTestMethod]
        [DataRow(new string[] { "123", "333", "563" }, 0)]
        [DataRow(new string[] { "123", "333", "563", "333", "563", "333", "563" }, 5)]
        public void RemovePointTest(string[] points, int deletingPoint)
        {
            Random random = new Random();
            Paragraph paragraph = new Paragraph();
            for (int i = 0; i < points.Length; ++i)
            {
                paragraph.AddPoint(new Point(random.Next(1000, 1000000), points[i], false));
                if (deletingPoint == i)
                    deletingPoint = paragraph.Items.Last().Id;
            }

            Assert.AreEqual(1, paragraph.Items.Where(x => x.Id == deletingPoint).Count());
            paragraph.RemovePoint(deletingPoint);
            Assert.AreEqual(0, paragraph.Items.Where(x => x.Id == deletingPoint).Count());
        }
    }
}