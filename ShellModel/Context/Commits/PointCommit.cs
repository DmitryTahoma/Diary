namespace ShellModel.Context
{
    public partial class ParagraphMission
    {
        struct PointCommit
        {
            public PointCommit(int id, string text, bool isChecked)
            {
                Id = id;
                Text = text;
                IsChecked = isChecked;
            }

            public int Id { set; get; }
            public string Text { set; get; }
            public bool IsChecked { set; get; }

            public static implicit operator Point(PointCommit commit) => new Point(commit.Id, commit.Text, commit.IsChecked);
        }
    }
}