using Microsoft.Win32;
using ShellModel;
using System;
using System.Collections.Generic;

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

        public bool CheckSignData()
        {
            RegistryKey registryKey = Registry.CurrentUser;
            RegistryKey diaryEHData = registryKey.CreateSubKey("DiaryEHData", false);
            string login = diaryEHData.GetValue("login").ToString();
            string password = diaryEHData.GetValue("password").ToString();
            DateTime time = DateTime.Parse(diaryEHData.GetValue("time").ToString());

            if(login == "" || password == "") 
                return false;    
            if(time < DateTime.Now.AddDays(-30) || time > DateTime.Now)
            {
                SaveSignData("", "");
                return false;
            }

            return true;
        }

        public KeyValuePair<string, string> GetSignData()
        {
            if (!CheckSignData())
                return new KeyValuePair<string, string>("", "");

            RegistryKey registryKey = Registry.CurrentUser;
            RegistryKey diaryEHData = registryKey.CreateSubKey("DiaryEHData", false);
            string login = diaryEHData.GetValue("login").ToString();
            string password = diaryEHData.GetValue("password").ToString();

            return new KeyValuePair<string, string>(login, password);
        }
    }
}