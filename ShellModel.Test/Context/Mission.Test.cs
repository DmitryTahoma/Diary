using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ShellModel.Context.Test
{
    [TestClass]
    public class MissionTest
    {
        [DataTestMethod]
        [DataRow(3752, 3984, 1732, "What's this fuss about true randomness?", "Perhaps you have wondered how predictable machines like computers can generate randomness. In reality, most random numbers used in computer programs are pseudo-random, which means they are generated in a predictable fashion using a mathematical formula. This is fine for many purposes, but it may not be random in the way you expect if you're used to dice rolls and lottery drawings.", 3750, 7373, 9751)]
        [DataRow(5212, 1131, 2012, "Lorem ipsum", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", 2341, 6416, 9440)]
        public void InitializationTestProgress(int id, int actionId, int noteId, string name, string text, int start, int current, int end)
        {
            Random random = new Random();
            DateTime created = DateTime.Now.AddDays(random.NextDouble() * -1);
            DateTime lastChanged = DateTime.Now;
            DateTime startT = new DateTime(random.Next());
            DateTime endT = new DateTime(random.Next());
            if (startT > endT)
            {
                DateTime t = startT;
                startT = endT;
                endT = t;
            }

            ServerRealization.Database.Context.User dbUser = new ServerRealization.Database.Context.User("Name", "Login", "Password", DateTime.Now);
            ServerRealization.Database.Context.Note dbNote = new ServerRealization.Database.Context.Note(noteId, dbUser, new ServerRealization.Database.Context.Collection(), name, text, created, lastChanged);
            ServerRealization.Database.Context.Action dbAction = new ServerRealization.Database.Context.Action(actionId, dbNote, startT, endT);
            ServerRealization.Database.Context.Mission dbMission = new ServerRealization.Database.Context.Mission(id, dbAction, true, new ServerRealization.Database.Context.Progress(start, current, end));

            ProgressMission mission = (ProgressMission)Mission.CreateNew(dbMission.ToString());

            Assert.AreEqual(id, mission.Id);
            Assert.AreEqual(actionId, mission.ActionId);
            Assert.AreEqual(noteId, mission.NoteId);
            Assert.AreEqual(name, mission.Name);
            Assert.AreEqual(text, mission.Text);
            Assert.AreEqual(start, ((Progress)mission.Context).Start);
            Assert.AreEqual(current, mission.Context.Count);
            Assert.AreEqual(end, ((Progress)mission.Context).End);

            if (created > mission.Created)
                Assert.IsTrue((created - mission.Created).TotalSeconds < 1);
            else if (mission.Created > created)
                Assert.IsTrue((mission.Created - created).TotalSeconds < 1);

            if (lastChanged > mission.LastChanged)
                Assert.IsTrue((lastChanged - mission.LastChanged).TotalSeconds < 1);
            else if (mission.LastChanged > lastChanged)
                Assert.IsTrue((mission.LastChanged - lastChanged).TotalSeconds < 1);

            if (startT > mission.Start)
                Assert.IsTrue((startT - mission.Start).TotalSeconds < 1);
            else if (mission.Start > startT)
                Assert.IsTrue((mission.Start - startT).TotalSeconds < 1);

            if (endT > mission.End)
                Assert.IsTrue((endT - mission.End).TotalSeconds < 1);
            else if (mission.End > endT)
                Assert.IsTrue((mission.End - endT).TotalSeconds < 1);
        }
    }
}