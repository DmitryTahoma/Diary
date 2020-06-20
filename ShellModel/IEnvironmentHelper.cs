using System.Collections.Generic;

namespace ShellModel
{
    public interface IEnvironmentHelper
    {
        void SaveSignData(string login, string password);

        bool CheckSignData();

        KeyValuePair<string, string> GetSignData();

        void ClearSignData();
    }
}