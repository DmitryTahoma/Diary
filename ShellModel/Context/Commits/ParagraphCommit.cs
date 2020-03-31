using System.Collections.Generic;

namespace ShellModel.Context
{
    public partial class ParagraphMission
    {
        struct ParagraphCommit
        {
            public ParagraphCommit(int id, List<Point> items)
            {
                Id = id;
                Items = new List<PointCommit>();
                for (int i = 0; i < items.Count; ++i)
                    Items.Add(new PointCommit(items[i].Id, items[i].Text, items[i].IsChecked));
            }

            public int Id { set; get; }
            public List<PointCommit> Items { set; get; }

            public static implicit operator Paragraph(ParagraphCommit commit)
            {
                List<Point> items = new List<Point>();
                for (int i = 0; i < commit.Items.Count; ++i)
                    items.Add(commit.Items[i]);
                return new Paragraph(commit.Id, items);
            }
        }
    }
}