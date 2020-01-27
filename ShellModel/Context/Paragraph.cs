using System.Collections.Generic;

namespace ShellModel.Context
{
    public class Paragraph : IMissionContext
    {
        public Paragraph(int id, List<string> items)
        {
            Id = id;
            if (items != null)
                Items = items;
            else
                Items = new List<string>();
        }

        public Paragraph(int id)
        {
            Id = id;
            Items = new List<string>();
        }

        public int Id { private set; get; }

        public int Count => Items.Count;

        public List<string> Items { private set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Paragraph))
                return false;
            Paragraph other = (Paragraph)obj;
            if (this.Count != other.Count)
                return false;
            for (int i = 0; i < Count; ++i)
                if (this.Items[i] != other.Items[i])
                    return false;
            return this.Id == other.Id;
        }
    }
}