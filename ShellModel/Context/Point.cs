using System;
using System.Text.RegularExpressions;

namespace ShellModel.Context
{
    public class Point
    {
        public Point(int id, string text, bool isChecked)
        {
            this.id = id;
            Text = text;
            IsChecked = isChecked;
        }

        public Point(string dbStr)
        {
            Regex regex1 = new Regex("^\b<sp>\b\\d+\b<sp>\b[\\s\\S]*\b<sp>\b1\b<sp>\b");
            Regex regex2 = new Regex("^\b<sp>\b\\d+\b<sp>\b[\\s\\S]*\b<sp>\b");
            if (regex2.IsMatch(dbStr))
            {
                string[] values = StringsHelper.Split("\b<sp>\b", dbStr);

                id = int.Parse(values[0]);
                Text = values[1];
                if (regex1.IsMatch(dbStr))
                    IsChecked = true;
            }
            else
                throw new ArgumentException();
        }

        public Point(string text, bool isAutoTiming)
        {
            Text = text;
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
        public string Text { set; get; }
        public bool IsChecked { set; get; }
    }
}