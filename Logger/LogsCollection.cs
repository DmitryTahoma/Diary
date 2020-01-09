using System.Collections.Generic;
using System.Linq;

namespace Logger
{
    public class LogsCollection
    {
        List<Entry> entries;

        public delegate void OnAddingContainer(Entry entry);
        public event OnAddingContainer OnAdd;

        public LogsCollection()
        {
            entries = new List<Entry>();
            OnAdd += (entry) => { };
        }

        public void Add(Entry entry)
        {
            entries.Add(entry);
            OnAdd(entry);
        }

        public int Count()
        {
            return entries.Count();
        }

        public int CountOfContains(string text, bool isCaseSensitive = false)
        {
            int result = 0;
            for(int i = 0; i < entries.Count(); ++i)
            {
                if (isCaseSensitive && entries[i].Text.Contains(text))
                    result++;
                else if (!isCaseSensitive && entries[i].Text.ToLower().Contains(text.ToLower()))
                    result++;
            }
            return result;
        }

        public ICollection<Entry> GetLogsByLevel(EntryLevel level)
        {
            return entries.Where((x) => x.Level == level).ToList();
        }

        public Entry LastByTime()
        {
            if(entries.Count() <= 0)
                return null;
            if (entries.Count() == 1)
                return entries[0];

            int index = 0;
            for(int i = 1; i < entries.Count(); ++i)
                if (entries[i].Time > entries[index].Time)
                    index = i;
            return entries[index];
        }

        public Entry LastAdded()
        {
            if (entries.Count() <= 0)
                return null;
            return entries[entries.Count() - 1];
        }

        public Entry this[int index]
        {
            get
            {
                return entries[index];
            }
        }
    }
}
