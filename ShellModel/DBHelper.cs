using ClientCore;
using SocketSettings;
using System.Threading;

namespace Shell.Models
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
    }
}
