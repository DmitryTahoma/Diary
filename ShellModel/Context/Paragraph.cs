using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShellModel.Context
{
    public class Paragraph : IMissionContext
    {
        private bool isAutoTiming = false;
        private ParagraphMission mission = null;

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

        public Paragraph(ParagraphMission mission, bool autoTiming) : this(-1) 
        {
            isAutoTiming = autoTiming;
            this.mission = mission;
        }

        public Paragraph(string dbStr)
        {
            Regex regex = new Regex("^\b<sc>\b\\d+\b<sc>\b\\d+\b<sc>\b[\b<sp>\b\\d+\b<sp>\b[\\s\\S]*\b<sp>\b[1\b<sp>\b]*\b<sc>\b]*"),
                  regex2 = new Regex("^\b<sc>\b\\d+\b<sc>\b\\d+\b<sc>\b");
            if (regex.IsMatch(dbStr) || regex2.IsMatch(dbStr))
            {
                string[] values = StringsHelper.Split("\b<sc>\b", dbStr);
                Id = int.Parse(values[0]);

                Items = new List<Point>();
                if (int.Parse(values[1]) != 0)
                    for(int i = 2; i < values.Length; ++i)
                        Items.Add(new Point(values[i]));
            }
            else
                throw new ArgumentException();
        }

        int id = -1;
        public int Id
        {
            set
            {
                if (id < 1)
                    id = value;
            }
            get => id;
        }

        public int Count => Items.Count;

        public List<Point> Items { private set; get; }

        public void AddPoint(Point point)
        {
            Items.Add(point);
            if(isAutoTiming)            
                point.Id = DBHelper.AddPointToParagraphMissionStatic(mission, point);
        }

        public void RemovePoint(int id)
        {
            Items.Remove(Items.Where(x => x.Id == id).First());
        }

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