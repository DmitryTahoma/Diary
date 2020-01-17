namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;

    public class RegistrationPageViewModel : ViewModelBase
    {
        public delegate void BackToSignInContainer();
        public event BackToSignInContainer BackToSignIn;

        public RegistrationPageViewModel()
        {
            DoBack = new Command(OnDoBackExecute);
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

        #endregion

        #region Commands

        public Command DoBack { private set; get; }
        private void OnDoBackExecute() => BackToSignIn?.Invoke();

        #endregion

        public void OnPageFontSizeSet()
        {
            PasswordBoxContext.Size = PageFontSize;
            ConfirmPasswordBoxContext.Size = PageFontSize;
        }
    }
}
