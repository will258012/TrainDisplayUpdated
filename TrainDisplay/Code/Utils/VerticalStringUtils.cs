using System.Collections.Generic;
using System.Text;
using TrainDisplay.Settings;
using TrainDisplay.UI;

namespace TrainDisplay.Utils
{
    public static class VerticalStringUtils
    {
        private static readonly Dictionary<string, string> verticalStringConvertDic = new()
        {
            {" ","　"},

            {",","︐"},
            {"，","︐"},
            {".","︒" },
            {"。","︒" },
            {"、","︑"},

            {"—", "｜"},
            {"–", "｜"},
            {"-", "｜"},
            {"ー", "｜"},
            {"~", "｜"},
            {"～", "｜"},

            {"\"", "〝"},
            {"'", "〞"},
            {"“", "〝"},
            {"”", "〟"},
            {"‘", "〞"},
            {"’", "〟"},
            {"「", "﹁"},
            {"」", "﹂"},
            {"『", "﹃"},
            {"』", "﹄"},

            {"(", "︵"},
            {")", "︶"},
            {"（", "︵"},
            {"）", "︶"},
            {"[", "︗"},
            {"]", "︘"},
            {"【", "︗"},
            {"】", "︘"},
            {"〖", "︗"},
            {"〗", "︘"},
            {"{", "︷"},
            {"}", "︸"},
            {"〈", "︿"},
            {"〉", "﹀"},
            {"《", "︽"},
            {"》", "︾"},

            {"…", "︙"},
            {"..", "︙"},
            {"...", "︙"},
            {"·", "・"},
            {"‧","・"},
            {"●", "︙"},
        };

        public static string ToVerticalString(this string str, int numPerLine = 4)
        {
            var tmpStr = new StringBuilder();
            str = ToFullWidth(str);
            tmpStr.Append(str);

            if (str.Length <= numPerLine)
            {
                return Join("\n", str.ToCharArray());
            }
            tmpStr.Append('　', str.Length % numPerLine == 0 ? 0 : numPerLine - (str.Length % numPerLine));
            var result = new StringBuilder();

            for (int j = 0; j < numPerLine; j++)
            {
                if (j != 0)
                {
                    result.Append("\n");
                }
                for (int i = 0; i < tmpStr.Length / numPerLine; i++)
                {
                    if (TrainDisplaySettings.DisplayRowDirection == TrainDisplaySettings.DisplayRowDirections.R2L)
                    {
                        result.Append(tmpStr[(((tmpStr.Length / numPerLine) - i - 1) * numPerLine) + j]);
                    }
                    else
                    {
                        result.Append(tmpStr[(i * numPerLine) + j]);
                    }
                }
            }

            return result.ToString();
        }
        public static string Join(string delimiter, char[] c)
        {
            if (c == null || c.Length == 0)
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            for (int i = 0; i < c.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(delimiter);
                }
                result.Append(c[i]);
            }
            return result.ToString();
        }
        public static string Sub(this string str)
        {
            int maxLength = DisplayUI.IsCJK() ? 16 : 30;
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            int half = (maxLength - 1) / 2;
            return str.Substring(0, half) + "…" + str.Substring(str.Length - half);
        }

        private static string ToFullWidth(string str)
        {
            foreach (string before in verticalStringConvertDic.Keys)
            {
                str = str.Replace(before, verticalStringConvertDic[before]);
            }
            var sb = new StringBuilder(str.Length);

            foreach (char c in str)
            {
                if (c == ' ')
                {
                    sb.Append('　');
                    continue;
                }
                if (c >= 33 && c <= 126)
                {
                    sb.Append((char)(c + 0xFEE0));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
