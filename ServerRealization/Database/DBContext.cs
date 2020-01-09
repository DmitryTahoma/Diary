using ServerRealization.Database.Context;
using System.Collections.Generic;

namespace ServerRealization.Database
{
    public class DBContext
    {
        private static List<TimeSpan> timeSpans;
        private static List<NameAndComment> namesAndComments;
        private static List<Name> names;
        private static List<Progress> progresses;
        private static List<User> users;
        private static List<Action> actions;
        private static List<CalculatedAction> calculatedActions;
        private static List<Mission> missions;
        private static List<Record> records;

        public static List<TimeSpan> TimeSpans
        {
            get
            {
                if (timeSpans == null)
                    Load(typeof(TimeSpan));
                return timeSpans;
            }
        }
        public static List<NameAndComment> NamesAndComments 
        {
            get
            {
                if (namesAndComments == null)
                    Load(typeof(NameAndComment));
                return namesAndComments;
            }
        }
        public static List<Name> Names
        {
            get
            {
                if (names == null)
                    Load(typeof(Name));
                return names;
            }
        }
        public static List<Progress> Progresses 
        {
            get
            {
                if (progresses == null)
                    Load(typeof(Progress));
                return progresses;
            }
        }
        public static List<User> Users
        {
            get
            {
                if (users == null)
                    Load(typeof(User));
                return users;
            }
        }
        public static List<Action> Actions
        {
            get
            {
                if (actions == null)
                    Load(typeof(Action));
                return actions;
            }
        }
        public static List<CalculatedAction> CalculatedActions
        {
            get
            {
                if (calculatedActions == null)
                    Load(typeof(CalculatedAction));
                return calculatedActions;
            }
        }
        public static List<Mission> Missions
        {
            get
            {
                if (missions == null)
                    Load(typeof(Mission));
                return missions;
            }
        }
        public static List<Record> Records 
        {
            get
            {
                if (records == null)
                    Load(typeof(Record));
                return records;
            }
        }

        private static void Load(System.Type type)
        {
            DBLoader loader = new DBLoader();
            if (type == typeof(TimeSpan))
            {
                timeSpans = new List<TimeSpan>();
                List<IDBObject> data = loader.Load("time_span");
                for (int i = 0; i < data.Count; ++i)
                    timeSpans.Add((TimeSpan)data[i]);
            }
            else if (type == typeof(NameAndComment))
            {
                namesAndComments = new List<NameAndComment>();
                List<IDBObject> data = loader.Load("names_and_comment");
                for (int i = 0; i < data.Count; ++i)
                    namesAndComments.Add((NameAndComment)data[i]);
            }
            else if (type == typeof(Name))
            {
                names = new List<Name>();
                List<IDBObject> data = loader.Load("names");
                for (int i = 0; i < data.Count; ++i)
                    names.Add((Name)data[i]);
            }
            else if (type == typeof(Progress))
            {
                progresses = new List<Progress>();
                List<IDBObject> data = loader.Load("progress");
                for (int i = 0; i < data.Count; ++i)
                    progresses.Add((Progress)data[i]);
            }
            else if (type == typeof(User))
            {
                if (names == null)
                    Load(typeof(Name));
                users = new List<User>();
                List<IDBObject> data = loader.Load("users");
                for (int i = 0; i < data.Count; ++i)
                    users.Add((User)data[i]);
            }
            else if (type == typeof(Mission))
            {
                if (namesAndComments == null)
                    Load(typeof(NameAndComment));
                if (progresses == null)
                    Load(typeof(Progress));
                missions = new List<Mission>();
                List<IDBObject> data = loader.Load("missions");
                for (int i = 0; i < data.Count; ++i)
                    missions.Add((Mission)data[i]);
            }
            else if (type == typeof(Action))
            {
                if (timeSpans == null)
                    Load(typeof(TimeSpan));
                if (namesAndComments == null)
                    Load(typeof(NameAndComment));
                if (users == null)
                    Load(typeof(User));
                actions = new List<Action>();
                List<IDBObject> data = loader.Load("action");
                for (int i = 0; i < data.Count; ++i)
                    actions.Add((Action)data[i]);
            }
            else if (type == typeof(Record))
            {
                if (missions == null)
                    Load(typeof(Mission));
                records = new List<Record>();
                List<IDBObject> data = loader.Load("records");
                for (int i = 0; i < data.Count; ++i)
                    records.Add((Record)data[i]);
            }
            else if (type == typeof(CalculatedAction))
            {
                if (actions == null)
                    Load(typeof(Action));
                calculatedActions = new List<CalculatedAction>();
                List<IDBObject> data = loader.Load("calculated_actions");
                for (int i = 0; i < data.Count; ++i)
                    calculatedActions.Add((CalculatedAction)data[i]);
            }
        }
    }
}
