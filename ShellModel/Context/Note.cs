using ShellModel.Context.Commits;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;

namespace ShellModel.Context
{
    public class Note
    {
        public delegate void TimingAction();
        public event TimingAction Timing;

        bool isAutoTiming = false;
        Timer updateTimer;
        NoteCommit commit;

        public Note(string name, string text, bool autoTiming = false)
            : this(0, name, text, DateTime.Now, DateTime.Now, autoTiming) { }

        public Note(int id, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged, bool autoTiming = false)
        {
            Id = id;
            StereotypeId = stereotypeId;
            Name = name;
            Text = text;
            Created = created;
            LastChanged = lastChanged;
            isAutoTiming = autoTiming;
            if (isAutoTiming)
            {
                InitializeTimer();
                if (id < 1)
                    Id = DBHelper.CreateNoteStatic(this);
                commit = new NoteCommit(name, text);
            }
        }

        public Note(int stereotypeId, string name, string text, DateTime created, DateTime lastChanged, bool autoTiming = false)
            : this(-1, stereotypeId, name, text, created, lastChanged, autoTiming) { }

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
                isAutoTiming = true;
                InitializeTimer();
                commit = new NoteCommit(name, text);
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
        string name = "";
        public string Name
        {
            set
            {
                if(isAutoTiming)
                    if (name != value && updateTimer != null)
                        if(!updateTimer.Enabled)
                            updateTimer.Start();
                name = value;
            }
            get => name;
        }
        string text = "";
        public string Text
        {
            set
            {
                if(isAutoTiming)
                    if (text != value && updateTimer != null)
                        if(!updateTimer.Enabled)
                            updateTimer.Start();
                text = value;
            }
            get => text;
        }
        public DateTime Created { protected set; get; }
        public DateTime LastChanged { protected set; get; }
        public string StringLastChanged { get => LastChanged.ToString("dddd, dd MMMM yyyy HH:mm:ss"); }

        public static List<KeyValuePair<string, string[]>> GetChanges(Note realNote, Note oldNote)
        {
            List<KeyValuePair<string, string[]>> result = new List<KeyValuePair<string, string[]>>();
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

        private void InitializeTimer()
        {
            updateTimer = new Timer();
            updateTimer.Interval = 10000;
            updateTimer.Elapsed += (s, e) => 
            {
                try
                {
                    if (DBHelper.SaveChangesAsync(GetChanges(this, commit)).Result)
                    {
                        updateTimer.Stop();
                        LastChanged = DateTime.Now;
                        Timing?.Invoke();
                    }
                }
                catch(ArgumentException) { }
            };
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