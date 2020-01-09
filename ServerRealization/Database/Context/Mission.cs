using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Mission : IDBObject
    {
        public Mission(int progressId, int nacId)
            : this(DBContext.Missions.Max(x => x.Id) + 1, progressId, nacId) { }

        public Mission(Progress progress, NameAndComment nac)
            : this(DBContext.Missions.Max(x => x.Id) + 1, progress, nac) { }

        public Mission(int id, int progressId, int nacId)
            : this(id, DBContext.Progresses.Where(x => x.Id == progressId).First(), DBContext.NamesAndComments.Where(x => x.Id == nacId).First()) { }
        

        public Mission(int id, Progress progress, NameAndComment nac)
        {
            Id = id;
            Progress = progress;
            NameAndComment = nac;
        }

        public int Id { private set; get; }
        public int ProgressId { get => Progress.Id; }
        public int NacId { get => NameAndComment.Id; }

        public Progress Progress { private set; get; }
        public NameAndComment NameAndComment { private set; get; }
    }
}
