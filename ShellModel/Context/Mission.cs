using System;

namespace ShellModel.Context
{
    public abstract class Mission : Action
    {
        public Mission(int id, MissionType type, int contextId, int actionId, int noteId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged, DateTime start, DateTime end) 
            : base(actionId, noteId, stereotypeId, name, text, created, lastChanged, start, end)
        {
            Id = id;
            Type = type;
            ContextId = contextId;
        }

        public new int Id { private set; get; }
        public int ActionId { get => base.Id; }
        public int ContextId { private set; get; }
        public MissionType Type { private set; get; }
        public IMissionContext Context { protected set; get; }
    }
}