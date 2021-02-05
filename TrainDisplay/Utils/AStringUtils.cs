using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrainDisplay.Utils
{
    class AStringUtils
    {

        private static readonly Dictionary<string, string> verticalStringConvertDic = new Dictionary<string, string>
        {
            {"ー", "｜" }
        };

        public static string Join(string delimiter, char[] c)
        {
            string result = "";
            for (int i = 0; i < c.Length; i++)
            {
                if (i != 0)
                {
                    result += delimiter;
                }
                result += c[i];
            }
            return result;
        }
        public static string CreateVerticalString(string str, int numPerLine)
        {
            string tmpStr = str;
            foreach (string before in verticalStringConvertDic.Keys)
            {
                tmpStr = tmpStr.Replace(before, verticalStringConvertDic[before]);
            }

            if (str.Length <= numPerLine)
            {
                return Join("\n", str.ToCharArray());
            }
            tmpStr = tmpStr + new String('　', str.Length % numPerLine == 0 ? 0 : numPerLine - (str.Length % numPerLine));
            string result = "";

            for (int j = 0; j < numPerLine; j++)
            {
                if (j != 0)
                {
                    result += "\n";
                }
                for (int i = 0; i < tmpStr.Length / numPerLine; i++)
                {
                    result += tmpStr[(tmpStr.Length / numPerLine - i - 1) * numPerLine + j];
                }
            }

            return result;
        }
    }
}
