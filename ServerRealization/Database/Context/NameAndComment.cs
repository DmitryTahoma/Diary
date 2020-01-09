using System.Linq;

namespace ServerRealization.Database.Context
{
    public class NameAndComment : IDBObject
    {
        public NameAndComment(string name, string comment)
            : this(DBContext.NamesAndComments.Max(x => x.Id) + 1, name, comment) { }

        public NameAndComment(int id, string name, string comment)
        {
            Id = id;
            Name = name;
            Comment = comment;
        }

        public int Id { private set; get; }
        public string Name { set; get; }
        public string Comment { set; get; }
    }
}
