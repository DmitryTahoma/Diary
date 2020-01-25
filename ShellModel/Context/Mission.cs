using System;

namespace ShellModel.Context
{
    abstract class Mission : Action
    {
        protected MissionType type;

        public Mission(int id, MissionType type, int contextId, int actionId, int noteId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged, DateTime start, DateTime end) 
            : base(actionId, noteId, stereotypeId, name, text, created, lastChanged, start, end)
        {
            Id = id;
            this.type = type;
            ContextId = contextId;
        }

        public new int Id { private set; get; }
        public int ActionId { get => base.Id; }
        public int ContextId { private set; get; }
    }
}