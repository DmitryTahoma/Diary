namespace ShellModel.Context.Commits
{
    struct NoteCommit
    {
        public NoteCommit(string name, string text)
        {
            Name = name;
            Text = text;
        }

        public string Name { set; get; }
        public string Text { set; get; }


        public static implicit operator Note(NoteCommit commit) => new Note(commit.Name, commit.Text);        
    }
}