using ClientCore;
using SocketSettings;
using System.Threading;

namespace Shell.Models
{
    public class DBHelper
    {
        Client client;
        object locker = new object();
        int delay;

        public DBHelper(ISocketSettings settings)
        {
            client = new Client(settings);
            delay = settings.MlsOfDelay;
        }

        public bool Registration(string login, string password, string name)
        {
            bool result = false;
            lock(locker)
            {
                bool isSended = false;
                Thread clientThread = new Thread(new ThreadStart(() => 
                {
                    result = bool.Parse( 
                        client.SendCommand("rnu", new string[] { login, password, name }));
                    isSended = true;
                }));
                clientThread.Start();

                int counter = 0;
                while(counter * 100 <= delay)
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
    }
}
