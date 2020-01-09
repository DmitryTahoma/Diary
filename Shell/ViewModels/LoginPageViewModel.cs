namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Models.Helpers;
    using System.Windows.Controls;

    public class LoginPageViewModel : ViewModelBase
    {
        private string password;

        public LoginPageViewModel()
        {
            UpdateFontSize = new Command<Page>(OnUpdateFontSizeExecute);
            UpdatePassword = new Command<PasswordBox>(OnUpdatePasswordExecute);
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

        public Command<Page> UpdateFontSize { get; private set; }
        private void OnUpdateFontSizeExecute(Page loginPage)
        {
            if (LoginPageScaleHelper.IsEmpty())
                LoginPageScaleHelper.FindAllControls(loginPage);

            double newSize = loginPage.ActualHeight / 10 + 10;
            PageFontSize = newSize;
            LoginPageScaleHelper.SetFontSize(newSize);
        }

        public Command<PasswordBox> UpdatePassword { get; private set; }
        private void OnUpdatePasswordExecute(PasswordBox passwordBox)
        {
            password = passwordBox.Password;
        }

        #endregion
    }
}
