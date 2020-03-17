using System;

namespace ShellModel.Context
{
    public class ParagraphMission : Mission, IMission
    {
        public ParagraphMission(int id, int contextId, int actionId, int noteId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged, DateTime start, DateTime end)
            : base(id, MissionType.Paragraph, contextId, actionId, noteId, stereotypeId, name, text, created, lastChanged, start, end)
        { }

        public ParagraphMission(int id, Paragraph paragraph, int actionId, int noteId, int stereotypeId, string name,string text, DateTime created, DateTime lastChanged, DateTime start, DateTime end)
            : base (id, MissionType.Paragraph, paragraph.Id, actionId, noteId, stereotypeId, name, text, created, lastChanged, start, end)
        {
            Context = paragraph;
        }

        public ParagraphMission(string name, string text, DateTime created)
            : this(-1, new Paragraph(), -1, -1, 0, name, text, created, DateTime.Now, DateTime.MinValue, DateTime.MinValue) { }

        public Paragraph Paragraph { get => (Paragraph)Context; }
    }
}