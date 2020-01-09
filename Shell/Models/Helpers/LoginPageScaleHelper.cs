using System.Collections.Generic;
using System.Windows.Controls;

namespace Shell.Models.Helpers
{
    static class LoginPageScaleHelper
    {
        static List<RowDefinition> rows;

        static LoginPageScaleHelper()
        {
            rows = new List<RowDefinition>();
        }

        public static void FindAllControls(Page page)
        {
            rows.Add(((Grid)((Grid)page.Content).Children[0]).RowDefinitions[1]);
            rows.Add(((Grid)((Grid)page.Content).Children[0]).RowDefinitions[4]);
        }

        public static void SetFontSize(double fontSize)
        {
            for (int i = 0; i < rows.Count; ++i)
                rows[i].MaxHeight = fontSize + 10 + (fontSize / 5);
        }

        public static bool IsEmpty()
        {
            return rows.Count == 0;
        }
    }
}
