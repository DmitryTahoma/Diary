namespace Shell.ViewModels
{
    using Catel.MVVM;
    using Shell.Controls;
    using System;
    using System.Windows.Controls;

    public class MonthViewModel : ViewModelBase
    {
        bool isLoaded = false;

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
                DateTime start = new DateTime(Year, Month, 1);
                while (start.DayOfWeek != System.DayOfWeek.Monday)
                    start = start.AddDays(-1);

                for (int i = 0; start.AddDays(7 * i).Month != (Month + 1 == 13 ? 1 : Month + 1); ++i)
                {
                    Week w = new Week();
                    w.DataContext.Start = start.AddDays(7 * i);
                    stackPanel.Children.Add(w);
                }
                isLoaded = true;
            }
        }

        #endregion
    }
}