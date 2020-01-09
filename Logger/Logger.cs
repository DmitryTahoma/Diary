namespace Logger
{
    public class Logger
    {
        public LogsCollection Logs { private set; get; }

        public Logger()
        {
            Logs = new LogsCollection();
        }

        public void Log(string text, EntryLevel level = EntryLevel.Undefined)
        {
            Logs.Add(new Entry(text, level));
        }
    }
}
