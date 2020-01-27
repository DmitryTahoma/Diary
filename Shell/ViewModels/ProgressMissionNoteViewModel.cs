namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using ShellModel.Context;
    using System;
    using System.Windows.Input;

    public class ProgressMissionNoteViewModel : ViewModelBase
    {
        public ProgressMissionNoteViewModel()
        {
            TextForChangeKeyDown = new Command<KeyEventArgs>(OnTextForChangeKeyDownExecute);
            AddBtnClick = new Command(OnAddBtnClickExecute);
            Loaded = new Command(OnLoadedExecute);
            CurrentProgress = GetProgressString(CurrentValue, MaxValue);
        }

        #region Properties

        public ProgressMission Context
        {
            get { return GetValue<ProgressMission>(ContextProperty); }
            set { SetValue(ContextProperty, value); OnSetContext(); }
        }
        public static readonly PropertyData ContextProperty = RegisterProperty(nameof(Context), typeof(ProgressMission), null);

        public int CurrentValue
        {
            get { return GetValue<int>(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }
        public static readonly PropertyData CurrentValueProperty = RegisterProperty(nameof(CurrentValue), typeof(int), 0);

        public int MaxValue
        {
            get { return GetValue<int>(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly PropertyData MaxValueProperty = RegisterProperty(nameof(MaxValue), typeof(int), 3);

        public string TextForChange
        {
            get { return GetValue<string>(TextForChangeProperty); }
            set { SetValue(TextForChangeProperty, value); }
        }
        public static readonly PropertyData TextForChangeProperty = RegisterProperty(nameof(TextForChange), typeof(string), "");

        public string CurrentProgress
        {
            get { return GetValue<string>(CurrentProgressProperty); }
            set { SetValue(CurrentProgressProperty, value); }
        }
        public static readonly PropertyData CurrentProgressProperty = RegisterProperty(nameof(CurrentProgress), typeof(string), "");

        public int TimeLeftMaxValue
        {
            get { return GetValue<int>(TimeLeftMaxValueProperty); }
            set { SetValue(TimeLeftMaxValueProperty, value); }
        }
        public static readonly PropertyData TimeLeftMaxValueProperty = RegisterProperty(nameof(TimeLeftMaxValue), typeof(int), 0);

        public int TimeLeftCurrent
        {
            get { return GetValue<int>(TimeLeftCurrentProperty); }
            set { SetValue(TimeLeftCurrentProperty, value); }
        }
        public static readonly PropertyData TimeLeftCurrentProperty = RegisterProperty(nameof(TimeLeftCurrent), typeof(int), 0);

        public string TimeLeftText
        {
            get { return GetValue<string>(TimeLeftTextProperty); }
            set { SetValue(TimeLeftTextProperty, value); }
        }
        public static readonly PropertyData TimeLeftTextProperty = RegisterProperty(nameof(TimeLeftText), typeof(string), null);

        public SimpleNoteViewModel Note
        {
            get { return GetValue<SimpleNoteViewModel>(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }
        public static readonly PropertyData NoteProperty = RegisterProperty(nameof(Note), typeof(SimpleNoteViewModel), null);
        
        #endregion

        #region Commands

        public Command<KeyEventArgs> TextForChangeKeyDown { get; private set; }
        private void OnTextForChangeKeyDownExecute(KeyEventArgs e)
        {
            if (!((e.Key == Key.OemMinus && TextForChange == "")
                || (int)e.Key > 33 && (int)e.Key < 44))
                e.Handled = true;
        }

        public Command AddBtnClick { get; private set; }
        private void OnAddBtnClickExecute()
        {
            if (TextForChange != "")
            {
                CurrentValue += int.Parse(TextForChange);
                CurrentProgress = GetProgressString(CurrentValue, MaxValue);
            }
        }

        public Command Loaded { get; private set; }
        private void OnLoadedExecute()
        {
            if (Note != null)
                Note.Note = Context;
        }

        #endregion

        private void OnSetContext()
        {
            if(Context != null)
            {
                Note = new SimpleNoteViewModel(Context);
                TimeSpan timeLeft = Context.End - Context.Start;
                TimeLeftMaxValue = (int)timeLeft.TotalDays;
                timeLeft = DateTime.Now - Context.Start;
                TimeLeftCurrent = (int)timeLeft.TotalDays;
                TimeLeftText = GetProgressString(TimeLeftCurrent, TimeLeftMaxValue);
            }
        }

        private string GetProgressString(int current, int max) => current.ToString() + " / " + max.ToString()
                + " [" + Math.Round((double)current / max * 100, 2).ToString() + "%]";
    }
}