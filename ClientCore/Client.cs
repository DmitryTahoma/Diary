using SocketSettings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientCore
{
    public class Client
    {
        ISocketSettings settings;
        TcpClient client;
        int port, serverPort;

        delegate string SendHandler(string msg);
        Queue<SendHandler> sendQueue;

        public Client(ISocketSettings clientSettings)
        {
            settings = clientSettings;
            port = -1;
            serverPort = settings.ServerPort;
            sendQueue = new Queue<SendHandler>();
        }

        public string Send(string message)
        {
            if (port == -1)
            {
                DetermineFreePort();
            }

            SendHandler handler = new SendHandler((msg) => 
            {  
                byte[] data = Encoding.Unicode.GetBytes(GetLocalIP() + ":" + port.ToString() + "|" + msg);
                client = new TcpClient(settings.ServerIP, serverPort);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                return ListenResponse();
            });
            sendQueue.Enqueue(handler);

            while (true)
                if (sendQueue.Peek() == handler)
                {
                    SendHandler sendHandler = sendQueue.Dequeue();
                    return sendHandler.Invoke(message);
                }
                else
                    Thread.Sleep(100);            
        }

        public string SendCommand(string command, string[] args)
        {
            string message = command;
            if (args == null)
                return Send(command);
            if(args.Length == 0)
                return Send(command);

            for (int i = 0; i < args.Length; ++i)
                message += "|" + args[i];
            return Send(message);
        }

        private string GetLocalIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private string ListenResponse()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(GetLocalIP()), port);
            NetworkStream stream = null;
            try
            {
                listener.Start();
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

        private void DetermineFreePort()
        {
            for (int i = 0; i < settings.DefaultClientPorts.Length; ++i)
            {
                if (port != -1)
                    break;
                port = settings.DefaultClientPorts[i];
                TcpListener tempServer = new TcpListener(IPAddress.Parse(GetLocalIP()), port);
                try
                {
                    tempServer.Start();
                    tempServer.Stop();
                }
                catch 
                {
                    port = -1;
                }
            }
        }
    }
}
