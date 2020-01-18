namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Shell.Models;

    public class MainWindowVewModel : ViewModelBase
    {
        DBHelper dbHelper;

        public MainWindowVewModel()
        {
            LoginPageContext = new LoginPageViewModel();
            LoginPageContext.IfNotRegistered += () =>  { SelectedTabItemId = 1; };

            RegistrationPageContext = new RegistrationPageViewModel();
            RegistrationPageContext.BackToSignIn += () => { SelectedTabItemId = 0; };

            dbHelper = new DBHelper(new SocketSettings.SocketSettings("192.168.0.105", 11221, new int[] { 11222, 11224, 12550 }, 3000));
            LoginPageContext.OnSignIn += (e, p) => 
            {
                bool result = dbHelper.SignIn(e, p);
                if (result)
                    SelectedTabItemId = 2;
                return result;
            };
            RegistrationPageContext.OnSignUp += (e, p, n) => 
            {
                SelectedTabItemId = 0;
                return dbHelper.Registration(e, p, n);
            };
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
