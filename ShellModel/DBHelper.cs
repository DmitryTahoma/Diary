﻿using ClientCore;
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
        public static bool IsNewUser = false;

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
                return client.SendCommand("cnn", new string[] { Login, Password, note.Name, note.Text, note.Created.Day.ToString(), note.Created.Month.ToString(), note.Created.Year.ToString() });
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

        public bool RemoveNoteCascade(Note note)
        {
            object result = DoLockedProcess(() => { return client.SendCommand("rnc", new string[] { Login, Password, note.Id.ToString() }); });
            if (result != null)
                if (result is string res)
                    if (bool.TryParse(res, out bool r))
                        if (r)
                            return true;
            return false;
        }

        public int[] CreateParagraphMission(ParagraphMission paragraphMission)
        {
            object result = DoLockedProcess(() => { return client.SendCommand("cnpm", new string[] { Login, Password, paragraphMission.Name, paragraphMission.Text, paragraphMission.Created.Year.ToString(), paragraphMission.Created.Month.ToString(), paragraphMission.Created.Day.ToString() }); });
            if (result != null)
                if (result is string res)
                    if(res.Split('|').Length == 4)
                        if (int.TryParse(res.Split('|')[0], out int r1))
                            if (int.TryParse(res.Split('|')[1], out int r2))
                                if (int.TryParse(res.Split('|')[2], out int r3))
                                    if (int.TryParse(res.Split('|')[3], out int r4))
                                        if(r1 > 0 && r2 > 0 && r3 > 0 && r4 > 0)
                                            return new int[] { r1, r2, r3, r4 };
            throw new ArgumentException();
        }

        public int AddPointToParagraphMission(ParagraphMission paragraphMission, Point point)
        {
            object result = DoLockedProcess(() => { return client.SendCommand("aptpm", new string[] { Login, Password, paragraphMission.Id.ToString(), point.Text }); });
            if (result != null)
                if (result is string res)
                    if (int.TryParse(res, out int r))
                        if (r > 0)
                            return r;
            throw new ArgumentException();
        }

        public bool RemovePoint(Point point)
        {
            object result = DoLockedProcess(() => { return client.SendCommand("rp", new string[] { Login, Password, point.Id.ToString() }); });
            if (result != null)
                if (result is string res)
                    if (bool.TryParse(res, out bool r))
                        return r;
            throw new ArgumentException();
        }

        public bool SetCheckedPoint(Point point, bool isChecked)
        {
            object result = DoLockedProcess(() => { return client.SendCommand("scp", new string[] { Login, Password, point.Id.ToString(), isChecked.ToString() }); });
            if (result != null)
                if (result is string res)
                    if (bool.TryParse(res, out bool _))
                        return true;
            throw new ArgumentException();
        }

        public Note DuplicateNote(Note note, DateTime dateNewCreated)
        {
            object result = DoLockedProcess(() => { return client.SendCommand("dn", new string[] { DBHelper.Login, DBHelper.Password, note.Id.ToString(), dateNewCreated.Day.ToString(), dateNewCreated.Month.ToString(), dateNewCreated.Year.ToString() }); });
            if (result != null)
                if (result is string res)
                    if (int.TryParse(res, out int r))
                        return new Note(r, 0, note.Name, note.Text, dateNewCreated, DateTime.Now, true);
            throw new ArgumentException();
        }

        public ParagraphMission DuplicateParagraphMission(ParagraphMission paragraphMission, DateTime dateNewCreated)
        {
            object result = DoLockedProcess(() => { return client.SendCommand("dpm", new string[] { DBHelper.Login, DBHelper.Password, paragraphMission.Id.ToString(), dateNewCreated.Day.ToString(), dateNewCreated.Month.ToString(), dateNewCreated.Year.ToString() }); });
            if(result != null)
                if(result is string res)
                {
                    string[] strIds = res.Split('|');
                    int[] ids = new int[strIds.Length];
                    for (int i = 0; i < ids.Length; ++i)
                        ids[i] = int.Parse(strIds[i]);
                    List<Point> items = new List<Point>();
                    for (int i = 0; i < paragraphMission.Paragraph.Items.Count; ++i)
                        items.Add(new Point(ids[i + 4], paragraphMission.Paragraph.Items[i].Text, paragraphMission.Paragraph.Items[i].IsChecked));

                    return new ParagraphMission(ids[2], new Paragraph(ids[3], items), ids[1], ids[0], 1, paragraphMission.Name, paragraphMission.Text, dateNewCreated, DateTime.Now, DateTime.MinValue, DateTime.MaxValue, true);
                }
            throw new ArgumentException();
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

        public static int CreateNoteStatic(Note note)
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

        public static bool RemoveNoteCascadeStatic(Note note)
        {
            DBHelper helper = new DBHelper(lastSettings);
            return helper.RemoveNoteCascade(note);
        }

        public static int[] CreateParagraphMissionStatic(ParagraphMission paragraphMission)
        {
            DBHelper helper = new DBHelper(lastSettings);
            try
            {
                return helper.CreateParagraphMission(paragraphMission);
            }
            catch
            {
                return new int[] { -3, -3, -3, -3 };
            }
        }

        public static int AddPointToParagraphMissionStatic(ParagraphMission paragraphMission, Point point) 
            => new DBHelper(lastSettings).AddPointToParagraphMission(paragraphMission, point);

        public static bool RemovePointStatic(Point point)
            => new DBHelper(lastSettings).RemovePoint(point);

        public static bool SetCheckedPointStatic(Point point, bool isChecked)
            => new DBHelper(lastSettings).SetCheckedPoint(point, isChecked);

        public static Note DuplicateNoteStatic(Note note, DateTime dateNewCreated)
            => new DBHelper(lastSettings).DuplicateNote(note, dateNewCreated);

        public static ParagraphMission DuplicateParagraphMissionStatic(ParagraphMission paragraphMission, DateTime dateNewCreated)
            => new DBHelper(lastSettings).DuplicateParagraphMission(paragraphMission, dateNewCreated);
    }
}