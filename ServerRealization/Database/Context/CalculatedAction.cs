using System.Linq;

namespace ServerRealization.Database.Context
{
    public class CalculatedAction : IDBObject
    {
        public CalculatedAction(Action action, string counterName, int count)
            : this(DBContext.CalculatedActions.Max(x => x.Id) + 1, action, counterName, count) { }

        public CalculatedAction(int actionId, string counterName, int count)
            : this(DBContext.CalculatedActions.Max(x => x.Id) + 1, actionId, counterName, count) { }

        public CalculatedAction(int id, int actionId, string counterName, int count)
            : this(id, DBContext.Actions.Where(x => x.Id == actionId).First(), counterName, count) { }

        public CalculatedAction(int id, Action action, string counterName, int count)
        {
            Id = id;
            Action = action;
            CounterName = counterName;
            Count = count;
        }

        public int Id { private set; get; }
        public int ActionId { get => Action.Id; }
        public string CounterName { set; get; }
        public int Count { set; get; }

        public Action Action { private set; get; }
    }
}
