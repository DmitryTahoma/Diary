using System.Linq;

namespace ServerRealization.Database.Context
{
    public class ProgressNote : IDBObject
    {
        public ProgressNote(int noteId, int progressId, int size)
            : this(DBContext.Notes.Where(x => x.Id == noteId).First(),
                  DBContext.Progresses.Where(x => x.Id == progressId).First(), size) { }

        public ProgressNote(Note note, Progress progress, int size)
            : this(DBContext.ProgressNotes.Max(x => x.Id) + 1, note, progress, size) { }

        public ProgressNote(int id, int noteId, int progressId, int size)
            : this(id, DBContext.Notes.Where(x => x.Id == noteId).First(),
                  DBContext.Progresses.Where(x => x.Id == progressId).First(), size) { }

        public ProgressNote(int id, Note note, Progress progress, int size)
        {
            Id = id;
            Note = note;
            Progress = progress;
            Size = size;
        }

        public int Id { private set; get; }
        public int NoteId { get => Note.Id; }
        public int ProgressId { get => Progress.Id; }
        public int Size { set; get; }

        public Note Note { private set; get; }
        public Progress Progress { private set; get; }
    }
}