using System;

namespace Shell.Models
{
    public class Note
    {
        public Note(int id, int userId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged)
        {
            Id = id;
            UserId = userId;
            StereotypeId = stereotypeId;
            Name = name;
            Text = text;
            Created = created;
            LastChanged = lastChanged;
        }

        public int Id { private set; get; }
        public int UserId { private set; get; }
        public int StereotypeId { private set; get; }
        public string Name { set; get; }
        public string Text { set; get; }
        public DateTime Created { set; get; }
        public DateTime LastChanged { set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Note))
                return false;
            Note other = (Note)obj;
            return this.Id == other.Id
                && this.Name == other.Name
                && this.Text == other.Text
                && this.UserId == other.UserId
                && this.StereotypeId == other.StereotypeId;
        }
    }
}