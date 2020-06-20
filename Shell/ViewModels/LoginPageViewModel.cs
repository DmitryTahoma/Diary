namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Controls;
    using Shell.Models;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    public class LoginPageViewModel : ViewModelBase
    {
        public delegate void NotRegisteredContainer();
        public delegate bool SignInContainer(string email, string password);

        public event NotRegisteredContainer IfNotRegistered;
        public event SignInContainer OnSignIn;

        public LoginPageViewModel()
        {
            LoginInputKeyDown = new Command<KeyEventArgs>(OnLoginInputKeyDownExecute);
            LoginInputKeyUp = new Command<RevealPasswordBox>(OnLoginInputKeyUpExecute);
            Register = new Command(OnRegisterExecute);
            SignIn = new Command(OnSignInExecute);
            PasswordBoxContext.Size = 40;
            PageFontSize = 40;
            IsRememberMe = false;
            PasswordBoxContext.OnEnterKeyUp += () => { OnSignIn?.Invoke(Login, PasswordBoxContext.GetPassword()); };
            ErrorTextVisibility = Visibility.Collapsed;

            KeyValuePair<string, string> signData = new EnvironmentHelperWpf().GetSignData();
            Login = signData.Key;
            PasswordBoxContext.SetPassword(signData.Value);
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

        public Visibility ErrorTextVisibility
        {
            get { return GetValue<Visibility>(ErrorTextVisibilityProperty); }
            set { SetValue(ErrorTextVisibilityProperty, value); }
        }
        public static readonly PropertyData ErrorTextVisibilityProperty = RegisterProperty(nameof(ErrorTextVisibility), typeof(Visibility), null);

        public bool IsRememberMe
        {
            get { return GetValue<bool>(IsRememberMeProperty); }
            set { SetValue(IsRememberMeProperty, value); }
        }
        public static readonly PropertyData IsRememberMeProperty = RegisterProperty(nameof(IsRememberMe), typeof(bool), null);

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
            if (isEnter)
                PasswordBoxContext.Focus();
        }

        public Command Register { get; private set; }
        private void OnRegisterExecute() => IfNotRegistered?.Invoke();

        public Command SignIn { get; private set; }
        private void OnSignInExecute()
        {
            if (OnSignIn != null)
                if (Login.Length < 6 || PasswordBoxContext.GetPassword().Length < 8)
                    ErrorTextVisibility = Visibility.Visible;
                else
                {
                    if (!OnSignIn.Invoke(Login, PasswordBoxContext.GetPassword()))
                        ErrorTextVisibility = Visibility.Visible;
                    else if (IsRememberMe)
                        new EnvironmentHelperWpf().SaveSignData(Login, PasswordBoxContext.GetPassword());
                }
        }

        #endregion
    }
}
