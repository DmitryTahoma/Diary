using System;

namespace ShellModel
{
    public static class StringsHelper
    {
        public static string[] Split(string splitter, string str)
        {
            if (splitter == str)
                return new string[] { };
            string[] values = new string[1];
            for (int i = 0, s = 0, index = -1, start = 0; i < str.Length; ++i)
            {
                if (i == str.Length - 1)
                    values[index] = str.Substring(start, i - start - splitter.Length + 1);
                if (s == splitter.Length)
                {
                    if (index != -1)
                        values[index] = str.Substring(start, i - start - splitter.Length);
                    index++;
                    if (values.Length == index)
                        Array.Resize(ref values, index + 1);
                    s = 0;
                    start = i;
                    if (index == values.Length)
                        break;
                }
                if (splitter[s] == str[i])
                {
                    s++;
                    if (s != splitter.Length && i != str.Length - 1)
                    {
                        if (splitter[s] != str[i + 1])
                            s = 0;
                    }
                }
            }
            return values;
        }
    }
}