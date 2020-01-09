using System;
using System.Linq;

namespace ServerRealization.Database.Context
{
    public class TimeSpan : IDBObject
    {
        public TimeSpan(DateTime start, DateTime end)
            : this(DBContext.TimeSpans.Max(x => x.Id) + 1, start, end) { }

        public TimeSpan(int id, DateTime start, DateTime end)
        {
            Id = id;
            Start = start;
            End = end;
        }

        public int Id { private set; get; }
        public DateTime Start { set; get; }
        public DateTime End { set; get; }
    }
}
