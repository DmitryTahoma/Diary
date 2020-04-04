namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using System.Windows;

    public class CheckTextBoxViewModel : ViewModelBase
    {
        public delegate void VoidHandler();
        public event VoidHandler OnDeleteMe;

        public CheckTextBoxViewModel()
        {
            ContextPoint = new ShellModel.Context.Point("", false);
            DeleteMe = new Command(OnDeleteMeExecute);
        }

        #region Properties

        public bool IsChecked
        {
            get { return GetValue<bool>(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); Decorations = IsChecked ? TextDecorations.Strikethrough : null; ContextPoint.IsChecked = value; }
        }
        public static readonly PropertyData IsCheckedProperty = RegisterProperty(nameof(IsChecked), typeof(bool), false);

        public TextDecorationCollection Decorations
        {
            get { return GetValue<TextDecorationCollection>(DecorationProperty); }
            set { SetValue(DecorationProperty, value); }
        }
        public static readonly PropertyData DecorationProperty = RegisterProperty(nameof(Decorations), typeof(TextDecorationCollection), null);

        public string Text
        {
            get { return GetValue<string>(TextProperty); }
            set { SetValue(TextProperty, value); ContextPoint.Text = value; }
        }
        public static readonly PropertyData TextProperty = RegisterProperty(nameof(Text), typeof(string), null);

        private ShellModel.Context.Point contextPoint = null;
        public ShellModel.Context.Point ContextPoint
        {
            set
            {
                contextPoint = value;
                Text = contextPoint.Text;
                IsChecked = contextPoint.IsChecked;
            }
            get => contextPoint;
        }

        #endregion

        #region Commands

        public Command DeleteMe { get; private set; }
        private void OnDeleteMeExecute()
        {
            OnDeleteMe?.Invoke();
        }

        #endregion
    }
}