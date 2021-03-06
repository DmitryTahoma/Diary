﻿using ServerCore;
using SocketSettings;
using System.Threading;

namespace ServerRealization
{
    public class ServerProgram
    {
        Server server;
        ICommands commands;
        int mlsToStop;

        public ServerProgram(string path)
        {
            commands = new ServerCommands();
            SocketSettings.SocketSettings settings = new SocketSettings.SocketSettings(path);
            mlsToStop = settings.MlsOfDelay;
            server = new Server(commands, settings);
        }

        public ServerProgram(string ip, int port, int mlsOfdelay)
        {
            commands = new ServerCommands();
            server = new Server(commands, ip, port, mlsOfdelay);
            mlsToStop = mlsOfdelay;
        }

        public ServerProgram(ISocketSettings settings)
        {
            commands = new ServerCommands();
            server = new Server(commands, settings);
            mlsToStop = settings.MlsOfDelay;
        }

        public void Run()
        {
            Thread serverThread = new Thread(server.Run);
            serverThread.Start();
        }

        public int Stop()
        {
            Thread serverThread = new Thread(server.Stop);
            serverThread.Start();
            return mlsToStop;
        }

        public string ExecuteCommand(string command, string[] args)
        {
            return commands.ExecuteCommand(command, args);
        }

        public Logger.Logger GetLogger() => server.GetLogger();
    }
}
