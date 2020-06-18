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

        public void Save(string ip, int serverPort, int mls)
        {
            string buf = ip + "|" + serverPort.ToString() + "|" + mls.ToString();

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
