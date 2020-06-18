using Microsoft.Win32;
using ShellModel;
using System;

namespace Shell.Models
{
    public class EnvironmentHelperWpf : IEnvironmentHelper
    {
        public void SaveSignData(string login, string password)
        {
            RegistryKey registryKey = Registry.CurrentUser;
            RegistryKey diaryEHData = registryKey.CreateSubKey("DiaryEHData", true);
            diaryEHData.SetValue("login", login);
            diaryEHData.SetValue("password", password);
            diaryEHData.SetValue("time", DateTime.Now.ToString());
        }
    }
}