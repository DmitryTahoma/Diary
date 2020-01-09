using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore.Test
{
    public partial class ServerTest
    {
        private class TestCommands : ICommands
        {
            public string ExecuteCommand(string commandName, string[] args)
            {
                if (args == null)
                    args = new string[0];
                if(args.Length == 0)
                    if (commandName == "hello, server")
                        return "hello, client";
                if(commandName == "multiplyAll")
                    {
                        int result = 1;
                        for (int i = 0; i < args.Length; ++i)
                            if (int.TryParse(args[i], out int a))
                                result *= a;
                        return result.ToString();
                    }
                return "message is received";
            }
        }

        private string getLocalIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private string Send(string message, TcpClient client, string ip, int port = 11222)
        {
            byte[] data = Encoding.Unicode.GetBytes(ip + ":" + port + "|" + message);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            return StartAcceptAnswer(ip, port);
        }

        private string SendAndWaitResponseWithDelay(string message, string ip, int serverPort, int clientPort, int delay)
        {
            SendNoWaitResponse(message, ip, serverPort, clientPort);
            Thread.Sleep(delay);
            return StartAcceptAnswer(ip, clientPort);
        }

        private void SendNoWaitResponse(string message, string ip, int serverPort, int clientPort)
        {
            byte[] data = Encoding.Unicode.GetBytes(ip + (clientPort != -1? ":" + clientPort.ToString() : "") + "|" + message);
            TcpClient client = new TcpClient(ip, serverPort);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            client.Dispose();
        }

        private string StartAcceptAnswer(string ip, int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(ip), port);
            NetworkStream stream = null;
            try
            {
                bool t = true;
                while (t)
                {
                    try
                    {
                        listener.Start();
                        t = false;
                    }
                    catch { Thread.Sleep(1000); }
                }
                TcpClient client = listener.AcceptTcpClient();
                stream = client.GetStream();
                byte[] data = new byte[64];

                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);
                return builder.ToString();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (listener != null)
                    listener.Stop();
            }
        }
    }
}
