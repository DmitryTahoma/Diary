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

        delegate string SendHandler(string msg);
        Queue<SendHandler> sendQueue;

        public Client(ISocketSettings clientSettings)
        {
            settings = clientSettings;
            sendQueue = new Queue<SendHandler>();
        }

        public string Send(string message)
        {
            SendHandler handler = new SendHandler((msg) => 
            {  
                byte[] data = Encoding.Unicode.GetBytes(msg);
                client = new TcpClient(settings.ServerIP, settings.ServerPort);
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

        private string ListenResponse()
        {
            NetworkStream stream = client.GetStream();
            try
            {
                DateTime start = DateTime.Now;
                while (!stream.DataAvailable && (DateTime.Now - start).TotalMilliseconds < settings.MlsOfDelay)
                    Thread.Sleep(100);
                if (!stream.DataAvailable)
                    return "Server didn't respond";

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
            }
        }
    }
}