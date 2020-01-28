using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class ParagraphMissionTest
    {
        [DataTestMethod]
        [DataRow(0, 1, 2, 3, 4, "Name", "Text", new string[] { "Lorem ipsum dolor", "sit amet, consectetur", "adipiscing elit, sed do eiusmod" })]
        [DataRow(10, 21, 32, 443, 53524, "gdsshdfh", "dgsdhsdfg", new string[] { "tempor incididunt ut labore", "et dolore magna aliqua. Ut enim", "ad minim veniam, quis nostrud exercitation", "ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." })]
        public void InitializationTest(int id, int cid, int aid, int nid, int sid, string name, string text, string[] items)
        {
            ParagraphMission mission = new ParagraphMission(id, cid, aid, nid, sid, name, text, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            Assert.AreEqual(id, mission.Id);
            Assert.AreEqual(cid, mission.ContextId);
            Assert.AreEqual(aid, mission.ActionId);
            Assert.AreEqual(nid, mission.NoteId);
            Assert.AreEqual(sid, mission.StereotypeId);
            Assert.AreEqual(name, mission.Name);
            Assert.AreEqual(text, mission.Text);
            Assert.IsTrue(mission is IMission && mission is Mission && mission.Type == MissionType.Paragraph);

            Paragraph paragraph = new Paragraph(cid, items.ToList());
            ParagraphMission mission2 = new ParagraphMission(id, paragraph, aid, nid, sid, name, text, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            Assert.AreEqual(items.Length, mission2.Context.Count);
            Assert.AreEqual(paragraph, mission2.Context);
            Assert.AreEqual(paragraph, mission2.Paragraph);
        }
    }
}