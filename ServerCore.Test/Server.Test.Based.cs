using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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

        private string Send(string msg, int delay = 0)
        {
            TcpClient client = new TcpClient();
            client.Connect(correctSettings.ServerIP, correctSettings.ServerPort);

            DateTime startWaiting = DateTime.Now;
            while (!client.Connected || !client.GetStream().CanWrite)
                if ((DateTime.Now - startWaiting).TotalSeconds > 5)
                    Assert.Fail("Server did not response.");

            byte[] msg_b = Encoding.Unicode.GetBytes(msg);
            client.GetStream().Write(msg_b, 0, msg_b.Length);

            if (delay < 0)
            {
                client.Close();
                return "";
            }
            Thread.Sleep(delay);

            startWaiting = DateTime.Now;
            while (!client.GetStream().DataAvailable || !client.GetStream().CanRead)
                if ((DateTime.Now - startWaiting).TotalSeconds > 50000)
                    Assert.Fail("Server did not response.");

            List<byte> response = new List<byte>();
            while (client.GetStream().DataAvailable)
                response.Add((byte)client.GetStream().ReadByte());
            string res = Encoding.Unicode.GetString(response.ToArray());
            client.Close();

            return res;
        }
    }
}
