namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Controls;
    using System;
    using System.Windows.Controls;

    public class UserContentViewModel : ViewModelBase
    {
        public UserContentViewModel()
        {
            SelectedMonthId = DateTime.Now.Month - 1;
            SetMonth = new Command<string>(OnSetMonthExecute);
            InitializeMonths = new Command<ItemCollection>(OnInitializeMonthsExecute);
        }

        #region Properties

        public int SelectedMonthId
        {
            get { return GetValue<int>(SelectedMonthIdProperty); }
            set { SetValue(SelectedMonthIdProperty, value); }
        }
        public static readonly PropertyData SelectedMonthIdProperty = RegisterProperty(nameof(SelectedMonthId), typeof(int), null);

        #endregion

        #region Commands

        public Command<string> SetMonth { get; private set; }
        private void OnSetMonthExecute(string content)
        {
            switch(content)
            {
                case "January":     SelectedMonthId = 0; break;
                case "February":    SelectedMonthId = 1; break;
                case "March":       SelectedMonthId = 2; break;
                case "April":       SelectedMonthId = 3; break;
                case "May":         SelectedMonthId = 4; break;
                case "June":        SelectedMonthId = 5; break;
                case "July":        SelectedMonthId = 6; break;
                case "August":      SelectedMonthId = 7; break;
                case "September":   SelectedMonthId = 8; break;
                case "October":     SelectedMonthId = 9; break;
                case "November":    SelectedMonthId = 10; break;
                case "December":    SelectedMonthId = 11; break;
            }
        }

        public Command<ItemCollection> InitializeMonths { get; private set; }
        private void OnInitializeMonthsExecute(ItemCollection items)
        {
            int m = 1;
            foreach(TabItem item in items)
            {
                Month month = new Month();
                month.DataContext.Month = m;
                month.DataContext.Year = DateTime.Now.Year;
                item.Content = month;
                m++;
            }
        }

        #endregion
    }
}