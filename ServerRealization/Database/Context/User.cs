using System;
using System.Linq;

namespace ServerRealization.Database.Context
{
    public class User : IDBObject
    {
        public User(string name, string login, string password, DateTime registration)
            : this(DBContext.Users.Max(x => x.Id) + 1, name, login, password, registration) { }

        public User(int id, string name, string login, string password, DateTime registration)
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
        public string Name { set; get; }
        public string Login { set; get; }
        public string Password { set; get; }
        public DateTime Registration { set; get; }
    }
}