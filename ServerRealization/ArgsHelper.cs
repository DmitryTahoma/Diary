using ServerRealization.Database;
using System.Linq;

namespace ServerRealization
{
    static class ArgsHelper
    {
        public static bool NoteIsExist(int id)
        {
            return DBContext.Notes
                    .Where(x => x.Id == id)
                    .Count() == 1;
        }

        public static bool IsAne(string login, string password, int noteId)
        {
            return DBContext.Users
                    .Where(y => y.Login == login && y.Password == password)
                    .First().Id != DBContext.Notes
                        .Where(y => y.Id == noteId)
                        .First().UserId;
        }

        public static bool CheckLoginPassword(string login, string password)
        {
            return DBContext.Users
                    .Where((x) => x.Login == login && x.Password == password)
                    .Count() == 1;
        }

        public static bool CheckArgs(string[] args, int expectedCount, bool isUInt = true, params int[] integerIds)
        {
            int i = -1;
            if (args != null)
                if (args.Length > expectedCount - 1)
                    for (i = 0; i < expectedCount; ++i)
                        if (args[i] == "")
                            return false;
            if (i != expectedCount)
                return false;
            for (i = 0; i < integerIds.Length; ++i)
                if (integerIds[i] < expectedCount)
                    if (!int.TryParse(args[integerIds[i]], out int val))
                        return false;
                    else { if (val == 0 || (isUInt && val < 0)) return false; }
                else return false;
            return true;
        }
    }
}