using ICities;
using UnityEngine;
using TrainDisplay.Utils;
using TrainDisplay.UI;
using System.Collections.Generic;

namespace TrainDisplay
{
    public class DisplayUIManager
    {
        VehicleManager vManager;
        TransportManager tManager;
        ushort followInstance;

        ushort[] stationIdList;
        string[] stationNameList;
        List<int> terminalList;
        int nowPos;
        int routeStart;
        int routeEnd;

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
            terminalList = new List<int>();
        }
        public bool SetTrain(ushort followInstance)
        {
            this.followInstance = followInstance;
            ushort firstVehicleId = vManager.m_vehicles.m_buffer[(int)followInstance].GetFirstVehicle(followInstance);
            Vehicle firstVehicle = vManager.m_vehicles.m_buffer[firstVehicleId];

            VehicleInfo info = firstVehicle.Info;

            if (info.m_vehicleType == VehicleInfo.VehicleType.Train || info.m_vehicleType == VehicleInfo.VehicleType.Metro) {
                DisplayUI.Instance.testString = "";
                terminalList.Clear();

                ushort lineId = firstVehicle.m_transportLine;
                TransportLine line = tManager.m_lines.m_buffer[lineId];
                TransportInfo tInfo = line.Info;
                int stopsNumber = line.CountStops(lineId);

                stationIdList = new ushort[stopsNumber];
                stationNameList = new string[stopsNumber];

                ushort nextStopId = firstVehicle.m_targetBuilding;
                ushort nowPosId = TransportLine.GetPrevStop(nextStopId);


                // 駅リストを作成
                for (int i = 0; i < stopsNumber; i++)
                {
                    ushort stationId = line.GetStop(i);
                    string stationName = StationUtils.GetStationName(stationId);
                    stationIdList[i] = stationId;
                    stationNameList[i] = StationUtils.removeStationSuffix(stationName);
                    DisplayUI.Instance.testString += stationName + " ";
                    
                    if (nowPosId == stationId)
                    {
                        nowPos = i;
                    }
                }

                
                // 終点駅リストを作成
                for (int i = 0; i < stopsNumber; i++)
                {
                    int prevIndex = (i - 1 + stopsNumber) % stopsNumber;
                    int nextIndex = (i + 1) % stopsNumber;

                    if (stationNameList[prevIndex] == stationNameList[nextIndex])
                    {
                        terminalList.Add(i);
                    }
                }


                routeUpdate();

                Log.Message(DisplayUI.Instance.testString);
                Log.Message("LineTrainID " + line.m_vehicles);

                return true;
            } else
            {
                return false;
            }
        }
        
        public void UpdateRouteIndices()
        {
            if (terminalList.Count == 0)
            {
                routeStart = 0;
                routeEnd = 0;
                return;
            }
            int tIndex = terminalList.BinarySearch(nowPos);
            if (tIndex < 0)
            {
                tIndex = ~tIndex;
            }
            Log.Message("TIndex: " + tIndex);

            routeStart = terminalList[(tIndex - 1 + terminalList.Count) % terminalList.Count];
            routeEnd = terminalList[tIndex % terminalList.Count];
            Log.Message("Start:" + routeStart + ", End:" + routeEnd);
        }
        

        public int[] GetStationIndicesOnRoute()
        {
            List<int> tmpList = new List<int>();
            tmpList.Add(routeStart);
            int stationIndex = (routeStart + 1) % stationNameList.Length;
            while (true)
            {
                tmpList.Add(stationIndex);
                if (stationIndex == routeEnd)
                {
                    break;
                }
                stationIndex = (stationIndex + 1) % stationNameList.Length;
            }
            return tmpList.ToArray();
        }

        void routeUpdate()
        {
            UpdateRouteIndices();
            DisplayUI.Instance.testString = "";
            var stopIndices = GetStationIndicesOnRoute();
            bool firstFlag = true;
            foreach (var stop in stopIndices)
            {
                Log.Message("Stop:" + stop);
                if (!firstFlag)
                {
                    DisplayUI.Instance.testString += " => ";
                }
                DisplayUI.Instance.testString += stationNameList[stop];
                firstFlag = false;
            }
            DisplayUI.Instance.forText = stationNameList[routeEnd];
        }

        public void updateNext()
        {
            ushort firstVehicleId = vManager.m_vehicles.m_buffer[(int)followInstance].GetFirstVehicle(followInstance);
            Vehicle trainVehicle = vManager.m_vehicles.m_buffer[firstVehicleId];
            ushort nextStop = trainVehicle.m_targetBuilding;
            string targetBuilding = StationUtils.removeStationSuffix(StationUtils.GetStationName(nextStop));

            if (targetBuilding != stationNameList[nowPos])
            {
                int beforePos = nowPos;
                nowPos = (nowPos + 1) % stationNameList.Length;
                DisplayUI.Instance.next = targetBuilding;

                if (beforePos == routeEnd)
                {
                    routeUpdate();
                }
            }
        }
    }
}
