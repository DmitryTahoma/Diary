namespace SocketSettings
{
    public interface ISocketSettings
    {
        int ServerPort { get; }
        int MlsOfDelay { get; }
        void Save(ISaverSettings saverSettings);

        string ServerIP { get; }
    }
}
