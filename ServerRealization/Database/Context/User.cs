using System;
using System.Linq;

namespace ServerRealization.Database.Context
{
    public class User : IDBObject
    {
        public User(int nameId, string login, string password, DateTime registration)
            : this(DBContext.Names.Max(x => x.Id) + 1, nameId, login, password, registration) { }

        public User(Name name, string login, string password, DateTime registration)
            : this(DBContext.Names.Max(x => x.Id) + 1, name, login, password, registration) { }

        public User(int id, int nameId, string login, string password, DateTime registration)
            : this(id, DBContext.Names.Where(x => x.Id == nameId).First(), login, password, registration) { }

        public User(int id, Name name, string login, string password, DateTime registration)
        {
            if (DBContext.Users
                .Where(x => x.Login == login)
                .Count() != 0)
                throw new ArgumentException("Login must be unique");
            Id = id;
            Name = name;
            Login = login;
            Password = password;
            Registration = registration;
        }

        public int Id { private set; get; }
        public int NameId { get => Name.Id; }
        public string Login { set; get; }
        public string Password { set; get; }
        public DateTime Registration { set; get; }

        public Name Name { private set; get; }
    }
}
