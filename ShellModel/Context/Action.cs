using System;

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
