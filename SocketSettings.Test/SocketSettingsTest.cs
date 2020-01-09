using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;

namespace SocketSettings.Test
{
    [TestClass]
    public partial class SocketSettingsTest
    {
        [DataTestMethod]
        [DataRow("192.168.0.120", 1, new int[] { 4, 5, 6 }, 367)]
        [DataRow("192.168.0.121", 2, new int[] { 4, 5, 6, 7 }, 3677)]
        public void EqualsTest(string ip, int port, int[] ports, int mls)
        {
            SocketSettings settings1 = new SocketSettings(ip, port, ports, mls);
            SocketSettings settings2 = new SocketSettings(ip, port, ports, mls);

            Assert.AreEqual(settings1, settings2);
        }

        [DataTestMethod]
        [DataRow("192.168.0.120", 1, new int[] { 4, 5, 6 }, 367, "192.168.0.121", 1, new int[] { 4, 5, 6 }, 367)]
        [DataRow("192.168.0.120", 1, new int[] { 4, 5, 6 }, 367, "192.168.0.121", 1, new int[] { 4, 5, 6 }, 367)]
        [DataRow("192.168.0.120", 1, new int[] { 4, 5, 6 }, 367,"192.168.0.120", 2, new int[] { 4, 5, 6 }, 367)]
        [DataRow("192.168.0.120", 1, new int[] { 4, 5, 6 }, 367,"192.168.0.120", 1, new int[] { 4, 5 }, 367)]
        [DataRow("192.168.0.120", 1, new int[] { 4, 5, 6 }, 367,"192.168.0.120", 1, new int[] { 4, 5, 7 }, 367)]
        [DataRow("192.168.0.120", 1, new int[] { 4, 5, 6 }, 367,"192.168.0.120", 1, new int[] { 4, 5, 6, 7 }, 367)]
        [DataRow("192.168.0.120", 1, new int[] { 4, 5, 6 }, 367,"192.168.0.120", 1, new int[] { 4, 5, 6 }, 368)]
        public void NotEqualsTest(string firstIP, int firstPort, int[] firstPorts, int firstMls, 
            string secondIP, int secondPort, int[] secondPorts, int secondMls)
        {
            Assert.AreNotEqual(new SocketSettings(firstIP, firstPort, firstPorts, firstMls),
                new SocketSettings(secondIP, secondPort, secondPorts, secondMls));
        }
        
        [DataTestMethod]
        [DataRow("ss1.bin", "192.168.0.120", 1, new int[] { 20, 4 }, 100, "192.168.0.120,1,20.4,100")]
        [DataRow("ss2.bin", "193.169.1.121", 11, new int[] { 204 }, 404, "193.169.1.121,11,204,404")]
        public void SaveTest(string path, string ip, int port, int[] ports, int mls, string expectedResult)
        {
            ISaverSettings saver = new MySaverSettings(path);
            SocketSettings settings = new SocketSettings(ip, port, ports, mls);
            settings.Save(saver);

            BinaryReader reader = null;
            try
            {
                reader = new BinaryReader(new FileStream(path, FileMode.Open));
                string buf = reader.ReadString();
                Assert.AreEqual(expectedResult, buf);
            }
            catch(FileNotFoundException)
            {
                Assert.Fail("File not found exception");
            }
            catch (EndOfStreamException) 
            {
                Assert.Fail("File is empty");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
        }

        [DataTestMethod]
        [DataRow("ss1.bin", "192.168.0.120", 1, new int[] { 20, 4 }, 100, "192.168.0.120|1|20,4|100")]
        [DataRow("ss2.bin", "193.169.1.121", 11, new int[] { 204 }, 404, "193.169.1.121|11|204|404")]
        public void DefaultSaveTest(string path, string ip, int port, int[] ports, int mls, string expectedResult)
        {
            SocketSettings settings = new SocketSettings(ip, port, ports, mls);
            settings.Save(path);

            BinaryReader reader = null;
            try
            {
                reader = new BinaryReader(new FileStream(path, FileMode.Open));
                string buf = reader.ReadString();

                Assert.AreEqual(expectedResult, buf);
            }
            catch (FileNotFoundException)
            {
                Assert.Fail("File not found exception");
            }
            catch (EndOfStreamException)
            {
                Assert.Fail("File is empty");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
        }

        [DataTestMethod]
        [DataRow("123.bin", "100.200.100.200", 11221, new int[] { 11, 22, 1, 33 }, 20000)]
        [DataRow("5fg43.bin", "192.222.76.5", 11, new int[] { 121, 322, 51, 3773 }, 2067000)]
        public void LoadTest(string path ,string ip, int port, int[] ports, int mls)
        {
            ISocketSettings settings = new SocketSettings(ip, port, ports, mls);
            settings.Save(new MySaverSettings(path));

            ILoaderSettings loader = new MyLoaderSettings(path);
            ISocketSettings loadedSettings = loader.Load();

            Assert.AreEqual(settings, loadedSettings);
        }

        [DataTestMethod]
        [DataRow("123.bin", "100.200.100.200", 11221, new int[] { 11, 22, 1, 33 }, 20000)]
        [DataRow("5fg43.bin", "192.222.76.5", 11, new int[] { 121, 322, 51, 3773 }, 2067000)]
        public void DefaultLoadTest(string path, string ip, int port, int[] ports, int mls)
        {
            SocketSettings settings = new SocketSettings(ip, port, ports, mls);
            settings.Save(path);

            SocketSettings loadedSettings = new SocketSettings(path);

            Assert.AreEqual(settings, loadedSettings);
        }
    }
}
