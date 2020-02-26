using System;
using System.Text.RegularExpressions;

namespace ShellModel.Context
{
    public class Action : Note
    {
        public Action(int id, int noteId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged, DateTime start, DateTime end)
            : base(noteId, stereotypeId, name, text, created, lastChanged)
        {
            Id = id;
            Start = start;
            End = end;
        }

        public Action(int id, Note baseNote, DateTime start, DateTime end)
            : base(baseNote.Id, baseNote.StereotypeId, baseNote.Name, baseNote.Text, baseNote.Created, baseNote.LastChanged)
        {
            Id = id;
            Start = start;
            End = end;
        }

        public Action(string dbStr) : base()
        {
            Regex regex = new Regex("^\b<sa>\b\\d+\b<sa>\b\b<sn>\b\\d+\b<sn>\b[\\s\\S]*\b<sn>\b[\\s\\S]*\b<sn>\b\\d+[,\\d[E\\-\\d]*]*\b<sn>\b\\d+[,\\d[E\\-\\d]*]*\b<sn>\b\b<sa>\b\\d+[,\\d[E\\-\\d]*]*\b<sa>\b\\d+[,\\d[E\\-\\d]*]*\b<sa>\b");
            if (regex.IsMatch(dbStr))
            {
                string[] values = StringsHelper.Split("\b<sa>\b", dbStr);

                Id = int.Parse(values[0]);
                Note baseNote = new Note(values[1]);
                Start = DateTime.MinValue.AddDays(double.Parse(values[2]));
                End = DateTime.MinValue.AddDays(double.Parse(values[3]));

                base.Id = baseNote.Id;
                StereotypeId = baseNote.StereotypeId;
                Name = baseNote.Name;
                Text = baseNote.Text;
                Created = baseNote.Created;
                LastChanged = baseNote.LastChanged;
            }
            else
                throw new ArgumentException();
        }

        public new int Id { private set; get; }
        public int NoteId { get => base.Id; }
        public DateTime Start { set; get; }
        public DateTime End { set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Action))
                return false;
            Action other = (Action)obj;
            return base.Equals(obj)
                && this.Id == other.Id
                && this.Start == other.Start
                && this.End == other.End;
        }

        public override int GetHashCode()
        {
            var hashCode = -668908338;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }
    }
}
