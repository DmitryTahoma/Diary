﻿namespace ServerCore
{
    public interface ICommands
    {
        string ExecuteCommand(string commandName, string[] args);
    }
}
