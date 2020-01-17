namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Pages;
    using System.Windows.Controls;

    public class MainWindowVewModel : ViewModelBase
    {
        public MainWindowVewModel()
        {
            CurrentPage = new LoginPage();
            LoginPageContext = ((LoginPage)CurrentPage).DataContext;
            LoginPageContext.IfNotRegistered += () => 
            {
                CurrentPage = new RegistrationPage();
            };
            RegistrationPageContext = new RegistrationPageViewModel();
        }

        #region Properties

        public LoginPageViewModel LoginPageContext { set; get; }

        public RegistrationPageViewModel RegistrationPageContext { set; get; }

        public Page CurrentPage
        {
            get { return GetValue<Page>(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }
        public static readonly PropertyData CurrentPageProperty = RegisterProperty(nameof(CurrentPage), typeof(Page));

        #endregion

        #region Commands

        #endregion
    }
}
