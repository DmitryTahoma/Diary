using System;
using System.Text.RegularExpressions;

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

        protected Mission() { }

        public static IMission CreateNew(string dbStr)
        {
            Regex regexProgress = new Regex("^\b<sm>\b\\d+\b<sm>\b\b<sa>\b\\d+\b<sa>\b\b<sn>\b\\d+\b<sn>\b[\\s\\S]*\b<sn>\b[\\s\\S]*\b<sn>\b\\d+[,\\d[E\\-\\d]*]*\b<sn>\b\\d+[,\\d[E\\-\\d]*]*\b<sn>\b\b<sa>\b\\d+[,\\d[E\\-\\d]*]*\b<sa>\b\\d+[,\\d[E\\-\\d]*]*\b<sa>\b\b<sm>\b\\d+z\\d+z\\d+z\\d+\b<sm>\b");
            if(regexProgress.IsMatch(dbStr))
            {
                string[] values = StringsHelper.Split("\b<sm>\b", dbStr);
                int id = int.Parse(values[0]);
                Action action = new Action(values[1]);
                Progress progress = new Progress(values[2]);
                ProgressMission progressMission = new ProgressMission(id, action, progress);
                return progressMission;
            }

            return null;
        }

        public new int Id { protected set; get; }
        public int ActionId { protected set { base.Id = value; } get => base.Id; }
        public int ContextId { protected set; get; }
        public MissionType Type { protected set; get; }
        public IMissionContext Context { protected set; get; }
    }
}