using System.Collections.Generic;
using System.Text;
using TrainDisplay.Settings;

namespace TrainDisplay.Utils
{
    public class AStringUtils
    {
        private static readonly Dictionary<string, string> verticalStringConvertDic = new Dictionary<string, string>
        {
            {"ー","｜"},
            {" ","　"}
        };

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
        public static string CreateVerticalString(string str, int numPerLine)
        {
            var tmpStr = new StringBuilder();
            tmpStr.Append(str);
            foreach (string before in verticalStringConvertDic.Keys)
            {
                tmpStr.Replace(before, verticalStringConvertDic[before]);
            }

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
    }
}
