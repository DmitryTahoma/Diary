using ServerCore;

namespace ClientCore.Test
{
    public partial class ClientTest
    {
        private class TestCommands : ICommands
        {
            public string ExecuteCommand(string commandName, string[] args)
            {
                if (args == null)
                    args = new string[0];
                if (args.Length == 0)
                {
                    if (commandName == "hello, server")
                        return "hello, client";
                    else if (commandName == "version")
                        return "alpha0.2";
                }
                else
                {
                    if(commandName == "showMyParams")
                    {
                        string result = args[0];
                        for(int i = 1; i < args.Length; ++i)
                        {
                            result += "," + args[i];
                        }
                        return result;
                    }
                    else if(commandName == "plus")
                    {
                        for (int i = 0; i < args.Length; ++i)
                            if (!int.TryParse(args[i], out int _))
                                return "ArgumentError";

                        int result = 0;
                        for (int i = 0; i < args.Length; ++i)
                            result += int.Parse(args[i]);
                        return result.ToString();
                    }
                }
                return "message is received";
            }
        }
    }
}