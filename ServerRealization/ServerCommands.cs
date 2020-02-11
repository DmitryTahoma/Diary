using ServerCore;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;
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
            if (!ArgsHelper.CheckArgs(args, 3))
                return "ae";
            
            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            string text = "";
            if (args.Length >= 4)
                text = args[3];

            DateTime created = DateTime.Now;
            Note note = new Note(DBContext.Users.Where(x => x.Login == args[0]).First(),
                DBContext.Collections.Where(x => x.Id == 1).First(),
                args[2], text, created, created);
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

            DBContext.Notes
                .Where(x => x.Id == id)
                .First().Text += args[3];
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
            return "True";
        }

        private string CreateNewParagraphMission(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 3))
                return "ae";

            if(!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";

            DateTime end = DateTime.MinValue;
            if (ArgsHelper.CheckArgs(args.Skip(4).ToArray(), 6, 0, 1, 2, 3, 4, 5))
                try { end = new DateTime(int.Parse(args[4]), int.Parse(args[5]), int.Parse(args[6]), int.Parse(args[7]), int.Parse(args[8]), int.Parse(args[9])); }
                catch(ArgumentOutOfRangeException) { }

            DateTime created = DateTime.Now;
            Note note = new Note(DBContext.Users.Where(x => x.Login == args[0] && x.Password == args[1]).First(),
                DBContext.Collections.Where(x => x.Id == 1).First(), args[2], args[3], created, created);
            DBContext.Notes.Add(note);
            Database.Context.Action action = new Database.Context.Action(note, created, end);
            DBContext.Actions.Add(action);
            Collection collection = new Collection();
            DBContext.Collections.Add(collection);
            Mission mission = new Mission(action, false, collection);
            DBContext.Missions.Add(mission);
            return mission.Id.ToString();
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


            DBContext.Notes.Where(x => x.Id == id).First().Name = args[3];
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
    }
}