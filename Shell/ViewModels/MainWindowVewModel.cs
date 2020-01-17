namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using System.Windows.Controls;

    public class MainWindowVewModel : ViewModelBase
    {
        public MainWindowVewModel()
        {
            LoginPageContext = new LoginPageViewModel();
            LoginPageContext.IfNotRegistered += () =>  { SelectedTabItemId = 1; };

            RegistrationPageContext = new RegistrationPageViewModel();
            RegistrationPageContext.BackToSignIn += () => { SelectedTabItemId = 0; };
        }

        #region Properties

        public LoginPageViewModel LoginPageContext { set; get; }

        public RegistrationPageViewModel RegistrationPageContext { set; get; }

        public int SelectedTabItemId
        {
            get { return GetValue<int>(SelectedTabItemIdProperty); }
            set { SetValue(SelectedTabItemIdProperty, value); }
        }
        public static readonly PropertyData SelectedTabItemIdProperty = RegisterProperty(nameof(SelectedTabItemId), typeof(int), 0);

        #endregion

        #region Commands

        #endregion
    }
}
