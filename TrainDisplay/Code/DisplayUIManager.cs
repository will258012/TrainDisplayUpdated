extern alias FPSCamera;
using AlgernonCommons;
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
            DisplayUI.Instance.enabled = false;
            _nextUpdateTime = 0f;
            FPSCamera.FPSCamera.Cam.Controller.FPSCamController.Instance.OnCameraEnabled += OnCameraEnabled;
            FPSCamera.FPSCamera.Cam.Controller.FPSCamController.Instance.OnCameraDisabled += OnCameraDisabled;
        }
        private void OnDestroy()
        {
            FPSCamera.FPSCamera.Cam.Controller.FPSCamController.Instance.OnCameraEnabled -= OnCameraEnabled;
            FPSCamera.FPSCamera.Cam.Controller.FPSCamController.Instance.OnCameraDisabled -= OnCameraDisabled;
            Destroy(DisplayUI.Instance);
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

                    // If the UI is not enabled and a warning hasn't been shown,
                    if (!DisplayUI.Instance.enabled && !_hasShownWarning)
                    {
                        // Check if a valid vehicle is being followed and it's different from the current follow ID.
                        if (vehicleId != default && vehicleId != FollowId)
                        {
                            // Enable the UI and update the display if a new vehicle is being followed.
                            DisplayUI.Instance.enabled = SetDisplay(vehicleId);

                            // If the UI is enabled, start a coroutine to update the display width.
                            if (DisplayUI.Instance.enabled)
                                StartCoroutine(DisplayUI.Instance.UpdateWidth());
                        }
                    }
                    else
                    {
                        // If the vehicle ID changed,
                        if (vehicleId != FollowId)
                        {
                            //Otherwise, If the vehicle ID is default (no vehicle is followed),
                            if (vehicleId == default)
                            {
                                // Disable the UI, and reset the warning flag.  
                                DisplayUI.Instance.enabled = false;
                                FollowId = default;
                            }
                            else
                            {
                                // Otherwise, Update the display with the new vehicle ID and reset the warning flag.
                                DisplayUI.Instance.enabled = SetDisplay(vehicleId);
                            }
                            _hasShownWarning = false;
                        }
                        UpdateNext();
                    }
                }
            }
            catch (System.Exception e)
            {
                Logging.LogException(e);
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
            enabled = DisplayUI.Instance.enabled = _hasShownWarning = false;
            FollowId = default;
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
                    {
                        _terminalList.Clear();

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

                        _stationIDList = new ushort[stopsNumber];
                        _stationNameList = new string[stopsNumber];
                        _nowPos = new LoopCounter(stopsNumber);

                        ushort nowPosId = firstVehicle.m_targetBuilding;

                        for (int i = 0; i < stopsNumber; i++)
                        {
                            ushort stationId = line.GetStop(i);
                            string stationName = StationUtils.RemoveStationSuffix(FPSCamera.FPSCamera.Utils.StationUtils.GetStationName(stationId, lineId));
                            _stationIDList[i] = stationId;
                            _stationNameList[i] = stationName;

                            if (nowPosId == stationId)
                            {
                                _nowPos.Value = i;
                            }
                        }

                        DisplayUI.Instance.nextStation_Name = _stationNameList[_nowPos.Value];
                        DisplayUI.Instance.nextStation_ID = _stationIDList[_nowPos.Value];
                        DisplayUI.Instance.prevStation_Name = _stationNameList[(_nowPos - 1).Value];
                        DisplayUI.Instance.prevStation_ID = _stationIDList[(_nowPos - 1).Value];

                        if (!_hasShownWarning)
                            for (int i = 0; i < stopsNumber; i++)
                            {
                                int prevIndex = (i - 1 + stopsNumber) % stopsNumber;
                                int nextIndex = (i + 1) % stopsNumber;

                                if (_stationNameList[prevIndex] == _stationNameList[nextIndex])
                                {
                                    _terminalList.Add(i);
                                }

                            }

                        RouteUpdate();
                        DisplayUI.Instance.lineColor = line.GetColor();

                        //Prevent display issues caused by duplicate station names
                        if (_terminalList.Count > 2 && !_hasShownWarning)
                        {
                            _hasShownWarning = true;
                            DisplayUI.Instance.ShowWarning(Translations.Translate("WARNTEXT"));
                            _terminalList.Clear();//disable turnback support
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
            if (_terminalList.Count == 0)
            {
                _routeStart = 0;
                _routeEnd = _stationIDList.Length - 1;
                return;
            }
            int tIndex = _terminalList.BinarySearch(_nowPos.Value);
            if (tIndex < 0)
            {
                tIndex = ~tIndex;
            }

            _routeStart = _terminalList[(tIndex - 1 + _terminalList.Count) % _terminalList.Count];
            _routeEnd = _terminalList[tIndex % _terminalList.Count];
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
                _routeStart
            };
            LoopCounter stationIndex = new LoopCounter(_stationIDList.Length, _routeStart + 1);
            while (true)
            {
                tmpList.Add(stationIndex.Value);
                if (stationIndex == _routeEnd)
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
                routeStationsName[i] = _stationNameList[stopIndices[i]];
                routeStationsId[i] = _stationIDList[stopIndices[i]];
            }
            DisplayUI.Instance.UpdateRouteStations(routeStationsName, routeStationsId, IsCircular);
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

            if (nextStopId != _stationIDList[_nowPos.Value])
            {
                var prevPos = _nowPos.Value;
                _nowPos++;
                DisplayUI.Instance.prevStation_Name = DisplayUI.Instance.nextStation_Name;
                DisplayUI.Instance.prevStation_ID = DisplayUI.Instance.nextStation_ID;
                DisplayUI.Instance.nextStation_Name = nextStopName;
                DisplayUI.Instance.nextStation_ID = nextStopId;

                if (prevPos == _routeEnd)
                {
                    RouteUpdate();
                }
            }
        }
        private Vehicle GetVehicle() => VehicleManager.instance.m_vehicles.m_buffer[FollowId];
        private ushort GetFirstVehicleId() => GetVehicle().GetFirstVehicle(FollowId);
        private Vehicle GetVehicle(ushort id) => VehicleManager.instance.m_vehicles.m_buffer[id];
        private bool IsCircular => _terminalList.Count == 0;

        private const float _updateInterval = .25f;
        private float _nextUpdateTime;
        private ushort[] _stationIDList; //The internal value should be unique
        private string[] _stationNameList;
        private List<int> _terminalList = new List<int>();//Turnback display support
        private LoopCounter _nowPos;
        private int _routeStart;
        private int _routeEnd;
        private bool _hasShownWarning = false;
    }
}
