using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ShellModel.Context
{
    public class Paragraph : IMissionContext
    {
        public Paragraph(int id, List<Point> items)
        {
            Id = id;
            if (items != null)
                Items = items;
            else
                Items = new List<Point>();
        }

        public Paragraph(int id)
        {
            Id = id;
            Items = new List<Point>();
        }

        public Paragraph() : this(-1) { }

        public Paragraph(string dbStr)
        {
            string splitter = "\b<sc>\b";
            Regex regex = new Regex("^\b<sc>\b\\d+\b<sc>\b\\d+\b<sc>\b[\b<sp>\b\\d+\b<sp>\b[\\s\\S]*\b<sp>\b[1\b<sp>\b]*\b<sc>\b]*");
            if (regex.IsMatch(dbStr))
            {
                string[] values = new string[3];
                for(int i = 0, s = 0, index = -1, start = 0; i < dbStr.Length; ++i)
                {
                    if (i == dbStr.Length - 1)
                        values[index] = dbStr.Substring(start, i - start - splitter.Length + 1);
                    if(s == splitter.Length)
                    {
                        if (index != -1)
                            values[index] = dbStr.Substring(start, i - start - splitter.Length);
                        index++;
                        s = 0;
                        start = i;
                        if (index == values.Length)
                            break;
                    }
                    if (splitter[s] == dbStr[i])
                        s++;
                    else if (s != 0)
                        s = 0;
                }
                Id = int.Parse(values[0]);

                Items = new List<Point>();
                string items = values[values.Length - 1];
                int count = int.Parse(values[1]);
                if (count != 0)
                {
                    for(int i = 0, s = 0, index = 0, start = 0; i < items.Length; ++i)
                    {
                        if (i == items.Length - 1)
                            Items.Add(new Point(items.Substring(start, i - start + 1)));
                        if(s == splitter.Length)
                        {
                            if (index != -1)
                                Items.Add(new Point(items.Substring(start, i - start - splitter.Length)));
                            index++;
                            s = 0;
                            start = i;
                            if (index == count)
                                break;
                        }
                        if (splitter[s] == items[i])
                        {
                            s++;
                            if (s != splitter.Length && i != items.Length - 1)
                            {
                                if (splitter[s] != items[i + 1])
                                    s = 0;
                            }
                        }
                    }
                }
            }
            else
                throw new ArgumentException();
        }

        public int Id { private set; get; }

        public int Count => Items.Count;

        public List<Point> Items { private set; get; }

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