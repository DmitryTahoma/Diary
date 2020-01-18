namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;

    public class RegistrationPageViewModel : ViewModelBase
    {
        public delegate void BackToSignInContainer();
        public delegate bool SignUpContainer(string email, string password, string name);

        public event BackToSignInContainer BackToSignIn;
        public event SignUpContainer OnSignUp;

        public RegistrationPageViewModel()
        {
            DoBack = new Command(OnDoBackExecute);
            SignUp = new Command(OnSignUpExecute);
            PasswordBoxContext = new RevealPasswordBoxViewModel();
            ConfirmPasswordBoxContext = new RevealPasswordBoxViewModel();
            PageFontSize = 25;
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
            set { SetValue(UserNameProperty, value); }
        }
        public static readonly PropertyData UserNameProperty = RegisterProperty(nameof(UserName), typeof(string), "");

        public string Email
        {
            get { return GetValue<string>(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }
        public static readonly PropertyData EmailProperty = RegisterProperty(nameof(Email), typeof(string), "");

        #endregion

        #region Commands

        public Command DoBack { private set; get; }
        private void OnDoBackExecute() => BackToSignIn?.Invoke();

        public Command SignUp { private set; get; }
        private void OnSignUpExecute() => OnSignUp?.Invoke(Email, PasswordBoxContext.GetPassword(), UserName);

        #endregion

        public void OnPageFontSizeSet()
        {
            PasswordBoxContext.Size = PageFontSize;
            ConfirmPasswordBoxContext.Size = PageFontSize;
        }
    }
}
