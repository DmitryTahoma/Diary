﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ShellModel.Context
{
    public class Note
    {
        public Note(int id, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged)
        {
            Id = id;
            StereotypeId = stereotypeId;
            Name = name;
            Text = text;
            Created = created;
            LastChanged = lastChanged;
        }

        public Note(int stereotypeId, string name, string text, DateTime created, DateTime lastChanged)
            : this(-1, stereotypeId, name, text, created, lastChanged) { }

        public Note(string dbStr)
        {
            Regex regex = new Regex("^\b<sn>\b\\d+\b<sn>\b[\\s\\S]*\b<sn>\b[\\s\\S]*\b<sn>\b\\d+[,\\d[E\\-\\d]*]*\b<sn>\b\\d+[,\\d[E\\-\\d]*]*\b<sn>\b");
            if (regex.IsMatch(dbStr))
            {
                string[] values = StringsHelper.Split("\b<sn>\b", dbStr);

                Id = int.Parse(values[0]);
                Name = values[1];
                Text = values[2];
                Created = DateTime.MinValue.AddDays(double.Parse(values[3]));
                LastChanged = DateTime.MinValue.AddDays(double.Parse(values[4]));
                StringLastChanged = LastChanged.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            }
            else
                throw new ArgumentException();
        }

        protected Note() { }

        int id = -1;
        public int Id
        {
            set
            {
                if (id < 1)
                    id = value;
            }
            get => id;
        }
        public int StereotypeId { protected set; get; }
        public string Name { set; get; }
        public string Text { set; get; }
        public DateTime Created { protected set; get; }
        DateTime lastChanged;
        public DateTime LastChanged 
        {
            protected set 
            {
                lastChanged = value;
                StringLastChanged = LastChanged.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            }
            get => lastChanged;
        }
        public string StringLastChanged { protected set; get; }

        public static List<KeyValuePair<string, string[]>> GetChanges(Note realNote, Note oldNote)
        {
            List<KeyValuePair<string, string[]>> result = new List<KeyValuePair<string, string[]>>();
            if (realNote.id != oldNote.id)
                return result;
            if (realNote.Name != oldNote.Name)
                result.Add(new KeyValuePair<string, string[]>("chnn", new string[] { realNote.id.ToString(), realNote.Name }));
            if(realNote.Text != oldNote.Text)
            {
                bool ies = true;
                int iee = 0;
                for(int i = 0; i < realNote.Text.Length && i < oldNote.Text.Length; ++i)
                    if(realNote.Text[i] != oldNote.Text[i])
                    {
                        ies = false;
                        iee = i;
                        break;
                    }
                if (ies && realNote.Text.Length > oldNote.Text.Length)
                    result.Add(new KeyValuePair<string, string[]>("attn", new string[] { realNote.id.ToString(), realNote.Text.Substring(oldNote.Text.Length, realNote.Text.Length - oldNote.Text.Length) }));
                else if (ies && realNote.Text.Length < oldNote.Text.Length)
                    result.Add(new KeyValuePair<string, string[]>("rtfn", new string[] { realNote.id.ToString(), (oldNote.Text.Length - realNote.Text.Length).ToString() }));
                else if (!ies)
                    result.Add(new KeyValuePair<string, string[]>("ittn", new string[] { realNote.id.ToString(), (oldNote.Text.Length - iee).ToString(), realNote.Text.Substring(iee, realNote.Text.Length - iee) }));
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Note))
                return false;
            Note other = (Note)obj;
            return this.Id == other.Id
                && this.Name == other.Name
                && this.Text == other.Text
                && this.StereotypeId == other.StereotypeId
                && this.Created.Equals(other.Created)
                && this.LastChanged.Equals(other.LastChanged);
        }

        public override int GetHashCode()
        {
            var hashCode = -2020559781;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Created.GetHashCode();
            return hashCode;
        }
    }
}