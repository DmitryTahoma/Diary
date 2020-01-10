using ServerRealization.Database.Context;
using System.Collections.Generic;
using System.Linq;

namespace ServerRealization.Database
{
    public static class DBContext
    {
        private static List<Image> images;
        private static List<Progress> progresses;
        private static List<Collection> collections;
        private static List<User> users;
        private static List<Label> labels;
        private static List<Point> points;
        private static List<Note> notes;
        private static List<LabelCollection> labelCollections;
        private static List<Mission> missions;
        private static List<Action> actions;
        private static List<ProgressNote> progressNotes;

        public static List<Image> Images 
        {
            get
            {
                if (images == null)
                    Load("images");
                return images;
            }
        }
        public static List<Progress> Progresses 
        {
            get
            {
                if (progresses == null)
                    Load("progresses");
                return progresses;
            }
        }
        public static List<Collection> Collections
        {
            get
            {
                if (collections == null)
                    Load("collections");
                return collections;
            }
}
        public static List<User> Users
        {
            get
            {
                if (users == null)
                    Load("users");
                return users;
            }
        }
        public static List<Label> Labels
        {
            get
            {
                if (labels == null)
                    Load("labels");
                return labels;
            }
        }
        public static List<Point> Points
        {
            get
            {
                if (points == null)
                    Load("points");
                return points;
            }
        }
        public static List<Note> Notes
        {
            get
            {
                if (notes == null)
                    Load("notes");
                return notes;
            }
        }
        public static List<LabelCollection> LabelCollections
        {
            get
            {
                if (labelCollections == null)
                    Load("labelcollections");
                return labelCollections;
            }
        }
        public static List<Mission> Missions
        {
            get
            {
                if (missions == null)
                    Load("missions");
                return missions;
            }
        }    
        public static List<Action> Actions
        {
            get
            {
                if (actions == null)
                    Load("actions");
                return actions;
            }
        }
        public static List<ProgressNote> ProgressNotes
        {
            get
            {
                if (progressNotes == null)
                    Load("progressnotes");
                return progressNotes;
            }
        }

        private static void Load(string tableName)
        {
            DBLoader loader = new DBLoader();
            switch(tableName.ToLower())
            {
                default: throw new System.ArgumentException("Unknown table name");
                case "images":              images = (List<Image>)                      loader.Load(tableName); break;
                case "progresses":          progresses = (List<Progress>)               loader.Load(tableName); break;
                case "collections":         collections = (List<Collection>)            loader.Load(tableName); break;
                case "users":               users = (List<User>)                        loader.Load(tableName); break;
                case "labels":              labels = (List<Label>)                      loader.Load(tableName); break;
                case "points":              points = (List<Point>)                      loader.Load(tableName); break;
                case "notes":               notes = (List<Note>)                        loader.Load(tableName); break;
                case "progressnotes":       progressNotes = (List<ProgressNote>)        loader.Load(tableName); break;
                case "actions":             actions = (List<Action>)                    loader.Load(tableName); break;
                case "missions":            missions = (List<Mission>)                  loader.Load(tableName); break;
                case "labelcollections":    labelCollections = (List<LabelCollection>)  loader.Load(tableName); break;
            }
        }
    }
}