using ServerCore;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerRealization
{
    class ServerCommands : ICommands
    {
        public string ExecuteCommand(string commandName, string[] args)
        {
            switch(commandName)
            {
                default: return "message is received";
                case "cc": return "cs";
                case "clp": return CheckLoginPassword(args);
                case "rnu": return RegisterNewUser(args);
                case "cnn": return CreateNewNote(args);
                case "attn": return AddTextToNote(args);
                case "rtfn": return RemoveTextFromNote(args);
                case "ittn": return InsertTextToNote(args);
                case "cnpm": return CreateNewParagraphMission(args);
                case "aptpm": return AddPointToParagraphMission(args);
                case "cpt": return ChangePointText(args);
                case "chnn": return ChangeNoteName(args);
                case "scp": return SetCheckedPoint(args);
                case "gd": return GetDay(args);
                case "rnc": return RemoveNoteCascade(args);
                case "rp": return RemovePoint(args);
                case "generate1000notes": return Generate1000Notes();
            }
        }

        private string CheckLoginPassword(string[] args)
        {
            if(ArgsHelper.CheckArgs(args, 2))
                return ArgsHelper.CheckLoginPassword(args[0], args[1]).ToString();
            return "ae";
        }

        private string RegisterNewUser(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 3))
                return "ae";
            
            if (DBContext.Users
                .Where(x => x.Login == args[0])
                .Count() >= 1)
                return "uc";

            User user = new User(args[2], args[0], args[1], DateTime.Now);
            DBContext.Users.Add(user);
            return "True";
        }

        private string CreateNewNote(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 2))
                return "ae";
            
            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            string name = "";
            if (args.Length >= 3)
                name = args[2];

            string text = "";
            if (args.Length >= 4)
                text = args[3];

            int day = 0, month = 0, year = 0;
            bool hasCreated = false;
            if (args.Length >= 7)
                if (ArgsHelper.CheckArgs(new string[] { args[4], args[5], args[6] }, 3, 0, 1, 2))
                {
                    hasCreated = true;
                    day = int.Parse(args[4]);
                    month = int.Parse(args[5]);
                    year = int.Parse(args[6]);
                }

            DateTime created = DateTime.Now, lastChanged = created;
            if (hasCreated)
                try
                {
                    created = new DateTime(year, month, day);
                }
                catch(ArgumentOutOfRangeException) { }

            Note note = new Note(DBContext.Users.Where(x => x.Login == args[0]).First(),
                DBContext.Collections.Where(x => x.Id == 1).First(),
                name, text, created, lastChanged);
            DBContext.Notes.Add(note);
            return note.Id.ToString();
        }

        private string AddTextToNote(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 4, 2))
                return "ae";
            int id = int.Parse(args[2]);

            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            if (ArgsHelper.IsAne(args[0], args[1], id))
                return "ane";

            if (!ArgsHelper.NoteIsExist(id))
                return "False";

            Note current = DBContext.Notes
                                    .Where(x => x.Id == id)
                                    .First();
            current.Text += args[3];
            current.LastChanged = DateTime.Now;

            return "True";
        }

        private string RemoveTextFromNote(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 4, 2, 3))
                return "ae";
            int id = int.Parse(args[2]);
            int count = int.Parse(args[3]);

            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            if (ArgsHelper.IsAne(args[0], args[1], id))
                return "ane";

            if (!ArgsHelper.NoteIsExist(id))
                return "False";

            Note note = DBContext.Notes
                .Where(x => x.Id == id).First();
            if (note.Text.Length < count)
                return "ae";
            note.Text = note.Text.Substring(0, note.Text.Length - count);
            note.LastChanged = DateTime.Now;
            return "True";
        }

        private string InsertTextToNote(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 5, 2, 3))
                return "ae";
            int id = int.Parse(args[2]);
            int count = int.Parse(args[3]);

            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            if (ArgsHelper.IsAne(args[0], args[1], id))
                return "ane";

            if (!ArgsHelper.NoteIsExist(id))
                return "False";

            Note note = DBContext.Notes
                .Where(x => x.Id == id).First();
            if (note.Text.Length < count)
                return "ae";
            note.Text = note.Text.Substring(0, note.Text.Length - count) + args[4];
            note.LastChanged = DateTime.Now;
            return "True";
        }

        private string CreateNewParagraphMission(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 2))
                return "ae";
            if(!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            string name = "";
            if (args.Length >= 3)
                name = args[2];

            string text = "";
            if (args.Length >= 4)
                text = args[3];

            DateTime end = DateTime.MaxValue;
            DateTime created = DateTime.Now;
            if (ArgsHelper.CheckArgs(args.Skip(4).ToArray(), 3, 0, 1, 2))
                try { created = new DateTime(int.Parse(args[4]), int.Parse(args[5]), int.Parse(args[6])); }
                catch (ArgumentOutOfRangeException) { }

            Note note = new Note(DBContext.Users.Where(x => x.Login == args[0] && x.Password == args[1]).First(),
                DBContext.Collections.Where(x => x.Id == 1).First(), name, text, created, DateTime.Now);
            DBContext.Notes.Add(note);
            Database.Context.Action action = new Database.Context.Action(note, DateTime.MinValue, DateTime.MaxValue);
            DBContext.Actions.Add(action);
            Collection collection = new Collection();
            DBContext.Collections.Add(collection);
            Mission mission = new Mission(action, false, collection);
            DBContext.Missions.Add(mission);
            return mission.Id.ToString() + "|" + collection.Id.ToString();
        }

        private string AddPointToParagraphMission(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 4, 2))
                return "ae";
            int id = int.Parse(args[2]);

            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]) || ArgsHelper.IsAne(args[0], args[1], DBContext.Missions.Where(x => x.Id == id).First().Action.NoteId))
                return "False";

            Mission mission = DBContext.Missions.Where(x => x.Id == id).First();
            Point point = null;
            if (DBContext.Points.Count != 0)
                point = new Point(mission.ContextId, args[3], false);
            else
                point = new Point(1, mission.ContextId, args[3], false);
            DBContext.Points.Add(point);
            ((Collection)mission.Context).Count++;
            return point.Id.ToString();
        }

        private string ChangePointText(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 4, 2))
                return "ae";
            int id = int.Parse(args[2]);
            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]) || ArgsHelper.IsAne(args[0], args[1],
                DBContext.Missions.Where(x => x.ContextId ==
                    DBContext.Points.Where(y => y.Id == id).First().ParagraphId).First().Action.NoteId))
                return "False";

            DBContext.Points.Where(x => x.Id == id).First().Name = args[3];
            return "True";
        }

        private string ChangeNoteName(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 4, 2))
                return "ae";
            int id = int.Parse(args[2]);

            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]) || !ArgsHelper.NoteIsExist(id))
                return "False";

            if (ArgsHelper.IsAne(args[0], args[1], id))
                return "ane";


            Note current = DBContext.Notes.Where(x => x.Id == id).First();
            current.Name = args[3];
            current.LastChanged = DateTime.Now;

            return "True";
        }

        private string SetCheckedPoint(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 4, 2) || !bool.TryParse(args[3], out bool val))
                return "ae";
            int id = int.Parse(args[2]);

            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]) || ArgsHelper.IsAne(args[0], args[1],
                DBContext.Missions
                .Where(y => y.ContextId == DBContext.Points
                    .Where(x => x.Id == id)
                    .First().ParagraphId)
                .First().Action.NoteId))
                return "False";

            DBContext.Points.Where(x => x.Id == id).First().IsChecked = val;
            return "True";
        }

        private string GetDay(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 5, 2, 3, 4))
                return "ae";
            try
            {
                DateTime t = new DateTime(int.Parse(args[4]), int.Parse(args[3]), int.Parse(args[2]));
            }
            catch { return "ae"; }
            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            string splitter = "\b<sgd>\b";
            string result = splitter;
            List<Note> notes = DBContext.Notes.Where(x => x.User.Login == args[0] 
                                                  && x.User.Password == args[1] 
                                                  && x.Created.Day.ToString() == args[2] 
                                                  && x.Created.Month.ToString() == args[3] 
                                                  && x.Created.Year.ToString() == args[4])
                                                        .ToList();
            foreach (Note note in notes)
                if (DBContext.Actions.Where(x => x.NoteId == note.Id).Count() != 0)
                    result += DBContext.Missions.Where(x => x.ActionId ==
                                            DBContext.Actions.Where(y => y.NoteId == note.Id)
                                                .First().Id).First().ToString() + splitter;
                else
                    result += note.ToString() + splitter;

            return result;
        }

        public string RemoveNoteCascade(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 3, 2))
                return "ae";

            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            int id = int.Parse(args[2]);
            if (ArgsHelper.IsAne(args[0], args[1], id))
                return "ane";

            if(DBContext.Actions.Where(x => x.NoteId == id).Count() != 0)
            {
                Mission mission = DBContext.Missions
                    .Where(x => x.Id == 
                        DBContext.Actions
                        .Where(y => y.NoteId == id)
                        .First().Id)
                    .First();
                DBContext.Actions.Remove(mission.Action);
                List<Point> points = DBContext.Points.Where(x => x.Paragraph.Id == mission.ContextId).ToList();
                foreach (Point point in points)
                    DBContext.Points.Remove(point);
                DBContext.Collections.Remove((Collection)mission.Context);
                DBContext.Missions.Remove(mission);
            }
            DBContext.Notes.Remove(DBContext.Notes.Where(x => x.Id == id).First());

            return "True";
        }

        public string RemovePoint(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 3, 2))
                return "ae";
            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            int pointId = int.Parse(args[2]);
            int noteId = DBContext.Missions
                .Where(x => x.ContextId == 
                    DBContext.Points
                    .Where(y => y.Id == pointId)
                    .First()
                    .ParagraphId)
                .First()
                .Action
                .NoteId;

            if (ArgsHelper.IsAne(args[0], args[1], noteId))
                return "ane";

            DBContext.Points.Where(x => x.Id == pointId).First().Paragraph.Count--;
            DBContext.Points.Remove(DBContext.Points.Where(x => x.Id == pointId).First());
            return "True";
        }

        public string Generate1000Notes()
        {
            string login = "Alex92", password = "pass1234";
            int dispDays = 1, count = 1000;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            int userId = -1;
            try
            {
                userId = DBContext.Users.Where(x => x.Login == login && x.Password == password).First().Id;
            }
            catch { return ""; }

            for (int i = 0; i < count; ++i)
            {
                DateTime created = DateTime.Now.AddDays(dispDays * (random.Next() % 2 == 0 ? 1 : -1) + random.NextDouble()),
                         lastChanged = DateTime.Now.AddDays(dispDays * (random.Next() % 2 == 0 ? 1 : -1) + random.NextDouble());
                if (created > lastChanged)
                {
                    DateTime t = created;
                    created = lastChanged;
                    lastChanged = t;
                }
                Note note = new Note(userId, 1, new string(Enumerable.Repeat(chars, random.Next(10, 40)).Select(s => s[random.Next(s.Length)]).ToArray()), new string(Enumerable.Repeat(chars, random.Next(40, 120)).Select(s2 => s2[random.Next(s2.Length)]).ToArray()), created, lastChanged);
                DBContext.Notes.Add(note);
                if (random.Next() % 2 == 0)
                {
                    DateTime start = DateTime.Now.AddDays(dispDays * (random.Next() % 2 == 0 ? 1 : -1) + random.NextDouble()),
                             end = DateTime.Now.AddDays(dispDays * (random.Next() % 2 == 0 ? 1 : -1) + random.NextDouble());
                    if (start > end)
                    {
                        DateTime t = start;
                        start = end;
                        end = t;
                    }
                    Database.Context.Action action = new Database.Context.Action(note, start, end);
                    DBContext.Actions.Add(action);
                    Collection collection = new Collection(random.Next(0, 10));
                    DBContext.Collections.Add(collection);
                    for (int j = 0; j < collection.Count; ++j)
                    {
                        Point point = new Point(collection, new string(Enumerable.Repeat(chars, random.Next(10, 70)).Select(s => s[random.Next(s.Length)]).ToArray()), random.Next() % 2 == 0);
                        DBContext.Points.Add(point);
                    }
                    Mission mission = new Mission(action, false, collection);
                    DBContext.Missions.Add(mission);
                }
            }
            return "";
        }
    }
}