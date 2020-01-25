namespace ShellModel.Context
{
    public interface IMission
    {
        int Id { get; }
        MissionType Type { get; }
        IMissionContext Context { get; }
    }
}