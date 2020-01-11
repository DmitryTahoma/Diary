using ServerCore;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System;
using System.Linq;

namespace ServerRealization
{
    class ServerCommands : ICommands
    {
        public string ExecuteCommand(string commandName, string[] args)
        {
            switch(commandName)
            {
                default: return "message is received";
                case "cc": return "cs";
                case "clp": return CheckLoginPassword(args);
                case "rnu": return RegisterNewUser(args);
            }
        }

        private string CheckLoginPassword(string[] args)
        {
            if(args != null)
                if(args.Length > 1)
                    if(args[0] != "" && args[1] != "")
                        return 
                            (DBContext.Users
                            .Where((x) => x.Login == args[0] && x.Password == args[1])
                            .Count()
                            == 1).ToString();
            return "ae";
        }

        private string RegisterNewUser(string[] args)
        {
            if(args != null)
                if(args.Length > 2)
                    if(args[0] != "" && args[1] != "" && args[2] != "")
                    {
                        if (DBContext.Users
                            .Where(x => x.Login == args[0])
                            .Count() >= 1)
                            return "uc";

                        User user = new User(args[2], args[0], args[1], DateTime.Now);
                        DBContext.Users.Add(user);
                        return "True";
                    }
            return "ae";
        }
    }
}