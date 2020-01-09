using System;
using System.IO;
using System.Net;

namespace SocketSettings
{
    class DefaultLoaderSettings : ILoaderSettings
    {
        string path;
        public DefaultLoaderSettings(string path)
        {
            this.path = path;
        }

        public ISocketSettings Load()
        {
            string buf = "";
            
            using(FileStream f = new FileStream(path, FileMode.Open))
            {
                using(BinaryReader ff = new BinaryReader(f))
                {
                    buf = ff.ReadString();
                }
            }

            string[] values = buf.Split(new char[] { '|' });
            if (values.Length < 4
                || !IPAddress.TryParse(values[0], out IPAddress ip)
                || !int.TryParse(values[1], out int port)
                || !int.TryParse(values[3], out int mls))
                    throw new Exception("Inappropriate file content [" + path + "]");

            string[] strPorts = values[2].Split(new char[] { ',' });
            if(strPorts.Length < 1)
                throw new Exception("Inappropriate file content [" + path + "]");
            for(int i = 0; i < strPorts.Length; ++i)
                if(!int.TryParse(strPorts[i], out int _))
                    throw new Exception("Inappropriate file content [" + path + "]");

            int[] ports = new int[strPorts.Length];
            for (int i = 0; i < ports.Length; ++i)
                ports[i] = int.Parse(strPorts[i]);

            return new SocketSettings(ip.ToString(), port, ports, mls);
        }
    }
}
