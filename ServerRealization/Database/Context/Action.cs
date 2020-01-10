using System;
using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Action : IDBObject
    {
        public Action(int noteId, DateTime start, DateTime end)
            : this(DBContext.Notes.Where(x => x.Id == noteId).First(), start, end) { }

        public Action(Note note, DateTime start, DateTime end)
            : this(DBContext.Actions.Max(x => x.Id) + 1, note, start, end) { }

        public Action(int id, int noteId, DateTime start, DateTime end)
            : this(id, DBContext.Notes.Where(x => x.Id == noteId).First(), start, end) { }

        public Action(int id, Note note, DateTime start, DateTime end)
        {
            Id = id;
            Note = note;
            Start = start;
            End = end;
        }

        public int Id { private set; get; }
        public int NoteId { get => Note.Id; }
        public DateTime Start { set; get; }
        public DateTime End { set; get; }

        public Note Note { private set; get; }
    }
}