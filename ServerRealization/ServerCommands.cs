﻿using ServerCore;
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
                case "dn": return DuplicateNote(args);
                case "dpm": return DuplicateParagraphMission(args);
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
            return note.Id.ToString() + "|" + action.Id.ToString() + "|" + mission.Id.ToString() + "|" + collection.Id.ToString();
        }

        private string AddPointToParagraphMission(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 3, 2))
                return "ae";
            int id = int.Parse(args[2]);

            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]) || ArgsHelper.IsAne(args[0], args[1], DBContext.Missions.Where(x => x.Id == id).First().Action.NoteId))
                return "False";

            string text = "";
            if (args.Length >= 4)
                text = args[3];

            Mission mission = DBContext.Missions.Where(x => x.Id == id).First();
            Point point = null;
            if (DBContext.Points.Count != 0)
                point = new Point(mission.ContextId, text, false);
            else
                point = new Point(1, mission.ContextId, text, false);
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

        private string RemoveNoteCascade(string[] args)
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

        private string RemovePoint(string[] args)
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

        private string DuplicateNote(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 6, 2, 3, 4, 5))
                return "ae";
            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";
            int id = int.Parse(args[2]);
            if (!ArgsHelper.NoteIsExist(id))
                return "ae";
            if (ArgsHelper.IsAne(args[0], args[1], id))
                return "ane";

            int day = int.Parse(args[3]);
            int month = int.Parse(args[4]);
            int year = int.Parse(args[5]);
            DateTime newCreated;
            try { newCreated = new DateTime(int.Parse(args[5]), int.Parse(args[4]), int.Parse(args[3])); }
            catch { return "ae"; }

            Note note = DBContext.Notes.Where(x => x.Id == id).First();
            Note dNote = new Note(note.User, DBContext.Collections.Where(x => x.Id == 1).First(), note.Name, note.Text, new DateTime(year, month, day), DateTime.Now);
            DBContext.Notes.Add(dNote);
            return dNote.Id.ToString();
        }

        private string DuplicateParagraphMission(string[] args)
        {
            if (!ArgsHelper.CheckArgs(args, 6, 2, 3, 4, 5))
                return "ae";
            if (!ArgsHelper.CheckLoginPassword(args[0], args[1]))
                return "False";
            int id = int.Parse(args[2]);
            Mission mission = DBContext.Missions.Where(x => x.Id == id).First();
            if (!ArgsHelper.NoteIsExist(mission.Action.NoteId))
                return "ae";
            if (ArgsHelper.IsAne(args[0], args[1], mission.Action.NoteId))
                return "ane";

            int day = int.Parse(args[3]);
            int month = int.Parse(args[4]);
            int year = int.Parse(args[5]);
            DateTime newCreated;
            try { newCreated = new DateTime(int.Parse(args[5]), int.Parse(args[4]), int.Parse(args[3])); }
            catch { return "ae"; }

            Note note = new Note(DBContext.Users.Where(x => x.Login == args[0] && x.Password == args[1]).First(),
                DBContext.Collections.Where(x => x.Id == 1).First(),
                mission.Action.Note.Name, 
                mission.Action.Note.Text, 
                newCreated,
                DateTime.Now);
            DBContext.Notes.Add(note);
            Database.Context.Action action = new Database.Context.Action(note, DateTime.MinValue, DateTime.MaxValue);
            DBContext.Actions.Add(action);
            Collection collection = new Collection();
            collection.Count = ((Collection)mission.Context).Count;
            DBContext.Collections.Add(collection);
            Mission dMission = new Mission(action, false, collection);
            DBContext.Missions.Add(dMission);
            List<Point> points = DBContext.Points.Where(x => x.ParagraphId == mission.ContextId).ToList();
            string result = note.Id.ToString() + "|" + action.Id.ToString() + "|" + dMission.Id.ToString() + "|" + collection.Id.ToString();
            foreach(Point point in points)
            {
                Point dPoint = new Point(dMission.ContextId, point.Name, point.IsChecked);
                DBContext.Points.Add(dPoint);
                result += "|" + dPoint.Id.ToString();
            }
            return result;
        }
    }
}