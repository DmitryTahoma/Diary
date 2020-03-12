namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using ShellModel.Context;
    using System;

    public class SimpleNoteViewModel : ViewModelBase
    {
        public delegate void DeletingHandler();
        public event DeletingHandler Deleting;

        public SimpleNoteViewModel()
        {
            Note = new Note(-1, "", "", DateTime.MinValue, DateTime.MinValue);
            Delete = new Command(OnDeleteExecute);
        }

        public SimpleNoteViewModel(Note note)
        {
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

        #endregion

        #region Commands

        public Command Delete { get; private set; }
        private void OnDeleteExecute()
        {
            Deleting?.Invoke();
        }

        #endregion
    }
}