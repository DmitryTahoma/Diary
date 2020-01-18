using ServerRealization;
using System;

namespace ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerProgram server = new ServerProgram("192.168.0.105", 11221, new int[] { 11222, 11224, 12550 }, 3000);
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
