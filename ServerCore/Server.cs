using SocketSettings;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore
{
    public class Server
    {
        TcpListener listener;
        ICommands commands;
        ISocketSettings settings;
        bool listen;
        Logger.Logger logger;

        public bool IsStarted { private set; get; }

        public Server(ICommands commands)
            : this(commands, new SocketSettings.SocketSettings()) { }

        public Server(ICommands commands, ISocketSettings settings)
        {
            this.commands = commands;
            IsStarted = false;
            logger = new Logger.Logger();
            if (settings.ServerIP == "current")
                this.settings = new SocketSettings.SocketSettings(getLocalIp(), settings.ServerPort, settings.DefaultClientPorts, settings.MlsOfDelay);
            else
                this.settings = settings;
        }

        public Server(ICommands commands, string serverIP, int serverPort, int[] defaultClientPorts, int mlsOfDelay)
            : this(commands, new SocketSettings.SocketSettings(serverIP, serverPort, defaultClientPorts, mlsOfDelay)) { }

        public void Run()
        {
            if (IsStarted)
                return;
            listen = true;
            IsStarted = true;
            logger.Log("Server started " + DateTime.Now.ToString("dd.MM.yy HH:mm:ss") + " IP = " + settings.ServerIP + ":" + settings.ServerPort.ToString(), Logger.EntryLevel.Server);
            if (commands == null)
                logger.Log("Commands slot is void", Logger.EntryLevel.Server);
            try
            {
                listener = new TcpListener(IPAddress.Parse(settings.ServerIP), settings.ServerPort);
                listener.Start();
                while (listen)
                {
                    TcpClient client = null;
                    try
                    {
                        client = listener.AcceptTcpClient();
                        ClientObject clientObject = new ClientObject(client, commands);
                        Thread clientThread = new Thread(new ThreadStart(()=> { clientObject.Process(logger, settings.DefaultClientPorts, settings.MlsOfDelay); }));
                        clientThread.Start();
                    } catch(SocketException) { }
                }
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }

        public void Stop()
        {
            if (!IsStarted)
                return;
            IsStarted = false;
            listen = false;
            Thread.Sleep((int)settings.MlsOfDelay);
            if (listener != null)
                listener.Stop();
        }
        public static bool IsFreePort(int port, string ip = "current")
        {
            TcpListener tempServer = new TcpListener(IPAddress.Parse(ip == "current" ? getLocalIp() : ip), port);
            bool isFree = false;
            try
            {
                tempServer.Start();
                isFree = true;
                tempServer.Stop();
            }
            catch { }
            return isFree;
        }

        public Logger.Logger GetLogger()
        {
            return logger;
        }

        private static string getLocalIp()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
