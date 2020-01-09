namespace SocketSettings
{
    public interface ISaverSettings
    {
        void Save(string serverIp, int serverPort, int[] defaultClientPorts, int mlsOfDelay);
    }
}
