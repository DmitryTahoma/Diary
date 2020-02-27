using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
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

        [DataTestMethod]
        [DataRow(3752, 3984, 1732, "What's this fuss about true randomness?", "Perhaps you have wondered how predictable machines like computers can generate randomness. In reality, most random numbers used in computer programs are pseudo-random, which means they are generated in a predictable fashion using a mathematical formula. This is fine for many purposes, but it may not be random in the way you expect if you're used to dice rolls and lottery drawings.", new string[] { "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.", "Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.", "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem." }, new bool[] { true, false, true })]
        [DataRow(5212, 1131, 2012, "Lorem ipsum", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", new string[] { "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?", "Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?", "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga.", "Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus.", "Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae.", "Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat." }, new bool[] { true, true, false, false, false, false })]
        public void InitializationTestParagraph(int id, int actionId, int noteId, string name, string text, string[] points, bool[] isCheckeds)
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
            DBContext.Users.Add(dbUser);
            ServerRealization.Database.Context.Note dbNote = new ServerRealization.Database.Context.Note(noteId, dbUser, new ServerRealization.Database.Context.Collection(), name, text, created, lastChanged);
            DBContext.Notes.Add(dbNote);
            ServerRealization.Database.Context.Action dbAction = new ServerRealization.Database.Context.Action(actionId, dbNote, startT, endT);
            DBContext.Actions.Add(dbAction);
            ServerRealization.Database.Context.Collection dbParagraph = new ServerRealization.Database.Context.Collection(points.Length);
            DBContext.Collections.Add(dbParagraph);
            for(int i = 0; i < points.Length; ++i)
            {
                ServerRealization.Database.Context.Point dbPoint = new ServerRealization.Database.Context.Point(dbParagraph, points[i], isCheckeds[i]);
                DBContext.Points.Add(dbPoint);
            }
            ServerRealization.Database.Context.Mission dbMission = new ServerRealization.Database.Context.Mission(id, dbAction, false, dbParagraph);

            ParagraphMission mission = (ParagraphMission)Mission.CreateNew(dbMission.ToString());

            Assert.AreEqual(id, mission.Id);
            Assert.AreEqual(actionId, mission.ActionId);
            Assert.AreEqual(noteId, mission.NoteId);
            Assert.AreEqual(name, mission.Name);
            Assert.AreEqual(text, mission.Text);
            Assert.AreEqual(points.Length, mission.Paragraph.Count);
            for(int i = 0; i < mission.Paragraph.Count; ++i)
            {
                Assert.AreEqual(points[i], mission.Paragraph.Items[i].Text);
                Assert.AreEqual(isCheckeds[i], mission.Paragraph.Items[i].IsChecked);
            }

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