namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Controls;
    using ShellModel;
    using ShellModel.Context;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    public class DayOfWeekViewModel : ViewModelBase
    {
        private StackPanel Notes = null;

        public DayOfWeekViewModel()
        {
            ChangeStackPanel = new Command<StackPanel>(OnChangeStackPanelExecute);
            AddNote = new Command(OnAddNoteExecute);
            AddParagraphNote = new Command(OnAddParagraphNoteExecute);
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

        public Command AddNote { get; private set; }
        private void OnAddNoteExecute()
        {
            SimpleNote note = new SimpleNote();
            note.DataContext.Note = new Note(0, "", "", Date, DateTime.Now, true);
            Notes.Children.Add(note);
            note.DataContext.Deleting += () => 
            {
                Notes.Children.Remove(note);
                DBHelper.RemoveNoteCascadeStatic(note.DataContext.Note);
            };
        }

        public Command AddParagraphNote { get; private set; }
        private void OnAddParagraphNoteExecute()
        {
            ParagraphMissionNote note = new ParagraphMissionNote();
            note.DataContext.Context = new ParagraphMission(-1, new Paragraph(), 0, 0, 0, "", "", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            Notes.Children.Add(note);
        }

        public Command<StackPanel> ChangeStackPanel { get; private set; }
        private void OnChangeStackPanelExecute(StackPanel stackPanel)
        {
            Notes = stackPanel;
        }

        #endregion
        
        public async void LoadDayFromDB()
        {
            List<Note> notes = await DBHelper.GetDayAsync(DBHelper.Login, DBHelper.Password, date.Day, date.Month, date.Year);
            foreach(Note n in notes)
                if(n is ParagraphMission)
                {
                    ParagraphMissionNote note = new ParagraphMissionNote();
                    note.DataContext.Context = (ParagraphMission) n;
                    Notes.Children.Add(note);
                }
                else
                {
                    SimpleNote note = new SimpleNote();
                    note.DataContext.Note = n;
                    Notes.Children.Add(note);
                    note.DataContext.Deleting += () =>
                    {
                        Notes.Children.Remove(note);
                        DBHelper.RemoveNoteCascadeStatic(note.DataContext.Note);
                    };
                }
        }
    }
}