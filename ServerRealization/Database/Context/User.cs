using System;
using System.Linq;

namespace ServerRealization.Database.Context
{
    public class User : IDBObject
    {
        public User(string name, string login, string password, DateTime registration)
            : this(DBContext.Users.Count == 0 ? 1 : DBContext.Users.Max(x => x.Id) + 1, name, login, password, registration) { }

        public User(int id, string name, string login, string password, DateTime registration)
        {
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

        public override bool Equals(object obj)
        {
            if(!(obj is User))
                return false;
            User other = (User)obj;
            return this.Id == other.Id
                && this.Name == other.Name
                && this.Login == other.Login
                && this.Password == other.Password
                && this.Registration.Equals(other.Registration);
        }
    }
}