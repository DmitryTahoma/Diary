namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class RegistrationPageViewModel : ViewModelBase
    {
        private bool dynamicValidationUserName = false, dynamicValidationEmail = false, dynamicValidationPassword = false, dynamicValidationConfirm = false;

        public delegate void BackToSignInContainer();
        public delegate bool SignUpContainer(string email, string password, string name);

        public event BackToSignInContainer BackToSignIn;
        public event SignUpContainer OnSignUp;

        public RegistrationPageViewModel()
        {
            DoBack = new Command(OnDoBackExecute);
            SignUp = new Command(OnSignUpExecute);
            NameKeyDown = new Command<KeyEventArgs>(OnNameKeyDownExecute);
            NameKeyUp = new Command<TextBox>(OnNameKeyUpExecute);
            EmailKeyUp = new Command<KeyEventArgs>(OnEmailKeyUpExecute);
            PasswordBoxContext = new RevealPasswordBoxViewModel();
            PasswordBoxContext.OnEnterKeyUp += () => { ConfirmPasswordBoxContext.Focus(); };
            PasswordBoxContext.OnEscapeKeyUp += OnDoBackExecute;
            ConfirmPasswordBoxContext = new RevealPasswordBoxViewModel();
            ConfirmPasswordBoxContext.OnEnterKeyUp += OnSignUpExecute;
            ConfirmPasswordBoxContext.OnEscapeKeyUp += OnDoBackExecute;
            PageFontSize = 25;
            PasswordBoxContext.PasswordChanged += PasswordSet;
            ConfirmPasswordBoxContext.PasswordChanged += ConfirmPasswordSet;
        }

        #region Properties

        public RevealPasswordBoxViewModel PasswordBoxContext
        {
            get { return GetValue<RevealPasswordBoxViewModel>(PasswordBoxContextProperty); }
            set { SetValue(PasswordBoxContextProperty, value); }
        }
        public static readonly PropertyData PasswordBoxContextProperty = RegisterProperty(nameof(PasswordBoxContext), typeof(RevealPasswordBoxViewModel));

        public RevealPasswordBoxViewModel ConfirmPasswordBoxContext
        {
            get { return GetValue<RevealPasswordBoxViewModel>(ConfirmPasswordBoxContextProperty); }
            set { SetValue(ConfirmPasswordBoxContextProperty, value); }
        }
        public static readonly PropertyData ConfirmPasswordBoxContextProperty = RegisterProperty(nameof(ConfirmPasswordBoxContext), typeof(RevealPasswordBoxViewModel));

        public double PageFontSize
        {
            get { return GetValue<double>(PageFontSizeProperty); }
            set { SetValue(PageFontSizeProperty, value); OnPageFontSizeSet(); }
        }
        public static readonly PropertyData PageFontSizeProperty = RegisterProperty(nameof(PageFontSize), typeof(double));

        public string UserName
        {
            get { return GetValue<string>(UserNameProperty); }
            set { SetValue(UserNameProperty, value); UserNameSet(); }
        }
        public static readonly PropertyData UserNameProperty = RegisterProperty(nameof(UserName), typeof(string), "");

        public string Email
        {
            get { return GetValue<string>(EmailProperty); }
            set { SetValue(EmailProperty, value); EmailSet(); }
        }
        public static readonly PropertyData EmailProperty = RegisterProperty(nameof(Email), typeof(string), "");

        public string EtNameBeetween
        {
            get { return GetValue<string>(EtNameBeetweenProperty); }
            set { SetValue(EtNameBeetweenProperty, value); }
        }
        public static readonly PropertyData EtNameBeetweenProperty = RegisterProperty(nameof(EtNameBeetween), typeof(string), "Name must be between 2 and 64");

        public string EtEmailWrong
        {
            get { return GetValue<string>(EtEmailWrongProperty); }
            set { SetValue(EtEmailWrongProperty, value); }
        }
        public static readonly PropertyData EtEmailWrongProperty = RegisterProperty(nameof(EtEmailWrong), typeof(string), "Invalid email adsress");

        public string EtPasswordBetween
        {
            get { return GetValue<string>(EtPasswordBetweenProperty); }
            set { SetValue(EtPasswordBetweenProperty, value); }
        }
        public static readonly PropertyData EtPasswordBetweenProperty = RegisterProperty(nameof(EtPasswordBetween), typeof(string), "Password must be between 8 and 64");

        public string EtPasswordConfirm
        {
            get { return GetValue<string>(EtPasswordConfirmProperty); }
            set { SetValue(EtPasswordConfirmProperty, value); }
        }
        public static readonly PropertyData EtPasswordConfirmProperty = RegisterProperty(nameof(EtPasswordConfirm), typeof(string), "Passwords do not match");

        public Visibility EtNameBetweenVisibility
        {
            get { return GetValue<Visibility>(EtNameBetweenVisibilityProperty); }
            set { SetValue(EtNameBetweenVisibilityProperty, value); }
        }
        public static readonly PropertyData EtNameBetweenVisibilityProperty = RegisterProperty(nameof(EtNameBetweenVisibility), typeof(Visibility), Visibility.Collapsed);

        public Visibility EtEmailWrongVisibility
        {
            get { return GetValue<Visibility>(EtEmailWrongVisibilityProperty); }
            set { SetValue(EtEmailWrongVisibilityProperty, value); }
        }
        public static readonly PropertyData EtEmailWrongVisibilityProperty = RegisterProperty(nameof(EtEmailWrongVisibility), typeof(Visibility), Visibility.Collapsed);

        public Visibility EtPasswordBetweenVisibility
        {
            get { return GetValue<Visibility>(EtPasswordBetweenVisibilityProperty); }
            set { SetValue(EtPasswordBetweenVisibilityProperty, value); }
        }
        public static readonly PropertyData EtPasswordBetweenVisibilityProperty = RegisterProperty(nameof(EtPasswordBetweenVisibility), typeof(Visibility), Visibility.Collapsed);

        public Visibility EtPasswordConfirmVisibility
        {
            get { return GetValue<Visibility>(EtPasswordConfirmVisibilityProperty); }
            set { SetValue(EtPasswordConfirmVisibilityProperty, value); }
        }
        public static readonly PropertyData EtPasswordConfirmVisibilityProperty = RegisterProperty(nameof(EtPasswordConfirmVisibility), typeof(Visibility), Visibility.Collapsed);

        #endregion

        #region Commands

        public Command DoBack { private set; get; }
        private void OnDoBackExecute()
        {
            UserName = "";
            Email = "";
            PasswordBoxContext.ClearPassword();
            ConfirmPasswordBoxContext.ClearPassword();
            BackToSignIn?.Invoke();
            EtNameBetweenVisibility = Visibility.Hidden;
            EtEmailWrongVisibility = Visibility.Hidden;
            EtPasswordBetweenVisibility = Visibility.Hidden;
            EtPasswordConfirmVisibility = Visibility.Hidden;
            dynamicValidationUserName = false;
            dynamicValidationEmail = false;
            dynamicValidationPassword = false;
            dynamicValidationConfirm = false;
        }

        public Command SignUp { private set; get; }
        private void OnSignUpExecute()
        {
            if(Validate())
                OnSignUp?.Invoke(Email, PasswordBoxContext.GetPassword(), UserName);
        }

        bool isEnter_ntb = false;
        public Command<KeyEventArgs> NameKeyDown { get; private set; }
        private void OnNameKeyDownExecute(KeyEventArgs e)
        {
            isEnter_ntb = e.Key == Key.Enter;
            if (e.Key == Key.Escape)
                OnDoBackExecute();
        }

        public Command<TextBox> NameKeyUp { get; private set; }
        private void OnNameKeyUpExecute(TextBox etb)
        {
            if (isEnter_ntb)
                etb.Focus();
        }

        public Command<KeyEventArgs> EmailKeyUp { get; private set; }
        private void OnEmailKeyUpExecute(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PasswordBoxContext.Focus();
            else if (e.Key == Key.Escape)
                OnDoBackExecute();
        }

        #endregion

        private void UserNameSet()
        {
            if (dynamicValidationUserName)
                if (UserName.Length < 2 || UserName.Length > 64)
                    EtNameBetweenVisibility = Visibility.Visible;
                else
                    EtNameBetweenVisibility = Visibility.Collapsed;
            else if (UserName.Length > 1 && UserName.Length < 63)
                dynamicValidationUserName = true;
        }

        private void EmailSet()
        {
            if(dynamicValidationEmail)
            {
                string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
                if (!Regex.IsMatch(Email, pattern))
                    EtEmailWrongVisibility = Visibility.Visible;
                else                
                    EtEmailWrongVisibility = Visibility.Collapsed;
            }
        }

        private void PasswordSet()
        {
            if (dynamicValidationPassword)
                if (PasswordBoxContext.GetPassword().Length < 8 || PasswordBoxContext.GetPassword().Length > 64)
                    EtPasswordBetweenVisibility = Visibility.Visible;
                else
                    EtPasswordBetweenVisibility = Visibility.Collapsed;
            else if (PasswordBoxContext.GetPassword().Length > 7 && PasswordBoxContext.GetPassword().Length < 63)
                dynamicValidationPassword = true;
        }

        private void ConfirmPasswordSet()
        {
            if (dynamicValidationConfirm && dynamicValidationPassword)
                if (PasswordBoxContext.GetPassword() != ConfirmPasswordBoxContext.GetPassword())
                    EtPasswordConfirmVisibility = Visibility.Visible;
                else
                    EtPasswordConfirmVisibility = Visibility.Collapsed;
            else if (PasswordBoxContext.GetPassword() == ConfirmPasswordBoxContext.GetPassword())
                dynamicValidationConfirm = true;
        }

        public void OnPageFontSizeSet()
        {
            PasswordBoxContext.Size = PageFontSize;
            ConfirmPasswordBoxContext.Size = PageFontSize;
        }

        public bool Validate()
        {
            dynamicValidationUserName = true;
            dynamicValidationEmail = true;
            dynamicValidationPassword = true;
            dynamicValidationConfirm = true;
            bool result = UserName.Length > 1 && UserName.Length < 65;
            if (!result)
            {
                EtNameBetweenVisibility = Visibility.Visible;
            }
            else
            {
                EtNameBetweenVisibility = Visibility.Collapsed;
            }

            string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
            if(!Regex.IsMatch(Email, pattern))
            {
                EtEmailWrongVisibility = Visibility.Visible;
                result = false;
            }
            else
            {
                EtEmailWrongVisibility = Visibility.Collapsed;
            }

            if (PasswordBoxContext.GetPassword().Length < 8 || PasswordBoxContext.GetPassword().Length > 64)
            {
                EtPasswordBetweenVisibility = Visibility.Visible;
                result = false;
            }
            else
            {
                EtPasswordBetweenVisibility = Visibility.Collapsed;
            }

            if (PasswordBoxContext.GetPassword() != ConfirmPasswordBoxContext.GetPassword())
            {
                EtPasswordConfirmVisibility = Visibility.Visible;
                result = false;
            }
            else
            {
                EtPasswordConfirmVisibility = Visibility.Collapsed;
            }

            return result;
        }
    }
}
