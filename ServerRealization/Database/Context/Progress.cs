using System;
using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Progress : IDBObject
    {
        public Progress(int start, int current, int end)
            : this(DBContext.Progresses.Count == 0? 1 : DBContext.Progresses.Max(x => x.Id) + 1, start, current, end) { }

        public Progress(int id, int start, int current, int end)
        {
            if (id <= 0 || end <= start)
                throw new ArgumentException();
            Id = id;
            Start = start;
            Current = current;
            End = end;
        }

        public int Id { private set; get; }
        public int Start { set; get; }
        public int Current { set; get; }
        public int End { set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Progress))
                return false;
            Progress other = (Progress)obj;
            return this.Id == other.Id
                && this.Start == other.Start
                && this.Current == other.Current
                && this.End == other.End;
        }
    }
}