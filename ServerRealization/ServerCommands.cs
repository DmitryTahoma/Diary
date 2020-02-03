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
            if (!ArgsHelper.CheckArgs(args, 4, true, 2))
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
            if (!ArgsHelper.CheckArgs(args, 4, true, 2, 3))
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
            if (!ArgsHelper.CheckArgs(args, 5, true, 2, 3))
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
    }
}