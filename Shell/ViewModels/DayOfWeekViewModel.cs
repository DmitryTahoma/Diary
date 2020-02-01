namespace Shell.ViewModels
{
    using Catel.MVVM;
    using Shell.Controls;
    using System.Windows.Controls;

    public class DayOfWeekViewModel : ViewModelBase
    {
        public DayOfWeekViewModel()
        {
            AddNote = new Command<StackPanel>(OnAddNoteExecute);
        }

        #region Properties

        #endregion

        #region Commands

        public Command<StackPanel> AddNote { get; private set; }
        private void OnAddNoteExecute(StackPanel stackPanel)
        {
            stackPanel.Children.Add(new SimpleNote());
        }

        #endregion
    }
}