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
            string splitter = "\b<sn>\b";
            Regex regex = new Regex("^\b<sn>\b\\d+\b<sn>\b[\\s\\S]*\b<sn>\b[\\s\\S]*\b<sn>\b\\d+[,\\d]*\b<sn>\b\\d+[,\\d]*\b<sn>\b");
            if (regex.IsMatch(dbStr))
            {
                string[] values = new string[5];
                for(int i = 0, s = 0, index = -1, start = 0; i < dbStr.Length; ++i)
                {
                    if (i == dbStr.Length - 1)
                        values[index] = dbStr.Substring(start, i - start - splitter.Length + 1);
                    if(s == splitter.Length)
                    {
                        if (index != -1)
                            values[index] = dbStr.Substring(start, i - start - splitter.Length);
                        index++;
                        s = 0;
                        start = i;
                        if (index == values.Length)
                            break;
                    }
                    if (splitter[s] == dbStr[i])
                        s++;
                    else if (s != 0)
                        s = 0;
                }
                Id = int.Parse(values[0]);
                Name = values[1];
                Text = values[2];
                Created = DateTime.MinValue.AddDays(double.Parse(values[3]));
                LastChanged = DateTime.MinValue.AddDays(double.Parse(values[4]));
            }
            else
                throw new ArgumentException();
        }

        public int Id { private set; get; }
        public int StereotypeId { private set; get; }
        public string Name { set; get; }
        public string Text { set; get; }
        public DateTime Created { private set; get; }
        public DateTime LastChanged { private set; get; }

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