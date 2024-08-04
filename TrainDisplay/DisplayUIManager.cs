using CSkyL.Game.Utils;
using System.Collections.Generic;
using TrainDisplay.Config;
using TrainDisplay.UI;
using TrainDisplay.Utils;

namespace TrainDisplay
{
    public class DisplayUIManager
    {
        readonly VehicleManager vManager;
        readonly TransportManager tManager;
        ushort followInstance;
        ushort[] stationIdList;
        string[] stationNameList;
        readonly List<int> terminalList = new List<int>();
        LoopCounter nowPos;
        int routeStart;
        int routeEnd;

        bool Circular => terminalList.Count == 0;

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

        private DisplayUIManager()
        {
            vManager = VehicleManager.instance;
            tManager = TransportManager.instance;
        }
        public bool SetTrain(ushort followInstance)
        {
            this.followInstance = followInstance;
            ushort firstVehicleId = vManager.m_vehicles.m_buffer[followInstance].GetFirstVehicle(followInstance);
            Vehicle firstVehicle = vManager.m_vehicles.m_buffer[firstVehicleId];

            VehicleInfo info = firstVehicle.Info;

            switch (info.vehicleCategory)
            {
                case VehicleInfo.VehicleCategory.PassengerTrain when TrainDisplayConfig.Instance.IsTrain:
                case VehicleInfo.VehicleCategory.MetroTrain when TrainDisplayConfig.Instance.IsMetro:
                case VehicleInfo.VehicleCategory.Monorail when TrainDisplayConfig.Instance.IsMonorail:
                case VehicleInfo.VehicleCategory.Tram when TrainDisplayConfig.Instance.IsTram:
                case VehicleInfo.VehicleCategory.Bus when TrainDisplayConfig.Instance.IsBus:
                case VehicleInfo.VehicleCategory.Trolleybus when TrainDisplayConfig.Instance.IsTrolleybus:
                case VehicleInfo.VehicleCategory.PassengerFerry when  TrainDisplayConfig.Instance.IsFerry:
                case VehicleInfo.VehicleCategory.PassengerBlimp when TrainDisplayConfig.Instance.IsBlimp:
                {
                        terminalList.Clear();

                        ushort lineId = firstVehicle.m_transportLine;
                        if (lineId == 0)
                        {
                            return false;
                        }
                        TransportLine line = tManager.m_lines.m_buffer[lineId];

                        int stopsNumber = line.CountStops(lineId);

                        if (stopsNumber == 0)
                        {
                            return false;
                        }

                        stationIdList = new ushort[stopsNumber];
                        stationNameList = new string[stopsNumber];
                        nowPos = new LoopCounter(stopsNumber);

                        ushort nowPosId = firstVehicle.m_targetBuilding;

                        for (int i = 0; i < stopsNumber; i++)
                        {
                            ushort stationId = line.GetStop(i);
                            string stationName = StationUtils.RemoveStationSuffix(StationUtil.GetStationName(stationId));
                            stationIdList[i] = stationId;
                            stationNameList[i] = stationName;

                            if (nowPosId == stationId)
                            {
                                nowPos.Value = i;
                            }
                        }

                        DisplayUI.Instance.next = stationNameList[nowPos.Value];
                        DisplayUI.Instance.prevText = stationNameList[(nowPos - 1).Value];

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

                        return true;
                    }

                default:
                    return false;
            }
        }


        public void UpdateRouteIndices()
        {
            if (terminalList.Count == 0)
            {
                routeStart = 0;
                routeEnd = stationNameList.Length - 1;
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


        public int[] GetStationIndicesOnRoute()
        {
            List<int> tmpList = new List<int>
            {
                routeStart
            };
            LoopCounter stationIndex = new LoopCounter(stationNameList.Length, routeStart + 1);
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

        public void RouteUpdate()
        {
            UpdateRouteIndices();
            var stopIndices = GetStationIndicesOnRoute();
            var routeStations = new string[stopIndices.Length];
            for (int i = 0; i < stopIndices.Length; i++)
            {
                routeStations[i] = stationNameList[stopIndices[i]];
            }
            DisplayUI.Instance.UpdateRouteStations(routeStations, Circular);
        }

        public void UpdateNext()
        {
            ushort firstVehicleId = vManager.m_vehicles.m_buffer[followInstance].GetFirstVehicle(followInstance);
            Vehicle trainVehicle = vManager.m_vehicles.m_buffer[firstVehicleId];
            ushort nextStop = trainVehicle.m_targetBuilding;
            string targetBuilding = StationUtils.RemoveStationSuffix(StationUtil.GetStationName(nextStop));

            DisplayUI.Instance.stopping = (trainVehicle.m_flags & Vehicle.Flags.Stopped) != 0;

            if (targetBuilding != stationNameList[nowPos.Value])
            {
                int beforePos = nowPos.Value;
                nowPos++;
                DisplayUI.Instance.prevText = DisplayUI.Instance.next;
                DisplayUI.Instance.next = targetBuilding;

                if (beforePos == routeEnd)
                {
                    RouteUpdate();
                }
            }
        }
    }
}
