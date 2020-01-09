using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Progress : IDBObject
    {
        public Progress(double start, double now, double end)
            : this(DBContext.Progresses.Max(x => x.Id) + 1, start, now, end) { }

        public Progress(int id, double start, double now, double end)
        {
            Id = id;
            Start = start;
            Now = now;
            End = end;
        }

        public int Id { private set; get; }
        public double Start { set; get; }
        public double Now { set; get; }
        public double End { set; get; }
    }
}
