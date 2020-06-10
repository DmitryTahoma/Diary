﻿namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Controls;
    using System;
    using System.Windows.Controls;

    public class UserContentViewModel : ViewModelBase
    {
        public delegate void VoidHandler();
        public delegate void DateTimeHandler(DateTime selected);
        public event VoidHandler OnCancel;
        public event DateTimeHandler OnSelected;

        public UserContentViewModel()
        {
            SelectedMonthId = DateTime.Now.Month - 1;
            SelectedDate = DateTime.Now;
            SetMonth = new Command<string>(OnSetMonthExecute);
            InitializeMonths = new Command<ItemCollection>(OnInitializeMonthsExecute);
            AddDay = new Command(OnAddDayExecute);
            RemoveDay = new Command(OnRemoveDayExecute);
            CancelSelecting = new Command(OnCancelSelectingExecute);
            SelectDate = new Command(OnSelectDateExecute);
        }

        #region Properties

        public int SelectedMonthId
        {
            get { return GetValue<int>(SelectedMonthIdProperty); }
            set { SetValue(SelectedMonthIdProperty, value); }
        }
        public static readonly PropertyData SelectedMonthIdProperty = RegisterProperty(nameof(SelectedMonthId), typeof(int), null);
        
        public DateTime SelectedDate
        {
            get { return GetValue<DateTime>(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public static readonly PropertyData SelectedDateProperty = RegisterProperty(nameof(SelectedDate), typeof(DateTime), null);
        
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

        public Command AddDay { get; private set; }
        private void OnAddDayExecute()
        {
            SelectedDate = SelectedDate.AddDays(1);
        }

        public Command RemoveDay { get; private set; }
        private void OnRemoveDayExecute()
        {
            SelectedDate = SelectedDate.AddDays(-1);
        }

        public Command CancelSelecting { get; private set; }
        private void OnCancelSelectingExecute()
        {
            OnCancel?.Invoke();
        }

        public Command SelectDate { get; private set; }
        private void OnSelectDateExecute()
        {
            OnSelected?.Invoke(SelectedDate);
            SelectedDate = DateTime.Now;
            OnCancel?.Invoke();
        }

        #endregion
    }
}