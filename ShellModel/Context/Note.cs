using System;
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
            }
            else
                throw new ArgumentException();
        }

        protected Note() { }

        public int Id { protected set; get; }
        public int StereotypeId { protected set; get; }
        public string Name { set; get; }
        public string Text { set; get; }
        public DateTime Created { protected set; get; }
        public DateTime LastChanged { protected set; get; }

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