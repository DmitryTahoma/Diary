using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class ClientObject
    {
        TcpClient client;
        ICommands commands;

        public ClientObject(TcpClient client, ICommands commands)
        {
            this.client = client;
            this.commands = commands;
        }

        public void Process(Logger.Logger logger, int delay)
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64];
                
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    try
                    {
                        bytes = stream.Read(data, 0, data.Length);
                    }
                    catch (IOException)
                    {
                        break;
                    }
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);
                string message = builder.ToString();
                string[] query = message.Split(new char[] { '|' });

                string[] queryArgs = null;
                if(query.Length > 1)
                {
                    queryArgs = new string[query.Length - 1];
                    for (int i = 0; i < queryArgs.Length; ++i)
                        queryArgs[i] = query[i + 1];
                }

                if (commands != null)
                    message = commands.ExecuteCommand(query[0], queryArgs);
                else
                    message = "c0";
                logger.Log("CLIENT-T-" + Thread.CurrentThread.ManagedThreadId.ToString() + " REQUEST: " + query[0] + " RESPONSE: " + message, Logger.EntryLevel.User);
                
                byte[] response = Encoding.Unicode.GetBytes(message);

                DateTime start = DateTime.Now;
                bool isSended = false;
                while (!isSended && (DateTime.Now - start).TotalMilliseconds < delay)
                {
                    try
                    {
                        client.GetStream().Write(response, 0, response.Length);
                        isSended = true;
                    }
                    catch(SocketException) 
                    {
                        Thread.Sleep(100);
                    }
                }
                if(!isSended)
                    logger.Log("CLIENT-T-" + Thread.CurrentThread.ManagedThreadId.ToString() + " didn't respond", Logger.EntryLevel.User);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}