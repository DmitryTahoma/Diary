using System;
using System.Net;

namespace SocketSettings
{
    public class SocketSettings : ISocketSettings
    {
        public string ServerIP { private set; get; }
        public int ServerPort { private set; get; }
        public int[] DefaultClientPorts { private set; get; }
        public int MlsOfDelay { private set; get; }

        public SocketSettings() : this("current", 11221, new int[] { 11222, 11224, 12550 }, 30000) { }

        public SocketSettings(string ip, int port, int[] defaultClientPorts, int mlsOfDelay)
        {
            if (ip != "current" && !IPAddress.TryParse(ip, out IPAddress temp))
                throw new ArgumentException("Invalid ip specified for server");
            if(port < 0 || port > 65535)
                throw new ArgumentException("Invalid port specified for server");
            if(mlsOfDelay < 0)
                throw new ArgumentException("Milliseconds of delay shouldn't by less than zero");
            for (int i = 0; i < defaultClientPorts.Length; ++i)
                if (defaultClientPorts[i] < 0 || defaultClientPorts[i] > 65535)
                    throw new ArgumentException("Invalid port specified for client [" + defaultClientPorts[i].ToString() + "]");
            ServerIP = ip;
            ServerPort = port;
            DefaultClientPorts = defaultClientPorts;
            MlsOfDelay = mlsOfDelay;
        }

        public SocketSettings(string path)
        {
            ISocketSettings settings = new DefaultLoaderSettings(path).Load();
            this.ServerIP = settings.ServerIP;
            this.ServerPort = settings.ServerPort;
            this.DefaultClientPorts = settings.DefaultClientPorts;
            this.MlsOfDelay = settings.MlsOfDelay;
        }

        public void Save(string path)
        {
            Save(new DefaultSaverSettings(path));
        }

        public void Save(ISaverSettings saverSettings)
        {
            saverSettings.Save(ServerIP, ServerPort, DefaultClientPorts, MlsOfDelay);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ISocketSettings))
                return false;

            SocketSettings other = (SocketSettings)obj;
            if (this.DefaultClientPorts.Length != other.DefaultClientPorts.Length)
                return false;

            for (int i = 0; i < DefaultClientPorts.Length; ++i)
            {
                if (this.DefaultClientPorts[i] != other.DefaultClientPorts[i])
                    return false;
            }

            return this.ServerIP == other.ServerIP 
                && this.ServerPort == other.ServerPort
                && this.MlsOfDelay == other.MlsOfDelay;
        }
    }
}
