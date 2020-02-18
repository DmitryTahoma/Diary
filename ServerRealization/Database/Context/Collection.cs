using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Collection : IDBObject
    {
        public Collection() : this(0) { }            

        public Collection(int count)
            : this(DBContext.Collections.Count() == 0 ? 1 : DBContext.Collections.Max(x => x.Id) + 1, count) { }

        public Collection(int id, int count)
        {
            if (id < 1)
                throw new ArgumentException();
            Id = id;
            Count = count < 0? 0 : count;
        }

        public int Id { private set; get; }
        public int Count { set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Collection))
                return false;
            Collection other = (Collection)obj;
            return this.Id == other.Id
                && this.Count == other.Count;
        }

        public override string ToString()
        {
            List<Point> points = DBContext.Points.Where(x => x.ParagraphId == Id).ToList();
            string result = "\b<sc>\b" + Count.ToString() + "\b<sc>\b";
            foreach (Point point in points)
                result += point.ToString() + "\b<sc>\b";
            return result;
        }
    }
}