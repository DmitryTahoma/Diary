using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Action : IDBObject
    {
        public Action(User user, NameAndComment nac, TimeSpan timeSpan)
            : this(DBContext.Actions.Max(x => x.Id) + 1, user, nac, timeSpan) { }

        public Action(int userId, int nacId, int timeSpanId)
            : this(DBContext.Actions.Max(x => x.Id) + 1, userId, nacId, timeSpanId) { }

        public Action(int id, int userId, int nacId, int timeSpanId)
        {
            Id = id;
            User = DBContext.Users.Where(x => x.Id == userId).First();
            NameAndComment = DBContext.NamesAndComments.Where(x => x.Id == nacId).First();
            TimeSpan = DBContext.TimeSpans.Where(x => x.Id == timeSpanId).First();
        }

        public Action(int id, User user, NameAndComment nac, TimeSpan timeSpan)
        {
            Id = id;
            User = user;
            NameAndComment = nac;
            TimeSpan = timeSpan;
        }

        public int Id { private set; get; }
        public int UserId { get => User.Id; }
        public int NacId { get => NameAndComment.Id; }
        public int TimeSpanId { get => TimeSpan.Id; }

        public User User { private set; get; }
        public NameAndComment NameAndComment { private set; get; }
        public TimeSpan TimeSpan { private set; get; }
    }
}
