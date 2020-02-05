namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Controls;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class DayOfWeekViewModel : ViewModelBase
    {
        public DayOfWeekViewModel()
        {
            AddNote = new Command<StackPanel>(OnAddNoteExecute);
            AddParagraphNote = new Command<StackPanel>(OnAddParagraphNoteExecute);
        }

        #region Properties

        DateTime date;
        public DateTime Date
        {
            set
            {
                date = value;
                DateString = date.ToString("dddd, dd MMMM");
            }
            get => date;
        }

        public string DateString
        {
            get { return GetValue<string>(DateStringProperty); }
            private set { SetValue(DateStringProperty, value); TodayTextVisibility = IsToday ? Visibility.Visible : Visibility.Collapsed; }
        }
        public static readonly PropertyData DateStringProperty = RegisterProperty(nameof(DateString), typeof(string), null);

        public bool IsToday 
        {
            get => date.Day == DateTime.Now.Day
                && date.Month == DateTime.Now.Month
                && date.Year == DateTime.Now.Year;
        }

        public Visibility TodayTextVisibility
        {
            get { return GetValue<Visibility>(TodayTextVisibilityProperty); }
            private set { SetValue(TodayTextVisibilityProperty, value); }
        }
        public static readonly PropertyData TodayTextVisibilityProperty = RegisterProperty(nameof(TodayTextVisibility), typeof(Visibility));

        #endregion

        #region Commands

        public Command<StackPanel> AddNote { get; private set; }
        private void OnAddNoteExecute(StackPanel stackPanel)
        {
            stackPanel.Children.Add(new SimpleNote());
        }

        public Command<StackPanel> AddParagraphNote { get; private set; }
        private void OnAddParagraphNoteExecute(StackPanel stackPanel)
        {
            ParagraphMissionNote note = new ParagraphMissionNote();
            note.DataContext.Context = new ShellModel.Context.ParagraphMission(0, 0, 0, 0, 0, "Name", "Text", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            stackPanel.Children.Add(note);
        }

        #endregion
    }
}