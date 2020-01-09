using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    /// <summary>
    /// Integrated client that responds to a client request
    /// </summary>
    class MiniClient
    {
        TcpClient client = null;
        int[] ports = null;
        string ip = "";

        public MiniClient(string ip, int port)
        {
            client = new TcpClient(ip, port);
            this.ip = ip;
        }

        public MiniClient(string ip, int[] defaultClientPorts)
        {
            this.ip = ip;
            ports = defaultClientPorts;
        }

        public bool SendNoWaitAnswer(string message, int mls)
        {
            bool sended = false;
            int i = 0;
            do
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    sended = TrySendNoWaitAnswer(message);
                });
                task.Wait(mls);
                ++i;
            }
            while (!sended && i <= 3);
            return sended;
        }

        private bool TrySendNoWaitAnswer(string message)
        {
            if(client != null)
            {
                bool sended = false;
                Task clientTask = Task.Factory.StartNew(() => {
                    NetworkStream stream = null;
                    try
                    {
                        stream = client.GetStream();
                        byte[] data = Encoding.Unicode.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                        sended = true;
                    }
                    catch
                    {
                        sended = false;
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                        if (client != null)
                            client.Close();
                    }
                });
                clientTask.Wait(1000);
                return sended;
            }
            else
            {
                for(int i = 0; i < ports.Length; ++ i)
                {
                    bool sended = false;
                    try
                    {
                        client = new TcpClient(ip, ports[i]);
                        sended = TrySendNoWaitAnswer(message);
                    }
                    catch(SocketException)
                    {
                        sended = false;
                    }
                    if (sended)
                        return true;
                }
                return false;
            }
        }
    }
}
