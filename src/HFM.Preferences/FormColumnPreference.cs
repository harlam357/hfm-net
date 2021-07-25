using System;
using System.Globalization;

namespace HFM.Preferences
{
    public static class FormColumnPreference
    {
        public static (int DisplayIndex, int Width, bool Visible, int Index)? Parse(string value)
        {
            string[] tokens = value.Split(',');
            if (tokens.Length != 4) return null;
            return (Int32.Parse(tokens[0]), Int32.Parse(tokens[1]), Boolean.Parse(tokens[2]), Int32.Parse(tokens[3]));
        }

        public static string Format(int displayIndex, int width, bool visible, int index)
        {
            return String.Format(CultureInfo.InvariantCulture,
                "{0},{1},{2},{3}",
                displayIndex.ToString("D2"),
                width,
                visible,
                index);
        }
    }
}
