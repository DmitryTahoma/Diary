using ServerRealization;
using System;

namespace ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerProgram server = new ServerProgram(@"D:\Projects\Portfolio\Diary\packages\ss.bin");
            server.GetLogger().Logs.OnAdd += (x) => 
            {
                Console.WriteLine("[" + x.Time.ToString("dd.MM.yy HH:mm:ss") + " | L=" + x.Level.ToString() + " ] " + x.Text); 
            };
            try
            {
                server.Run();
                while(true)
                    if (Console.ReadLine().ToLower() == "stop")
                        break;
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
