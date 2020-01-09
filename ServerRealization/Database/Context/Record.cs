using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Record : IDBObject
    {
        public Record(int missionId, double newValue)
            : this(DBContext.Missions.Max(x => x.Id) + 1, missionId, newValue) { }

        public Record(Mission mission, double newValue)
            : this(DBContext.Missions.Max(x => x.Id) + 1, mission, newValue) { }

        public Record(int id, int missionId, double newValue)
            : this(id, DBContext.Missions.Where(x => x.Id == missionId).First(), newValue) { }

        public Record(int id, Mission mission, double newValue)
        {
            Id = id;
            Mission = mission;
            NewValue = newValue;
        }

        public int Id { private set; get; }
        public int MissionId { get => Mission.Id; }
        public double NewValue { set; get; }

        public Mission Mission { private set; get; }
    }
}
