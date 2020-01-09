using System.IO;

namespace SocketSettings
{
    class DefaultSaverSettings : ISaverSettings
    {
        string path;
        public DefaultSaverSettings(string path)
        {
            this.path = path;
        }

        public void Save(string ip, int serverPort, int[] clientPorts, int mls)
        {
            string buf = ip + "|" + serverPort.ToString() + "|";
            for (int i = 0; i < clientPorts.Length; ++i)
                buf += clientPorts[i].ToString() + (i == clientPorts.Length - 1 ? "|" : ",");
            buf += mls.ToString();

            using(FileStream f = new FileStream(path, FileMode.OpenOrCreate))
            {
                using(BinaryWriter ff = new BinaryWriter(f))
                {
                    ff.Write(buf);
                }
            }
        }
    }
}
