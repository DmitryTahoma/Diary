using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;

namespace ServerRealization.Context.Test
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

            DBContext.Missions.Clear();
            DBContext.Actions.Clear();
            DBContext.Notes.Clear();
            DBContext.Users.Clear();
            DBContext.Progresses.Clear();

            User user = new User("Name", "Login", "Password", DateTime.Now);
            DBContext.Users.Add(user);

            Note note = new Note(noteId, user, new Collection(), name, text, created, lastChanged);
            DBContext.Notes.Add(note);

            Database.Context.Action action = new Database.Context.Action(actionId, note, startT, endT);
            DBContext.Actions.Add(action);

            Progress progress = new Progress(start, current, end);
            DBContext.Progresses.Add(progress);

            Mission mission1 = new Mission(action, true, progress);
            Mission mission2 = new Mission(id, action, true, progress);
            DBContext.Missions.Add(mission1);
            DBContext.Missions.Add(mission2);

            Assert.AreEqual(id, mission2.Id);
            Assert.AreEqual(true, mission1.IsProgressType);
            Assert.AreEqual(true, mission2.IsProgressType);
            Assert.AreEqual(user, mission1.Action.Note.User);
            Assert.AreEqual(user, mission2.Action.Note.User);
            Assert.AreEqual(note, mission1.Action.Note);
            Assert.AreEqual(note, mission2.Action.Note);
            Assert.AreEqual(action, mission1.Action);
            Assert.AreEqual(action, mission2.Action);
            Assert.AreEqual(progress, mission1.Context);
            Assert.AreEqual(progress, mission2.Context);
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

            DBContext.Missions.Clear();
            DBContext.Actions.Clear();
            DBContext.Notes.Clear();
            DBContext.Users.Clear();
            DBContext.Points.Clear();
            DBContext.Collections.Clear();

            User user = new User("Name", "Login", "Password", DateTime.Now);
            DBContext.Users.Add(user);

            Note note = new Note(noteId, user, new Collection(), name, text, created, lastChanged);
            DBContext.Notes.Add(note);

            Database.Context.Action action = new Database.Context.Action(actionId, note, startT, endT);
            DBContext.Actions.Add(action);

            Collection paragraph = new Collection(points.Length);
            DBContext.Collections.Add(paragraph);

            for(int i = 0; i < points.Length; ++i)
            {
                Point point = new Point(paragraph, points[i], isCheckeds[i]);
                DBContext.Points.Add(point);
            }

            Mission mission1 = new Mission(action, false, paragraph);
            Mission mission2 = new Mission(id, action, false, paragraph);
            DBContext.Missions.Add(mission1);
            DBContext.Missions.Add(mission2);

            Assert.AreEqual(id, mission2.Id);
            Assert.AreEqual(false, mission1.IsProgressType);
            Assert.AreEqual(false, mission2.IsProgressType);
            Assert.AreEqual(user, mission1.Action.Note.User);
            Assert.AreEqual(user, mission2.Action.Note.User);
            Assert.AreEqual(note, mission1.Action.Note);
            Assert.AreEqual(note, mission2.Action.Note);
            Assert.AreEqual(action, mission1.Action);
            Assert.AreEqual(action, mission2.Action);
            Assert.AreEqual(paragraph, mission1.Context);
            Assert.AreEqual(paragraph, mission2.Context);
        }

        [DataTestMethod]
        [DataRow(3752, 3984, 1732, "What's this fuss about true randomness?", "Perhaps you have wondered how predictable machines like computers can generate randomness. In reality, most random numbers used in computer programs are pseudo-random, which means they are generated in a predictable fashion using a mathematical formula. This is fine for many purposes, but it may not be random in the way you expect if you're used to dice rolls and lottery drawings.", 3750, 7373, 9751)]
        [DataRow(5212, 1131, 2012, "Lorem ipsum", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", 2341, 6416, 9440)]
        public void ToStringTestProgress(int id, int actionId, int noteId, string name, string text, int start, int current, int end)
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

            DBContext.Missions.Clear();
            DBContext.Actions.Clear();
            DBContext.Notes.Clear();
            DBContext.Users.Clear();
            DBContext.Progresses.Clear();

            User user = new User("Name", "Login", "Password", DateTime.Now);
            DBContext.Users.Add(user);

            Note note = new Note(noteId, user, new Collection(), name, text, created, lastChanged);
            DBContext.Notes.Add(note);

            Database.Context.Action action = new Database.Context.Action(actionId, note, startT, endT);
            DBContext.Actions.Add(action);

            Progress progress = new Progress(start, current, end);
            DBContext.Progresses.Add(progress);

            Mission mission = new Mission(id, action, true, progress);
            DBContext.Missions.Add(mission);

            Assert.AreEqual("\b<sm>\b" + id.ToString() + "\b<sm>\b" + action.ToString() + "\b<sm>\b" + progress.ToString() + "\b<sm>\b", mission.ToString());
        }

        [DataTestMethod]
        [DataRow(3752, 3984, 1732, "What's this fuss about true randomness?", "Perhaps you have wondered how predictable machines like computers can generate randomness. In reality, most random numbers used in computer programs are pseudo-random, which means they are generated in a predictable fashion using a mathematical formula. This is fine for many purposes, but it may not be random in the way you expect if you're used to dice rolls and lottery drawings.", new string[] { "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.", "Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.", "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem." }, new bool[] { true, false, true })]
        [DataRow(5212, 1131, 2012, "Lorem ipsum", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", new string[] { "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?", "Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?", "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga.", "Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus.", "Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae.", "Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat." }, new bool[] { true, true, false, false, false, false })]
        public void ToStringTestParagraph(int id, int actionId, int noteId, string name, string text, string[] points, bool[] isCheckeds)
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

            DBContext.Missions.Clear();
            DBContext.Actions.Clear();
            DBContext.Notes.Clear();
            DBContext.Users.Clear();
            DBContext.Points.Clear();
            DBContext.Collections.Clear();

            User user = new User("Name", "Login", "Password", DateTime.Now);
            DBContext.Users.Add(user);

            Note note = new Note(noteId, user, new Collection(), name, text, created, lastChanged);
            DBContext.Notes.Add(note);

            Database.Context.Action action = new Database.Context.Action(actionId, note, startT, endT);
            DBContext.Actions.Add(action);

            Collection paragraph = new Collection(points.Length);
            DBContext.Collections.Add(paragraph);

            for (int i = 0; i < points.Length; ++i)
            {
                Point point = new Point(paragraph, points[i], isCheckeds[i]);
                DBContext.Points.Add(point);
            }

            Mission mission = new Mission(id, action, false, paragraph);
            DBContext.Missions.Add(mission);

            Assert.AreEqual("\b<sm>\b" + (id * -1).ToString() + "\b<sm>\b" + action.ToString() + "\b<sm>\b" + paragraph.ToString() + "\b<sm>\b", mission.ToString());
        }
    }
}