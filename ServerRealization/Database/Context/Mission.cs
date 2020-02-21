using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Mission : IDBObject
    {
        public Mission(int actionId, bool isProgressType, int contextId)
            : this(DBContext.Missions.Count == 0 ? 1 : DBContext.Missions.Max(x => x.Id) + 1, actionId, isProgressType, contextId) { }

        public Mission(Action action, bool isProgressType, IDBObject context)
            : this(DBContext.Missions.Count == 0? 1 : DBContext.Missions.Max(x => x.Id) + 1, action, isProgressType, context) { }

        public Mission(int id, int actionId, bool isProgressType, int contextId)
            : this(id, DBContext.Actions.Where(x => x.Id == actionId).First(), isProgressType,
                  (isProgressType ? DBContext.Progresses.ToList<IDBObject>() : DBContext.Collections.ToList<IDBObject>())
                    .Where(x => x.Id == contextId).First()) { }

        public Mission(int id, Action action, bool isProgressType, IDBObject context)
        {
            Id = id;
            Action = action;
            IsProgressType = isProgressType;
            Context = context;
        }

        public int Id { private set; get; }
        public int ActionId { get => Action.Id; }
        public bool IsProgressType { set; get; }
        public int ContextId { get => Context.Id; }

        public Action Action { private set; get; }
        public IDBObject Context { private set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Mission))
                return false;
            Mission other = (Mission)obj;
            return this.Id == other.Id
                && this.IsProgressType == other.IsProgressType
                && this.Action.Equals(other.Action)
                && this.Context.Equals(other.Context);
        }
    }
}