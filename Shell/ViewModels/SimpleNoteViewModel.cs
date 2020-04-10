namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using ShellModel.Context;
    using System;
    using System.Windows;

    public class SimpleNoteViewModel : ViewModelBase
    {
        public delegate void DeletingHandler();
        public event DeletingHandler Deleting;

        public SimpleNoteViewModel()
        {
            Delete = new Command(OnDeleteExecute);
            UpdateNamePH = new Command(OnUpdateNamePHExecute);
            UpdateTextPH = new Command(OnUpdateTextPHExecute);
            Note = new Note(-1, "", "", DateTime.MinValue, DateTime.MinValue);
        }

        public SimpleNoteViewModel(Note note)
        {
            Delete = new Command(OnDeleteExecute);
            UpdateNamePH = new Command(OnUpdateNamePHExecute);
            UpdateTextPH = new Command(OnUpdateTextPHExecute);
            Note = note;
        }

        #region Properties

        public Note Note
        {
            get { return GetValue<Note>(NoteProperty); }
            set 
            { 
                SetValue(NoteProperty, value);
                StringLastChanged = Note.StringLastChanged;
                if (value != null)
                    value.Timing += () => {
                        StringLastChanged = value.StringLastChanged; };
            }
        }
        public static readonly PropertyData NoteProperty = RegisterProperty(nameof(Note), typeof(Note), null);

        public string StringLastChanged
        {
            get { return GetValue<string>(StringLastChangedProperty); }
            set { SetValue(StringLastChangedProperty, value); }
        }
        public static readonly PropertyData StringLastChangedProperty = RegisterProperty(nameof(StringLastChanged), typeof(string), null);

        public Visibility VisibilityNamePH
        {
            get { return GetValue<Visibility>(VisibilityNamePHProperty); }
            set { SetValue(VisibilityNamePHProperty, value); }
        }
        public static readonly PropertyData VisibilityNamePHProperty = RegisterProperty(nameof(VisibilityNamePH), typeof(Visibility), null);

        public Visibility VisibilityTextPH
        {
            get { return GetValue<Visibility>(VisibilityTextPHProperty); }
            set { SetValue(VisibilityTextPHProperty, value); }
        }
        public static readonly PropertyData VisibilityTextPHProperty = RegisterProperty(nameof(VisibilityTextPH), typeof(Visibility), null);

        #endregion

        #region Commands

        public Command Delete { get; private set; }
        private void OnDeleteExecute()
        {
            Deleting?.Invoke();
        }

        public Command UpdateNamePH { get; private set; }
        private void OnUpdateNamePHExecute()
        {
            VisibilityNamePH = Note.Name == "" ? Visibility.Visible : Visibility.Collapsed;
        }

        public Command UpdateTextPH { get; private set; }
        private void OnUpdateTextPHExecute()
        {
            VisibilityTextPH = Note.Text == "" ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}