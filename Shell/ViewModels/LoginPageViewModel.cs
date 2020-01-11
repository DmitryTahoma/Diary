namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class LoginPageViewModel : ViewModelBase
    {
        private string password;

        public LoginPageViewModel()
        {
            UpdatePassword = new Command<PasswordBox>(OnUpdatePasswordExecute);
            LoginInputKeyDown = new Command<KeyEventArgs>(OnLoginInputKeyDownExecute);
            LoginInputKeyUp = new Command<PasswordBox>(OnLoginInputKeyUpExecute);
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

        #endregion

        #region Commands

        public Command<PasswordBox> UpdatePassword { get; private set; }
        private void OnUpdatePasswordExecute(PasswordBox passwordBox)
        {
            password = passwordBox.Password;
        }

        public Command<KeyEventArgs> LoginInputKeyDown { get; private set; }
        private void OnLoginInputKeyDownExecute(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                isEnter = true;
        }
        private bool isEnter = false;
        public Command<PasswordBox> LoginInputKeyUp { get; private set; }
        private void OnLoginInputKeyUpExecute(PasswordBox passwordBox)
        {
            if(isEnter)
                passwordBox.Focus();
        }

        #endregion
    }
}
