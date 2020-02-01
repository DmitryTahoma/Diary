namespace Shell.ViewModels
{
    using Catel.MVVM;
    using Shell.Controls;
    using ShellModel.Context;
    using System;
    using System.Windows.Controls;

    public class NoteCollectionViewModel : ViewModelBase
    {
        public NoteCollectionViewModel()
        {
            AddNote = new Command<StackPanel>(OnAddNoteExecute);
            AddPmNote = new Command<StackPanel>(OnAddPmNoteExecute);
            AddDow = new Command<StackPanel>(OnAddDowExecute);
        }

        #region Properties

        #endregion

        #region Commands

        public Command<StackPanel> AddNote { get; private set; }
        private void OnAddNoteExecute(StackPanel stackPanel)
        {
            stackPanel.Children.Add(new SimpleNote());
        }

        public Command<StackPanel> AddPmNote { get; private set; }
        private void OnAddPmNoteExecute(StackPanel stackPanel)
        {
            ProgressMissionNote note = new ProgressMissionNote();
            stackPanel.Children.Add(note);
            (note.DataContext).Context = new ProgressMission(0, 0, 0, 0, 0, "Name", "Text", DateTime.Now, DateTime.Now, new DateTime(2020, 1, 20, 0, 0, 0), new DateTime(2020, 2, 3, 0, 0, 0));
        }

        public Command<StackPanel> AddDow { get; private set; }
        private void OnAddDowExecute(StackPanel stackPanel)
        {
            Controls.DayOfWeek dow = new Controls.DayOfWeek();
            stackPanel.Children.Add(dow);
            dow.DataContext.Date = DateTime.Now;
        }

        #endregion
    }
}