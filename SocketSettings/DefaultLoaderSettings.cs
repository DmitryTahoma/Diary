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
            if (values.Length < 3
                || !IPAddress.TryParse(values[0], out IPAddress ip)
                || !int.TryParse(values[1], out int port)
                || !int.TryParse(values[2], out int mls))
                    throw new Exception("Inappropriate file content [" + path + "]");

            return new SocketSettings(ip.ToString(), port, mls);
        }
    }
}
