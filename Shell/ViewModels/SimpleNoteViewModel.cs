namespace Shell.ViewModels
{
    using Catel.MVVM;
    using Shell.Models;
    using System;

    public class SimpleNoteViewModel : ViewModelBase
    {
        public SimpleNoteViewModel()
        {
            Note = new Note(0, 0, 0, "Note not loaded", "Note not loaded", DateTime.Now, DateTime.Now);
        }

        #region Properties

        public Note Note { set; get; }

        public string Text { get => Note.Text; }

        public string Name { get => Note.Name; }

        public string LastChanged { get => Note.LastChanged.ToString("dddd, dd MMMM yyyy HH:mm:ss"); }

        #endregion
    }
}
