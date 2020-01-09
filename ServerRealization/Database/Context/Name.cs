using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Name : IDBObject
    {
        public Name(string name, string surname, string patronymic)
            : this(DBContext.Names.Max(x => x.Id) + 1, name, surname, patronymic) { }

        public Name(int id, string name, string surname, string patronymic)
        {
            Id = id;
            TheName = name;
            Surname = surname;
            Patronymic = patronymic;
        }

        public int Id { private set; get; }
        public string TheName { set; get; }
        public string Surname { set; get; }
        public string Patronymic { set; get; }
    }
}
