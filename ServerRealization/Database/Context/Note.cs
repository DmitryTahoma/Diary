using System;
using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Note : IDBObject
    {
        public Note(int userId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged)
            : this(DBContext.Users.Where(x => x.Id == userId).First(),
                  DBContext.Collections.Where(x => x.Id == stereotypeId).First(), name, text, created, lastChanged) { }

        public Note(User user, Collection stereotype, string name, string text, DateTime created, DateTime lastChanged)
            : this(DBContext.Notes.Max(x => x.Id) + 1, user, stereotype, name, text, created, lastChanged) { }

        public Note(int id, int userId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged)
            : this(id, DBContext.Users.Where(x => x.Id == userId).First(),
                  DBContext.Collections.Where(x => x.Id == stereotypeId).First(), name, text, created, lastChanged) { }
        public Note(int id, User user, Collection stereotype, string name, string text, DateTime created, DateTime lastChanged)
        {
            Id = id;
            User = user;
            Stereotype = stereotype;
            Name = name;
            Text = text;
            Created = created;
            LastChanged = lastChanged;
        }

        public int Id { private set; get; }
        public int UserId { get => User.Id; }
        public int StereotypeId { get => Stereotype.Id; }
        public string Name { set; get; }
        public string Text { set; get; }
        public DateTime Created { set; get; }
        public DateTime LastChanged { set; get; }

        public User User { private set; get; }
        public Collection Stereotype { private set; get; }
    }
}