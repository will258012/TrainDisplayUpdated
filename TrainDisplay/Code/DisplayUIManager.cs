extern alias FPSCamera;
using AlgernonCommons.Translation;
using System.Collections.Generic;
using TrainDisplay.Settings;
using TrainDisplay.UI;
using TrainDisplay.Utils;

namespace TrainDisplay
{
    public class DisplayUIManager
    {
        ushort followId;
        ushort[] stationIDList;//The internal value should be unique
        string[] stationNameList;
        List<int> terminalList = new List<int>();//Turnback display support
        LoopCounter nowPos;
        int routeStart;
        int routeEnd;
        bool IsCircular => terminalList.Count == 0;

        private static DisplayUIManager instance;

        public static DisplayUIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DisplayUIManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Initializes and sets up the display for the current vehicle instance, including station data and terminal checks.
        /// </summary>
        /// <param name="followId">The instance ID of the vehicle to follow.</param>
        /// <returns>
        /// Returns true if the display setup is successful, otherwise returns false.
        /// </returns>
        public bool SetDisplay(ushort followId)
        {
            this.followId = followId;
            var firstVehicle = GetVehicle(GetFirstVehicleId());
            var info = firstVehicle.Info;

            switch (info.vehicleCategory)
            {
                case VehicleInfo.VehicleCategory.PassengerTrain when TrainDisplaySettings.IsTrain:
                case VehicleInfo.VehicleCategory.MetroTrain when TrainDisplaySettings.IsMetro:
                case VehicleInfo.VehicleCategory.Monorail when TrainDisplaySettings.IsMonorail:
                case VehicleInfo.VehicleCategory.Tram when TrainDisplaySettings.IsTram:
                case VehicleInfo.VehicleCategory.Bus when TrainDisplaySettings.IsBus:
                case VehicleInfo.VehicleCategory.Trolleybus when TrainDisplaySettings.IsTrolleybus:
                case VehicleInfo.VehicleCategory.PassengerFerry when TrainDisplaySettings.IsFerry:
                case VehicleInfo.VehicleCategory.PassengerBlimp when TrainDisplaySettings.IsBlimp:
                    {
                        terminalList.Clear();

                        ushort lineId = firstVehicle.m_transportLine;
                        if (lineId == 0)
                        {
                            return false;
                        }
                        var line = TransportManager.instance.m_lines.m_buffer[lineId];

                        int stopsNumber = line.CountStops(lineId);

                        if (stopsNumber == 0)
                        {
                            return false;
                        }

                        stationIDList = new ushort[stopsNumber];
                        stationNameList = new string[stopsNumber];
                        nowPos = new LoopCounter(stopsNumber);

                        ushort nowPosId = firstVehicle.m_targetBuilding;

                        for (int i = 0; i < stopsNumber; i++)
                        {
                            ushort stationId = line.GetStop(i);
                            string stationName = StationUtils.RemoveStationSuffix(FPSCamera.FPSCamera.Utils.StationUtils.GetStationName(stationId, lineId));
                            stationIDList[i] = stationId;
                            stationNameList[i] = stationName;

                            if (nowPosId == stationId)
                            {
                                nowPos.Value = i;
                            }
                        }

                        DisplayUI.Instance.nextStation_Name = stationNameList[nowPos.Value];
                        DisplayUI.Instance.nextStation_ID = stationIDList[nowPos.Value];
                        DisplayUI.Instance.prevStation_Name = stationNameList[(nowPos - 1).Value];
                        DisplayUI.Instance.prevStation_ID = stationIDList[(nowPos - 1).Value];

                        if (!TrainDisplayMain.Instance.HasShownWarning)
                            for (int i = 0; i < stopsNumber; i++)
                            {
                                int prevIndex = (i - 1 + stopsNumber) % stopsNumber;
                                int nextIndex = (i + 1) % stopsNumber;

                                if (stationNameList[prevIndex] == stationNameList[nextIndex])
                                {
                                    terminalList.Add(i);
                                }

                            }

                        RouteUpdate();
                        DisplayUI.Instance.lineColor = line.GetColor();
                        //Log.Message("LineTrainID " + line.m_vehicles);

                        //Prevent display issues caused by duplicate station names
                        if (terminalList.Count > 2 && !TrainDisplayMain.Instance.HasShownWarning)
                        {
                            TrainDisplayMain.Instance.HasShownWarning = true;
                            DisplayUI.Instance.ShowWarning(Translations.Translate("WARNTEXT"));
                            terminalList.Clear();//disable turnback support
                        }
                        return true;
                    }

                default:
                    return false;
            }
        }

        /// <summary>
        /// Updates the route start and end indices based on the current position of the vehicle.
        /// If there are no terminals, the route spans the entire list of stations.
        /// </summary>
        private void UpdateRouteIndices()
        {
            if (terminalList.Count == 0)
            {
                routeStart = 0;
                routeEnd = stationIDList.Length - 1;
                return;
            }
            int tIndex = terminalList.BinarySearch(nowPos.Value);
            if (tIndex < 0)
            {
                tIndex = ~tIndex;
            }

            routeStart = terminalList[(tIndex - 1 + terminalList.Count) % terminalList.Count];
            routeEnd = terminalList[tIndex % terminalList.Count];
            //Log.Message("Start:" + routeStart + ", End:" + routeEnd);
        }

        /// <summary>
        /// Retrieves the indices of the stations in stationNameList that the vehicle will pass by in the current route segment.
        /// The method calculates the route from routeStart to routeEnd, including all intermediate stations.
        /// </summary>
        /// <returns>
        /// An array of integers representing the indices of the stations in stationNameList that the vehicle will pass by in this turn.
        /// The array starts with the routeStart index and ends with the routeEnd index.
        /// </returns>
        private int[] GetStationIndicesOnRoute()
        {
            List<int> tmpList = new List<int>
            {
                routeStart
            };
            LoopCounter stationIndex = new LoopCounter(stationIDList.Length, routeStart + 1);
            while (true)
            {
                tmpList.Add(stationIndex.Value);
                if (stationIndex == routeEnd)
                {
                    break;
                }
                stationIndex++;
            }
            return tmpList.ToArray();
        }

        /// <summary>
        /// Updates the route and station data in the display based on the current vehicle's route.
        /// This method recalculates the route and updates the stations to be shown on the display.
        /// </summary>
        public void RouteUpdate()
        {
            UpdateRouteIndices();
            var stopIndices = GetStationIndicesOnRoute();
            var routeStationsName = new string[stopIndices.Length];
            var routeStationsId = new ushort[stopIndices.Length];
            for (int i = 0; i < stopIndices.Length; i++)
            {
                routeStationsName[i] = stationNameList[stopIndices[i]];
                routeStationsId[i] = stationIDList[stopIndices[i]];
            }
            DisplayUI.Instance.UpdateRouteStations(routeStationsName, routeStationsId, IsCircular);
        }

        /// <summary>
        /// Updates the display when the vehicle moves to the next station.
        /// This method checks if the vehicle has reached a new station, updates the previous and next station information,
        /// and triggers a RouteUpdate() if the vehicle has reached the end of the current route.
        /// </summary>
        public void UpdateNext()
        {
            var firstVehicle = GetVehicle(GetFirstVehicleId());
            ushort nextStopId = firstVehicle.m_targetBuilding;
            string nextStopName = StationUtils.RemoveStationSuffix(FPSCamera.FPSCamera.Utils.StationUtils.GetStationName(nextStopId, firstVehicle.m_transportLine));

            DisplayUI.Instance.IsStopping = (firstVehicle.m_flags & Vehicle.Flags.Stopped) != 0;

            if (nextStopId != stationIDList[nowPos.Value])
            {
                var prevPos = nowPos.Value;
                nowPos++;
                DisplayUI.Instance.prevStation_Name = DisplayUI.Instance.nextStation_Name;
                DisplayUI.Instance.prevStation_ID = DisplayUI.Instance.nextStation_ID;
                DisplayUI.Instance.nextStation_Name = nextStopName;
                DisplayUI.Instance.nextStation_ID = nextStopId;

                if (prevPos == routeEnd)
                {
                    RouteUpdate();
                }
            }
        }
        private Vehicle GetVehicle() => VehicleManager.instance.m_vehicles.m_buffer[followId];
        private ushort GetFirstVehicleId() => GetVehicle().GetFirstVehicle(followId);
        private Vehicle GetVehicle(ushort id) => VehicleManager.instance.m_vehicles.m_buffer[id];
    }
}
