namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Controls;
    using ShellModel.Context;
    using System.Windows.Controls;

    public class ParagraphMissionNoteViewModel : ViewModelBase
    {
        public ParagraphMissionNoteViewModel()
        {
            AddNew = new Command<StackPanel>(OnAddNewExecute);
        }

        #region Properties

        public Note Note
        {
            get { return GetValue<Note>(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }
        public static readonly PropertyData NoteProperty = RegisterProperty(nameof(Note), typeof(Note), null);

        public ParagraphMission Context
        {
            get { return GetValue<ParagraphMission>(ContextProperty); }
            set { SetValue(ContextProperty, value); Note = value; }
        }
        public static readonly PropertyData ContextProperty = RegisterProperty(nameof(Context), typeof(ParagraphMission), null);

        #endregion

        #region Commands

        public Command<StackPanel> AddNew { get; private set; }
        private void OnAddNewExecute(StackPanel stackPanel)
        {
            stackPanel.Children.Insert(stackPanel.Children.Count - 1, new CheckTextBox());
        }

        #endregion
    }
}