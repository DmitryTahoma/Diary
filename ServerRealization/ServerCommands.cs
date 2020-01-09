using ServerCore;
using ServerRealization.Database;
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
            }
        }

        private string CheckLoginPassword(string[] args)
        {
            return 
                (DBContext.Users
                .Where((x) => x.Login == args[0] && x.Password == args[1])
                .Count()
                == 1).ToString();
        }
    }
}
