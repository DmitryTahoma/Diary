using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;
using System.Linq;

namespace ServerRealization.Test
{
    public partial class ServerCommandsTest
    {
        private static string correctLogin = "Alex92", correctPassword = "pass1234";

        private void GenerateNotes(string login, string password, int count, int dispDays)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            int userId = -1;
            try
            {
                userId = DBContext.Users.Where(x => x.Login == login && x.Password == password).First().Id;
            }
            catch { return; }

            for (int i = 0; i < count; ++i)
            {
                DateTime created = DateTime.Now.AddDays(dispDays * (random.Next() % 2 == 0 ? 1 : -1) + random.NextDouble()),
                         lastChanged = DateTime.Now.AddDays(dispDays * (random.Next() % 2 == 0 ? 1 : -1) + random.NextDouble());
                if (created > lastChanged)
                {
                    DateTime t = created;
                    created = lastChanged;
                    lastChanged = t;
                }
                Note note = new Note(userId, 1, new string(Enumerable.Repeat(chars, random.Next(10, 40)).Select(s => s[random.Next(s.Length)]).ToArray()), new string(Enumerable.Repeat(chars, random.Next(40, 120)).Select(s2 => s2[random.Next(s2.Length)]).ToArray()), created, lastChanged);
                DBContext.Notes.Add(note);
                if (random.Next() % 2 == 0)
                {
                    DateTime start = DateTime.Now.AddDays(dispDays * (random.Next() % 2 == 0 ? 1 : -1) + random.NextDouble()),
                             end = DateTime.Now.AddDays(dispDays * (random.Next() % 2 == 0 ? 1 : -1) + random.NextDouble());
                    if (start > end)
                    {
                        DateTime t = start;
                        start = end;
                        end = t;
                    }
                    Database.Context.Action action = new Database.Context.Action(note, start, end);
                    DBContext.Actions.Add(action);
                    Collection collection = new Collection(random.Next(0, 10));
                    DBContext.Collections.Add(collection);
                    for (int j = 0; j < collection.Count; ++j)
                    {
                        Point point = new Point(collection, new string(Enumerable.Repeat(chars, random.Next(10, 70)).Select(s => s[random.Next(s.Length)]).ToArray()), random.Next() % 2 == 0);
                        DBContext.Points.Add(point);
                    }
                    Mission mission = new Mission(action, false, collection);
                    DBContext.Missions.Add(mission);
                }
            }
        }
    }
}