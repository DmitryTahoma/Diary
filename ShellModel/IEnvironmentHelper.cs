namespace ShellModel
{
    public interface IEnvironmentHelper
    {
        void SaveSignData(string login, string password);

        bool CheckSignData();
    }
}