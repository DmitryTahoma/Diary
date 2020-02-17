using System;
using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Point : IDBObject
    {
        public Point(int paragraphId, string name, bool isChecked)
            : this(DBContext.Collections.Where(x => x.Id == paragraphId).First(), name, isChecked) { }

        public Point(Collection paragraph, string name, bool isChecked)
            : this(DBContext.Points.Count == 0? 1 : DBContext.Points.Max(x => x.Id) + 1, paragraph, name, isChecked) { }

        public Point(int id, int paragraphId, string name, bool isChecked)
            : this(id, DBContext.Collections.Where(x => x.Id == paragraphId).First(), name, isChecked) { }

        public Point(int id, Collection paragraph, string name, bool isChecked)
        {
            if (id < 1)
                throw new ArgumentException();
            Id = id;
            Paragraph = paragraph;
            Name = name;
            IsChecked = isChecked;
        }

        public int Id { private set; get; }
        public int ParagraphId { get => Paragraph.Id; }
        public string Name { set; get; }
        public bool IsChecked { set; get; }

        public Collection Paragraph { private set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;
            Point other = (Point)obj;
            return this.Id == other.Id
                && this.Name == other.Name
                && this.IsChecked == other.IsChecked
                && this.Paragraph.Equals(other.Paragraph);
        }

        public override string ToString()
        {
            return "\b<sp>\b" + Id.ToString() + "\b<sp>\b" + Name + "\b<sp>\b" + (IsChecked ? "1\b<sp>\b" : "");
        }
    }
}