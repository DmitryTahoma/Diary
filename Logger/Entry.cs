using System;

namespace Logger
{
    public class Entry
    {
        public string Text { private set; get; }
        public DateTime Time { private set; get; }

        public EntryLevel Level { private set; get; }

        public Entry(string text, EntryLevel level = EntryLevel.Undefined) : this(text, DateTime.Now, level) { }

        public Entry(string text, DateTime time, EntryLevel level = EntryLevel.Undefined)
        {
            Text = text;
            Time = time;
            Level = level;
        }

        public override bool Equals(object obj)
        {
            if(obj is Entry)
            {
                Entry entry = (Entry)obj;
                return entry.Text == this.Text
                    && entry.Time.Equals(this.Time)
                    && entry.Level == this.Level;
            }
            return false;
        }
    }
}
