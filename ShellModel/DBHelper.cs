using ClientCore;
using ShellModel.Context;
using SocketSettings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShellModel
{
    public class DBHelper
    {
        public static string Login = "", Password = "";
        static ISocketSettings lastSettings = null;

        static object locker = new object();
        delegate object ProcessAction();

        Client client;
        int delay;

        public DBHelper(ISocketSettings settings)
        {
            client = new Client(settings);
            delay = settings.MlsOfDelay;
            lastSettings = settings;
        }

        public DBHelper(string path)
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings(path);
            delay = settings.MlsOfDelay;
            client = new Client(settings);
            lastSettings = settings;
        }

        private object DoLockedProcess(ProcessAction action)
        {
            object result = null;
            lock (locker)
            {
                bool isSended = false;
                Thread clientThread = new Thread(new ThreadStart(() =>
                {
                    result = action.Invoke();
                    isSended = true;
                }));
                clientThread.Start();

                int counter = 0;
                while (counter * 100 <= delay)
                {
                    Thread.Sleep(100);
                    counter++;
                    if (isSended)
                        break;
                }
                if (!isSended)
                    clientThread.Abort();
            }
            return result;
        }

        public bool Registration(string login, string password, string name)
        {
            object result = DoLockedProcess(() => 
            {
                return client.SendCommand("rnu", new string[] { login, password, name });
            });
            if (result != null)
                if (result is string)
                    if (bool.TryParse(result.ToString(), out bool x))
                        return x;
            return false;
        }

        public bool SignIn(string login, string password)
        {
            object result = DoLockedProcess(() =>
            {
                return client.SendCommand("clp", new string[] { login, password });
            });
            if (result != null)
                if (result is string)
                    if (bool.TryParse(result.ToString(), out bool x))
                        return x;
            return false;
        }

        public List<Note> GetDay(string login, string password, int day, int month, int year)
        {
            object result = DoLockedProcess(() => 
            {
                return client.SendCommand("gd", new string[] { login, password, day.ToString(), month.ToString(), year.ToString() });
            });
            if(result != null)
                if(result is string)
                    if(result.ToString() != "ae")
                    {
                        string[] r = StringsHelper.Split("\b<sgd>\b", result.ToString());
                        List<Note> res = new List<Note>();
                        for (int i = 0; i < r.Length; ++i)
                            if (Mission.IsStringMission(r[i]))
                                res.Add((ParagraphMission)Mission.CreateNew(r[i]));
                            else
                                res.Add(new Note(r[i]));
                        return res;
                    }
            throw new ArgumentException();
        }

        public int CreateNote(Note note)
        {
            object result = DoLockedProcess(() =>
            {
                return client.SendCommand("cnn", new string[] { Login, Password, note.Name, note.Text });
            });
            if(result != null)
                if (result is string res)                
                    if (int.TryParse(res, out int r))
                            return r;                
            throw new ArgumentException();
        }

        public bool SaveChanges(List<KeyValuePair<string, string[]>> changes)
        {
            foreach(KeyValuePair<string, string[]> change in changes)
            {
                List<string> args = new List<string>(new string[] { Login, Password });
                foreach (string arg in change.Value)
                    args.Add(arg);

                object result = DoLockedProcess(() => 
                {
                    return client.SendCommand(change.Key, args.ToArray());
                });
                bool s = false;
                if (result != null)
                    if (result is string)
                        if (bool.TryParse((string)result, out bool res))
                            if (res)
                                s = true;
                if (!s)
                    throw new ArgumentException();
            }
            return true;
        }

        public static async Task<List<Note>> GetDayAsync(string login, string password, int day, int month, int year)
        {
            return await Task<List<Note>>.Run(() =>
            {
                DBHelper dbHelper = new DBHelper(lastSettings);
                try
                {
                    return dbHelper.GetDay(login, password, day, month, year);
                }
                catch
                {
                    return new List<Note>();
                }
            });
        }

        public static async Task<int> CreateNoteAsync(Note note)
        {
            return await Task<int>.Run(() =>
            {
                DBHelper dbHelper = new DBHelper(lastSettings);
                try
                {
                    return dbHelper.CreateNote(note);
                }
                catch
                {
                    return -2;
                }
            });
        }

        public static async Task<bool> SaveChangesAsync(List<KeyValuePair<string, string[]>> changes)
        {
            return await Task<bool>.Run(() =>
            {
                DBHelper dbHelper = new DBHelper(lastSettings);
                try
                {
                    return dbHelper.SaveChanges(changes);
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}