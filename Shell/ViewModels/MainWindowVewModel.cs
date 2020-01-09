namespace Shell.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class MainWindowVewModel : ViewModelBase
    {
        public MainWindowVewModel()
        {

        }

        #region Properties

        #endregion

        #region Commands

        #endregion

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }
    }
}
