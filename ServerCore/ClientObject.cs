﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

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

        public void Process(Logger.Logger logger, int[] defaultClientPorts, int delay)
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64];
                while (true)
                {
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
                    if(query.Length > 2)
                    {
                        queryArgs = new string[query.Length - 2];
                        for (int i = 0; i < queryArgs.Length; ++i)
                            queryArgs[i] = query[i + 2];
                    }

                    int port = -1;
                    if (query[0].Split(new char[] { ':' }).Length == 2)
                        port = int.Parse(query[0].Split(new char[] { ':' })[1]);

                    try
                    {
                        if (commands != null)
                            message = commands.ExecuteCommand(query[1], queryArgs);
                        else
                            message = "c0";
                        logger.Log("CLIENT: " + query[0] + " REQUEST: " + query[1] + " RESPONSE: " + message, Logger.EntryLevel.User);
                    } catch (IndexOutOfRangeException) { break; }

                    string clientIP = query[0].Split(new char[] { ':' })[0];
                    MiniClient miniClient = null;
                    if (port != -1)
                        try
                        {
                            miniClient = new MiniClient(clientIP, port);
                        }
                        catch(SocketException)
                        {
                            logger.Log("CLIENT: " + query[0] + " didn't respond", Logger.EntryLevel.User);
                        }
                    else
                        miniClient = new MiniClient(clientIP, defaultClientPorts);
                    if (miniClient != null && !miniClient.SendNoWaitAnswer(message, delay / 3))
                        logger.Log("CLIENT: " + query[0] + " didn't respond", Logger.EntryLevel.User);
                }
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
