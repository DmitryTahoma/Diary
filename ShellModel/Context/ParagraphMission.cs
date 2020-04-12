using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellModel.Context
{
    public partial class ParagraphMission : Mission, IMission
    {
        new ParagraphCommit commit;

        public ParagraphMission(int id, int contextId, int actionId, int noteId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged, DateTime start, DateTime end)
            : base(id, MissionType.Paragraph, contextId, actionId, noteId, stereotypeId, name, text, created, lastChanged, start, end)
        { }

        public ParagraphMission(int id, Paragraph paragraph, int actionId, int noteId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged, DateTime start, DateTime end, bool isAutoTiming = false)
            : base (id, MissionType.Paragraph, paragraph.Id, actionId, noteId, stereotypeId, name, text, created, lastChanged, start, end)
        {
            if(isAutoTiming)
            {
                Context = new Paragraph(this, true) { Id = paragraph.Id };
                for (int i = 0; i < paragraph.Items.Count; ++i)
                    ((Paragraph)Context).Items.Add(paragraph.Items[i]);
                InitializeAutoTiming();
                base.isAutoTiming = true;
            }
            else
                Context = paragraph;
        }

        public ParagraphMission(string name, string text, DateTime created, bool autoTiming = false)
        {
            Type = MissionType.Paragraph;
            this.name = name;
            this.text = text;
            Created = created;
            LastChanged = DateTime.Now;
            Start = DateTime.MinValue;
            End = DateTime.MaxValue;
            isAutoTiming = autoTiming;

            if (isAutoTiming)
            {
                int[] ids = DBHelper.CreateParagraphMissionStatic(this);
                NoteId = ids[0];
                ActionId = ids[1];
                id = ids[2];
                Context = new Paragraph(this, autoTiming);
                Paragraph.Id = ids[3];
                InitializeAutoTiming();
            }
            else
                Context = new Paragraph();
        }

        public Paragraph Paragraph { get => (Paragraph)Context; }

        public void SetIntervalTiming(int mls)
        {
            if(mls > 499)
                updateTimer.Interval = mls;
        }

        private void InitializeAutoTiming()
        {
            commit = new ParagraphCommit(Paragraph.Id, Paragraph.Items);
            base.commit = new NoteCommit(name, text);
            InitializeTimer();
            updateTimer.Elapsed += (s, e) =>
            {
                try
                {
                    if (DBHelper.SaveChangesAsync(GetChanges(this, new ParagraphMission(Id, commit, -1, -1, -1, "", "", DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue))).Result)
                    {
                        commit = new ParagraphCommit(Paragraph.Id, Paragraph.Items);
                    }
                }
                catch (ArgumentException) { }
            };
            Paragraph.PointPropertyChanged += () =>
            {
                if (!updateTimer.Enabled)
                    updateTimer.Start();
            };
            Paragraph.OnAddedPoint += (point) => { commit.Items.Add(new PointCommit(point.Id, point.Text, point.IsChecked)); };
            Paragraph.OnRemovedPoint += (point) => { commit.Items.Remove(commit.Items.Where(x => x.Id == point.Id).First()); };
        }

        public static List<KeyValuePair<string, string[]>> GetChanges(ParagraphMission newParagraphMission, ParagraphMission oldParagraphMission)
        {
            List<KeyValuePair<string, string[]>> result = new List<KeyValuePair<string, string[]>>();
            for (int i = 0; i < newParagraphMission.Paragraph.Count; ++i)
            {
                if (newParagraphMission.Paragraph.Items
                        .Where((x) =>
                        {
                            Point p = oldParagraphMission.Paragraph.Items[i];
                            return x.Id == p.Id && x.Text != p.Text;
                        })
                        .Count() == 1)
                    result.Add(new KeyValuePair<string, string[]>("cpt", new string[] { newParagraphMission.Paragraph.Items[i].Id.ToString(), newParagraphMission.Paragraph.Items[i].Text }));
                if (newParagraphMission.Paragraph.Items
                        .Where((x) =>
                        {
                            Point p = oldParagraphMission.Paragraph.Items[i];
                            return x.Id == p.Id && x.IsChecked != p.IsChecked;
                        })
                        .Count() == 1)
                    result.Add(new KeyValuePair<string, string[]>("scp", new string[] { oldParagraphMission.Paragraph.Items[i].Id.ToString(), newParagraphMission.Paragraph.Items[i].IsChecked.ToString() }));
            }
            return result;
        }
    }
}