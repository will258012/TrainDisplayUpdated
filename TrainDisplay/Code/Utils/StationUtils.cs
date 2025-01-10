using System;
using System.Linq;
using TrainDisplay.Settings;

namespace TrainDisplay.Utils
{
    public class StationUtils
    {
        private static string[] StationSuffix => TrainDisplaySettings.StationSuffix
            .Split(new[] { @""",""" }, StringSplitOptions.None)
            .Select(suffix => suffix.Trim('"'))
            .ToArray();

        private static string[] StationSuffixWhiteList => TrainDisplaySettings.StationSuffixWhiteList
            .Split(new[] { @""",""" }, StringSplitOptions.None)
            .Select(suffix => suffix.Trim('"'))
            .ToArray();
        public static string RemoveStationSuffix(string stationName)
        {
            if (string.IsNullOrEmpty(TrainDisplaySettings.StationSuffix) ||
                (!string.IsNullOrEmpty(TrainDisplaySettings.StationSuffixWhiteList) 
                && StationSuffixWhiteList.Any(whiteListSuffix =>stationName.EndsWith(whiteListSuffix, StringComparison.OrdinalIgnoreCase))))
            {
                return stationName;
            }

            if (StationSuffix.FirstOrDefault(suffix =>
                stationName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) is string suffixToRemove)
            {
                return stationName.Substring(0, stationName.Length - suffixToRemove.Length);
            }

            return stationName;
        }
    }
}
