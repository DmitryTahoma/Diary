namespace Shell.ViewModels
{
    using Catel.MVVM;
    using Shell.Controls;
    using ShellModel.Context;
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    public class MonthViewModel : ViewModelBase
    {
        private List<Week> weeks = null;

        public delegate void NoteHandler(Note note);
        public event NoteHandler SelectDate;

        bool isLoaded = false;
        public bool IsLoaded { get => isLoaded; }

        public MonthViewModel()
        {
            BindStackPanel = new Command<StackPanel>(OnBindStackPanelExecute);            
        }

        #region Properties

        public int Month { set; get; }
        
        public int Year { set; get; }

        #endregion

        #region Commands

        public Command<StackPanel> BindStackPanel { get; private set; }
        private void OnBindStackPanelExecute(StackPanel stackPanel)
        {
            if (!isLoaded)
            {
                weeks = new List<Week>();
                DateTime start = new DateTime(Year, Month, 1);
                while (start.DayOfWeek != System.DayOfWeek.Monday)
                    start = start.AddDays(-1);

                for (int i = 0; start.AddDays(7 * i).Month != (Month + 1 == 13 ? 1 : Month + 1); ++i)
                {
                    Week w = new Week();
                    w.DataContext.Start = start.AddDays(7 * i);
                    w.DataContext.SelectDate += (note) => { SelectDate?.Invoke(note); };
                    stackPanel.Children.Add(w);
                    weeks.Add(w);
                }
                isLoaded = true;
            }
        }

        #endregion

        public void FindAndPaste(Note note, DateTime date)
        {
            foreach(Week week in weeks)
            {
                DateTime start = week.DataContext.Start;
                DateTime end = start.AddDays(week.DataContext.Length);
                if (start < date && date < end)
                {
                    week.DataContext.FindAndPaste(note, date);
                    break;
                }
            }
        }
    }
}