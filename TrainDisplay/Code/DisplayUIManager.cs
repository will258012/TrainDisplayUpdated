extern alias FPSCamera;
using AlgernonCommons;
using AlgernonCommons.Notifications;
using AlgernonCommons.Translation;
using ColossalFramework;
using System.Collections.Generic;
using TrainDisplay.Settings;
using TrainDisplay.UI;
using TrainDisplay.Utils;
using UnityEngine;

namespace TrainDisplay
{
    public class DisplayUIManager : MonoBehaviour
    {
        public static DisplayUIManager Instance { get; private set; }
        public ushort FollowId { get; private set; }
        private void Awake()
        {
            Instance = this;
            DisplayUI.Instance.enabled = enabled = false;
            FPSCamera.FPSCamera.Cam.Controller.FPSCamController.OnCameraEnabled += OnCameraEnabled;
            FPSCamera.FPSCamera.Cam.Controller.FPSCamController.OnCameraDisabled += OnCameraDisabled;
        }
        private void OnDestroy()
        {
            FPSCamera.FPSCamera.Cam.Controller.FPSCamController.OnCameraEnabled -= OnCameraEnabled;
            FPSCamera.FPSCamera.Cam.Controller.FPSCamController.OnCameraDisabled -= OnCameraDisabled;
        }
        private void Update()
        {
            try
            {
                // If it's time to perform the next update based on the time interval,
                if (Time.time >= _nextUpdateTime)
                {
                    // Set the next update time.
                    _nextUpdateTime = Time.time + _updateInterval;

                    // Retrieve the ID of the vehicle currently being followed.
                    var vehicleId = FPSCamera.FPSCamera.Utils.ModSupport.FollowVehicleID;
                    // If the UI is not enabled,
                    if (!DisplayUI.Instance.enabled)
                    {
                        // Check if a valid vehicle is being followed and it's different from the current follow ID, and a warning hasn't been shown,
                        if (vehicleId != default && vehicleId != FollowId && !hasShownWarning)
                        {
                            // Enable the UI and update the display if a new vehicle is being followed.
                            DisplayUI.Instance.enabled = SetDisplay(vehicleId);
                        }
                    }
                    else
                    {
                        // If the vehicle ID changed,
                        if (vehicleId != FollowId)
                        {
                            // If the vehicle ID is default (no vehicle is followed),
                            if (vehicleId == default)
                            {
                                // Disable the UI.  
                                DisplayUI.Instance.enabled = false;
                                FollowId = default;
                            }
                            else
                            {
                                // Otherwise, Update the display with the new vehicle ID.
                                DisplayUI.Instance.enabled = SetDisplay(vehicleId);
                            }
                        }
                        UpdateNext();
                    }
                }
            }
            catch (System.Exception e)
            {

                var notification = NotificationBase.ShowNotification<ListNotification>();
                notification.AddParas(Translations.Translate("ERROR"));
                notification.AddSpacer();
                notification.AddParas(e.ToString());

                Logging.LogException(e, "Train Display is disabled due to some issues");
                enabled = DisplayUI.Instance.enabled = hasShownWarning = false;
            }
        }
        private void OnCameraEnabled()
        {
            enabled = true;
            Logging.Message("FPSCamera enabled, DisplayUIManager is enabled");
        }
        private void OnCameraDisabled()
        {
            // Disable self, the UI, and reset the warning flag.  
            enabled = DisplayUI.Instance.enabled = hasShownWarning = false;
            FollowId = default;
            TTSHelper.Instance.Stop();
            Logging.Message("FPSCamera disabled, DisplayUIManager is disabled");
        }

        /// <summary>
        /// Initializes and sets up the display for the current vehicle instance, including station data and terminal checks.
        /// </summary>
        /// <param name="followId">The vehicle id of the vehicle to follow.</param>
        /// <returns>
        /// Returns true if the display setup is successful, otherwise returns false.
        /// </returns>
        public bool SetDisplay(ushort followId)
        {
            Logging.Message("Setting display");
            if (followId == default) return false;
            FollowId = followId;
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
                case VehicleInfo.VehicleCategory.PassengerCopter when TrainDisplaySettings.IsCopter:
                    {


                        ushort lineId = firstVehicle.m_transportLine;
                        if (lineId == 0) return false;
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
                        DisplayUI.Instance.LineColor = line.GetColor();
                        lineName = TransportManager.instance.GetLineName(lineId);

                        terminalList.Clear();
                        if (!hasShownWarning)
                        {
                            var counter = new LoopCounter(stopsNumber);
                            for (int i = 0; i < stopsNumber; i++)
                            {
                                int prevIndex = (counter - 1).Value;
                                int nextIndex = (counter + 1).Value;

                                var pos1 = NetManager.instance.m_nodes.m_buffer[stationIDList[prevIndex]].m_position;
                                var pos2 = NetManager.instance.m_nodes.m_buffer[stationIDList[nextIndex]].m_position;

                                if (Vector3.Distance(pos1, pos2) <= 50f)
                                {
                                    terminalList.Add(i);
                                }
                                counter++;
                            }

                            //Prevent display issues caused by more than 2 terminal stations
                            if (terminalList.Count > 2)
                            {
                                hasShownWarning = true;
                                DisplayUI.Instance.ShowWarning(Translations.Translate("WARNTEXT"));
                                terminalList.Clear();//disable terminal stations display support
                            }
                        }

                        RouteUpdate();
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

            LoopCounter loopCounterStart = new LoopCounter(terminalList.Count, tIndex - 1);
            LoopCounter loopCounterEnd = new LoopCounter(terminalList.Count, tIndex);

            routeStart = terminalList[loopCounterStart.Value];
            routeEnd = terminalList[loopCounterEnd.Value];
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
            DisplayUI.Instance.IsCircular = IsCircular;
            DisplayUI.Instance.UpdateRouteStations(routeStationsName, routeStationsId);
        }

        /// <summary>
        /// Updates the display when the vehicle moves to the next station.
        /// This method checks if the vehicle has reached a new station, updates the previous and next station information,
        /// and triggers a <see cref="RouteUpdate()"/> if the vehicle has reached the end of the current route.
        /// </summary>
        public void UpdateNext()
        {
            var firstVehicle = GetVehicle(GetFirstVehicleId());
            ushort nextStopId = firstVehicle.m_targetBuilding;
            string nextStopName = StationUtils.RemoveStationSuffix(FPSCamera.FPSCamera.Utils.StationUtils.GetStationName(nextStopId, firstVehicle.m_transportLine));

            DisplayUI.Instance.IsStopping = firstVehicle.m_flags.IsFlagSet(Vehicle.Flags.Stopped);

            if (nextStopId != stationIDList[nowPos.Value])
            {
                hasSpokenDepart = false;
                var prevPos = nowPos.Value;
                nowPos++;
                DisplayUI.Instance.prevStation_Name = DisplayUI.Instance.nextStation_Name;
                DisplayUI.Instance.prevStation_ID = DisplayUI.Instance.nextStation_ID;
                DisplayUI.Instance.nextStation_Name = nextStopName;
                DisplayUI.Instance.nextStation_ID = nextStopId;
                TTSHelper.Instance.Speak(string.Format(TrainDisplaySettings.TTSArriving, DisplayUI.Instance.prevStation_Name, DisplayUI.Instance.ForDisplayedText, lineName));
                if (prevPos == routeEnd)
                {
                    RouteUpdate();
                }
            }
            else if (!DisplayUI.Instance.IsStopping && !hasSpokenDepart)
            {
                TTSHelper.Instance.Speak(string.Format(TrainDisplaySettings.TTSDeparting, DisplayUI.Instance.nextStation_Name, DisplayUI.Instance.ForDisplayedText, lineName));
                hasSpokenDepart = true;
            }

        }
        private Vehicle GetVehicle() => VehicleManager.instance.m_vehicles.m_buffer[FollowId];
        private ushort GetFirstVehicleId() => GetVehicle().GetFirstVehicle(FollowId);
        private Vehicle GetVehicle(ushort id) => VehicleManager.instance.m_vehicles.m_buffer[id];
        private bool IsCircular => terminalList.Count == 0;

        private const float _updateInterval = .25f;
        private float _nextUpdateTime = default;
        private ushort[] stationIDList; //The internal value should be unique
        private string[] stationNameList;
        private List<int> terminalList = new List<int>();//Turnback display support
        private LoopCounter nowPos;
        private int routeStart;
        private int routeEnd;
        private bool hasShownWarning = false;
        private bool hasSpokenDepart = false;
        private string lineName;
    }
}
