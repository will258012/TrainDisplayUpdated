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

        public static string RemoveStationSuffix(string stationName)
        {
            if (TrainDisplaySettings.StationSuffix == null || TrainDisplaySettings.StationSuffix.Length == 0)
            {
                return stationName;
            }

            foreach (var suffix in StationSuffix)
            {
                if (stationName.ToLower().EndsWith(suffix.ToLower()))
                {
                    return stationName.Remove(stationName.Length - suffix.Length);
                }
            }
            return stationName;
        }
    }
}
