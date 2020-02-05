using System;
using System.Collections.Generic;

namespace ShellModel.Context
{
    public class ProgressMission : Mission, IMission
    {
        public ProgressMission(int id, int contextId, int actionId, int noteId, int stereotypeId, string name, string text, DateTime created, DateTime lastChanged, DateTime start, DateTime end) 
            : base(id, MissionType.Progress, contextId, actionId, noteId, stereotypeId, name, text, created, lastChanged, start, end)
        {
        }

        public Progress Progress { get => (Progress)Context; }

        public override bool Equals(object obj)
        {
            if (!(obj is ProgressMission))
                return false;
            ProgressMission other = (ProgressMission)obj;
            return base.Equals(obj)
                && this.Id == other.Id
                && this.ContextId == other.ContextId;
        }

        public override int GetHashCode()
        {
            var hashCode = 1063810044;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Progress>.Default.GetHashCode(Progress);
            return hashCode;
        }
    }
}