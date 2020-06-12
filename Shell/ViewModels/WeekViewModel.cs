namespace Shell.ViewModels
{
    using Catel.MVVM;
    using ShellModel;
    using ShellModel.Context;
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    public class WeekViewModel : ViewModelBase
    {
        private List<Controls.DayOfWeek> days = null;

        public delegate void NoteHandler(Note note);
        public event NoteHandler SelectDate;

        bool isLoaded = false;

        public WeekViewModel()
        {
            CreateDays = new Command<WrapPanel>(OnCreateDaysExecute);
            Start = DateTime.Now;
            Length = 7;
        }

        #region Properties

        public DateTime Start { set; get; }

        public int Length { set; get; }

        #endregion

        #region Commands

        public Command<WrapPanel> CreateDays { get; private set; }
        private void OnCreateDaysExecute(WrapPanel wrapPanel)
        {
            if (!isLoaded)
            {
                days = new List<Controls.DayOfWeek>();
                DateTime date = Start;
                for (int i = 0; i < Length; ++i)
                {
                    Controls.DayOfWeek day = new Controls.DayOfWeek()
                    {
                        Margin = new System.Windows.Thickness(10),
                        MinWidth = 300,
                        MinHeight = 100
                    };
                    day.DataContext.Date = date.AddDays(i);
                    day.DataContext.SelectDate += (note) => { SelectDate?.Invoke(note); };
                    if(!DBHelper.IsNewUser)
                        day.DataContext.LoadDayFromDB();
                    wrapPanel.Children.Add(day);
                    days.Add(day);
                }
                isLoaded = true;
            }
        }

        #endregion

        public void FindAndPaste(Note note, DateTime date)
        {
            foreach(Controls.DayOfWeek day in days)
            {
                if (day.DataContext.Date.Date == date.Date)
                {
                    day.DataContext.Paste(note);
                    break;
                }
            }
        }
    }
}