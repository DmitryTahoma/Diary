namespace Shell.ViewModels
{
    using Catel.MVVM;
    using System.Windows.Controls;
    using System;
    using Shell.Controls;

    public class YearViewModel : ViewModelBase
    {
        public YearViewModel()
        {
            CreateWeeks = new Command<StackPanel>(OnCreateWeeksExecute);
            Year = DateTime.Now.Year;
        }

        #region Properties

        public int Year { set; get; }

        #endregion

        #region Commands

        public Command<StackPanel> CreateWeeks { get; private set; }
        private void OnCreateWeeksExecute(StackPanel stackPanel)
        {
            DateTime start = new DateTime(Year, 1, 1);
            Week week = new Week();
            stackPanel.Children.Add(week);
            week.DataContext.Length = 7 - (int)start.DayOfWeek + 1;
            week.DataContext.Start = start;
            DateTime date = start.AddDays(week.DataContext.Length);
            for(int i = 0; date.AddDays(i * 7).Year == Year; ++i)
            {
                Week w = new Week();
                stackPanel.Children.Add(w);
                w.DataContext.Start = date.AddDays(7 * i);
            }
        }

        #endregion
    }
}