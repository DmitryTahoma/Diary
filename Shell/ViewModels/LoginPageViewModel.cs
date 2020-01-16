namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Controls;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class LoginPageViewModel : ViewModelBase
    {
        public delegate void NotRegisteredContainer();
        public event NotRegisteredContainer IfNotRegistered;

        public LoginPageViewModel()
        {
            LoginInputKeyDown = new Command<KeyEventArgs>(OnLoginInputKeyDownExecute);
            LoginInputKeyUp = new Command<RevealPasswordBox>(OnLoginInputKeyUpExecute);
            Register = new Command(OnRegisterExecute);
            PasswordBoxContext.Size = 40;
            PageFontSize = 40;
        }

        #region Properties

        public string LoginText
        {
            get { return GetValue<string>(LoginTextProperty); }
            set { SetValue(LoginTextProperty, value); }
        }
        public static readonly PropertyData LoginTextProperty = RegisterProperty(nameof(LoginText), typeof(string), "Login");

        public string PasswordText
        {
            get { return GetValue<string>(PasswordTextProperty); }
            set { SetValue(PasswordTextProperty, value); }
        }
        public static readonly PropertyData PasswordTextProperty = RegisterProperty(nameof(PasswordText), typeof(string), "Password");

        public string Login
        {
            get { return GetValue<string>(LoginProperty); }
            set { SetValue(LoginProperty, value); }
        }
        public static readonly PropertyData LoginProperty = RegisterProperty(nameof(Login), typeof(string), "");

        public double PageFontSize
        {
            get { return GetValue<double>(PageFontSizeProperty); }
            set { SetValue(PageFontSizeProperty, value); }
        }
        public static readonly PropertyData PageFontSizeProperty = RegisterProperty(nameof(PageFontSize), typeof(double), 40);

        public RevealPasswordBoxViewModel PasswordBoxContext
        {
            get { return GetValue<RevealPasswordBoxViewModel>(PasswordBoxContextProperty); }
            set { SetValue(PasswordBoxContextProperty, value); }
        }
        public static readonly PropertyData PasswordBoxContextProperty = RegisterProperty(nameof(PasswordBoxContext), typeof(RevealPasswordBoxViewModel), new RevealPasswordBoxViewModel());

        public double HalfPageFontSize
        {
            get { return PageFontSize / 2; }
        }

        #endregion

        #region Commands

        public Command<KeyEventArgs> LoginInputKeyDown { get; private set; }
        private void OnLoginInputKeyDownExecute(KeyEventArgs e)
        {
            isEnter = e.Key == Key.Enter;
        }
        private bool isEnter = false;
        public Command<RevealPasswordBox> LoginInputKeyUp { get; private set; }
        private void OnLoginInputKeyUpExecute(RevealPasswordBox passwordBox)
        {
            if(isEnter)
                passwordBox.Focus();
        }

        public Command Register { get; private set; }
        private void OnRegisterExecute() => IfNotRegistered?.Invoke();

        #endregion
    }
}
