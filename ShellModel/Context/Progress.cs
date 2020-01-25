namespace ShellModel.Context
{
    public class Progress : IMissionContext
    {
        public Progress(int id, int start, int current, int end)
        {
            Id = id;
            Start = start;
            Count = current;
            End = end;
        }

        public int Id { private set; get; }
        public int Start { set; get; }
        public int Count { set; get; }
        public int End { set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Progress))
                return false;
            Progress other = (Progress)obj;
            return this.Id == other.Id
                && this.Start == other.Start
                && this.Count == other.Count
                && this.End == other.End;
        }

        public override int GetHashCode()
        {
            var hashCode = 885619693;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Start.GetHashCode();
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            hashCode = hashCode * -1521134295 + End.GetHashCode();
            return hashCode;
        }
    }
}