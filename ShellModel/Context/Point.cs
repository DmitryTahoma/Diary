using System;
using System.Text.RegularExpressions;

namespace ShellModel.Context
{
    public class Point
    {
        public Point(int id, string text, bool isChecked)
        {
            Id = id;
            Text = text;
            IsChecked = isChecked;
        }

        public Point(string dbStr)
        {
            string splitter = "\b<sp>\b";
            Regex regex1 = new Regex("^" + splitter + @"\d+" + splitter + @"[\s\S]*" + splitter + "1" + splitter);
            Regex regex2 = new Regex("^" + splitter + @"\d+" + splitter + @"[\s\S]*" + splitter);
            if (regex2.IsMatch(dbStr))
            {
                string[] values = new string[2];
                for(int i = 0, s = 0, index = -1, start = 0; i < dbStr.Length; ++i)
                {
                    if (i == dbStr.Length - 1)
                        values[index] = dbStr.Substring(start, i - start - splitter.Length + 1);
                    if(s == splitter.Length)
                    {
                        if(index != -1)
                            values[index] = dbStr.Substring(start, i - start - splitter.Length);
                        index++;
                        s = 0;
                        start = i;
                        if (index == 2)
                            break;
                    }
                    if (splitter[s] == dbStr[i])
                        s++;
                    else if (s != 0)
                        s = 0;
                }
                Id = int.Parse(values[0]);
                Text = values[1];
                if (regex1.IsMatch(dbStr))
                    IsChecked = true;
            }
            else
                throw new ArgumentException();
        }

        public int Id { private set; get; }
        public string Text { set; get; }
        public bool IsChecked { set; get; }
    }
}