namespace Shell.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using ShellModel;

    public class MainWindowVewModel : ViewModelBase
    {
        DBHelper dbHelper;

        public MainWindowVewModel()
        {
            LoginPageContext = new LoginPageViewModel();
            LoginPageContext.IfNotRegistered += () =>  { SelectedTabItemId = 1; };

            RegistrationPageContext = new RegistrationPageViewModel();
            RegistrationPageContext.BackToSignIn += () => { SelectedTabItemId = 0; };

            dbHelper = new DBHelper(@"D:\Projects\Portfolio\Diary\packages\ss.bin");
            LoginPageContext.OnSignIn += (e, p) => 
            {
                bool result = dbHelper.SignIn(e, p);
                if (result)
                {
                    DBHelper.Login = e;
                    DBHelper.Password = p;
                    SelectedTabItemId = 2;
                }
                return result;
            };
            RegistrationPageContext.OnSignUp += (e, p, n) => 
            {
                if(dbHelper.Registration(e, p, n))
                {
                    DBHelper.Login = e;
                    DBHelper.Password = p;
                    DBHelper.IsNewUser = true;
                    SelectedTabItemId = 2;
                    return true;
                }
                SelectedTabItemId = 0;
                return false;
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
