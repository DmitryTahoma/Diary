using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClientCore.Test
{
    public partial class ClientTest
    {
        private class TestCommands : ICommands
        {
            public string ExecuteCommand(string commandName, string[] args)
            {
                if (args == null)
                    args = new string[0];
                if (args.Length == 0)
                {
                    if (commandName == "hello, server")
                        return "hello, client";
                    else if (commandName == "version")
                        return "alpha0.2";
                }
                else
                {
                    if(commandName == "showMyParams")
                    {
                        string result = args[0];
                        for(int i = 1; i < args.Length; ++i)
                        {
                            result += "," + args[i];
                        }
                        return result;
                    }
                    else if(commandName == "plus")
                    {
                        for (int i = 0; i < args.Length; ++i)
                            if (!int.TryParse(args[i], out int _))
                                return "ArgumentError";

                        int result = 0;
                        for (int i = 0; i < args.Length; ++i)
                            result += int.Parse(args[i]);
                        return result.ToString();
                    }
                }
                return "message is received";
            }
        }

        private static class PortsHelper
        {
            static List<TcpListener> listeners;

            static PortsHelper()
            {
                listeners = new List<TcpListener>();
            }


            public static void OccupyPort(int port)
            {
                TcpListener listener = new TcpListener(GetLocalIP(), port);
                listener.Start();
                Thread thread = new Thread(()=> { listener.AcceptTcpClient(); });
                listeners.Add(listener);
                thread.Start();
            }

            public static void Dispose()
            {
                for (int i = 0; i < listeners.Count; ++i) 
                    if (listeners[i] != null)
                        listeners[i].Stop();
            }

            private static IPAddress GetLocalIP()
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip;
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
        }
    }
}
