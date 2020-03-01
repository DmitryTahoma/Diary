using ClientCore;
using ShellModel.Context;
using SocketSettings;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ShellModel
{
    public class DBHelper
    {
        object locker = new object();
        delegate object ProcessAction();

        Client client;
        int delay;

        public DBHelper(ISocketSettings settings)
        {
            client = new Client(settings);
            delay = settings.MlsOfDelay;
        }

        public DBHelper(string path)
        {
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings(path);
            delay = settings.MlsOfDelay;
            client = new Client(settings);
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
    }
}
