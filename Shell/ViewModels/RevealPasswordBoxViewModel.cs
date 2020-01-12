namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class RevealPasswordBoxViewModel : ViewModelBase
    {
        public delegate void OnEnterKeyUpContainer();
        public event OnEnterKeyUpContainer OnEnterKeyUp;

        private string password = "";
        public RevealPasswordBoxViewModel()
        {
            UpdatePassword = new Command<FrameworkElement>(OnUpdatePasswordExecute);
            UpdateBoxesData = new Command<FrameworkElement>(OnUpdateBoxesDataExecute);
            ClickCheckBox = new Command<FrameworkElement>(OnClickCheckBoxExecute);
            BoxKeyUp = new Command<KeyEventArgs>(OnBoxKeyUpExecute);
        }

        #region Properties

        public bool IsShown
        {
            get { return GetValue<bool>(IsShownProperty); }
            set { SetValue(IsShownProperty, value); OnUpdateIsShown(); }
        }
        public static readonly PropertyData IsShownProperty = RegisterProperty(nameof(IsShown), typeof(bool), false);

        public Visibility TextBoxVisibility
        {
            get { return GetValue<Visibility>(TextBoxVisibilityProperty); }
            set { SetValue(TextBoxVisibilityProperty, value); }
        }
        public static readonly PropertyData TextBoxVisibilityProperty = RegisterProperty(nameof(TextBoxVisibility), typeof(Visibility), Visibility.Collapsed);

        public Visibility PasswordBoxVisibility
        {
            get { return GetValue<Visibility>(PasswordBoxVisibilityProperty); }
            set { SetValue(PasswordBoxVisibilityProperty, value); }
        }
        public static readonly PropertyData PasswordBoxVisibilityProperty = RegisterProperty(nameof(PasswordBoxVisibility), typeof(Visibility), Visibility.Visible);

        public double PasswordFontSize
        {
            get { return GetValue<double>(PasswordFontSizeProperty); }
            set { SetValue(PasswordFontSizeProperty, value); }
        }
        public static readonly PropertyData PasswordFontSizeProperty = RegisterProperty(nameof(PasswordFontSize), typeof(double));
        
        public double HintFontSize
        {
            get { return GetValue<double>(HintFontSizeProperty); }
            set { SetValue(HintFontSizeProperty, value); }
        }
        public static readonly PropertyData HintFontSizeProperty = RegisterProperty(nameof(HintFontSize), typeof(double));

        #endregion

        #region Commands

        public Command<FrameworkElement> UpdatePassword { get; private set; }
        private void OnUpdatePasswordExecute(FrameworkElement sender)
        {
            if (IsShown && sender is TextBox)
                password = ((TextBox)sender).Text;
            else if (!IsShown && sender is PasswordBox)
                password = ((PasswordBox)sender).Password;
        }

        public Command<FrameworkElement> UpdateBoxesData { get; private set; }
        private void OnUpdateBoxesDataExecute(FrameworkElement element)
        {
            if (IsShown && element is PasswordBox)
                ((PasswordBox)element).Password = password;
            else if(!IsShown && element is TextBox)
                ((TextBox)element).Text = password;
        }

        public Command<FrameworkElement> ClickCheckBox { get; private set; }
        private void OnClickCheckBoxExecute(FrameworkElement element)
        {
            if (IsShown && element is TextBox)
            {
                ((TextBox)element).Focus();
                ((TextBox)element).SelectionStart = ((TextBox)element).Text.Length;
            }
            else if (!IsShown && element is PasswordBox)
            {
                ((PasswordBox)element).Focus();
                ((PasswordBox)element).GetType()
                    .GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke((PasswordBox)element, new object[] { ((PasswordBox)element).Password.Length, 0 });
            }
        }

        public Command<KeyEventArgs> BoxKeyUp { get; private set; }
        private void OnBoxKeyUpExecute(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OnEnterKeyUp();
        }

        #endregion

        private void OnUpdateIsShown()
        {
            TextBoxVisibility = IsShown ? Visibility.Visible : Visibility.Collapsed;
            PasswordBoxVisibility = IsShown ? Visibility.Collapsed : Visibility.Visible;
        }

        public string GetPassword() => password;
    }
}