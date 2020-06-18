using System.IO;

namespace SocketSettings.Test
{
    public partial class SocketSettingsTest
    {
        private class MySaverSettings : ISaverSettings
        {
            private string path;

            public MySaverSettings(string path)
            {
                this.path = path;
            }

            public void Save(string ip, int serverPort, int mls)
            {
                string buf = ip + "," + serverPort.ToString() + "," + mls.ToString();

                using(FileStream f = new FileStream(path, FileMode.OpenOrCreate))
                {
                    using(BinaryWriter ff = new BinaryWriter(f))
                    {
                        ff.Write(buf);
                    }
                }
            }
        }

        private class MyLoaderSettings : ILoaderSettings
        {
            string path;
            public MyLoaderSettings(string path)
            {
                this.path = path;
            }

            public ISocketSettings Load()
            {
                string buf = "";
                using (FileStream f = new FileStream(path, FileMode.Open))
                {
                    using(BinaryReader ff = new BinaryReader(f))
                    {
                        buf = ff.ReadString();
                    }
                }

                string[] values = buf.Split(new char[] { ',' });
                string ip = values[0];
                int port = int.Parse(values[1]);
                int mls = int.Parse(values[2]);

                return new SocketSettings(ip, port, mls);
            }
        }
    }
}
