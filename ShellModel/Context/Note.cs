using System;

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