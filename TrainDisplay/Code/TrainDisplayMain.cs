extern alias FPSCamera;
using AlgernonCommons;
using FPSCamera.FPSCamera.Cam;
using TrainDisplay.UI;
using UnityEngine;
using Controller = FPSCamera.FPSCamera.Cam.Controller.FPSCamController;

namespace TrainDisplay
{

    public class TrainDisplayMain : MonoBehaviour
    {
        public static TrainDisplayMain Instance { get; set; }

        internal bool HasShownWarning = false;

        private void Awake()
        {
            Instance = this;
            DisplayUI.Instance.enabled = false;
            Logging.Message("Now Running");
            _nextUpdateTime = _nextLateUpdateTime = 0f;
        }

        private void Update()
        {
            try
            {
                if (Time.time >= _nextUpdateTime)
                {
                    _nextUpdateTime = Time.time + _updateInterval;
                    var vehicleId = GetCurrentVehicleID();
                    if (!DisplayUI.Instance.enabled && !HasShownWarning)
                    {
                        if (vehicleId != default && DisplayUIManager.Instance.SetDisplay(vehicleId))
                        {
                            DisplayUI.Instance.enabled = true;
                            StartCoroutine(DisplayUI.Instance.UpdateWidth());
                        }
                    }
                    else
                    {
                        if (vehicleId == default)
                        {
                            DisplayUI.Instance.enabled = HasShownWarning = false;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Logging.LogException(e);
            }
        }

        private void LateUpdate()
        {
            try
            {
                if (DisplayUI.Instance.enabled && Time.time >= _nextLateUpdateTime)
                {
                    _nextLateUpdateTime = Time.time + _updateInterval;
                    DisplayUIManager.Instance.UpdateNext();
                }
            }
            catch (System.Exception e)
            {
                Logging.LogException(e);
            }
        }

        private ushort GetCurrentVehicleID()
        {
            var cam = Controller.Instance.FPSCam;
            if (cam == null || !cam.IsActivated)
                return default;

            switch (cam)
            {
                case CitizenCam citizenCam:
                    return (ushort)(citizenCam.AnotherCam?.FollowID ?? default);

                case VehicleCam vehicleCam when vehicleCam.FollowID != default:
                    return (ushort)vehicleCam.FollowID;

                case WalkThruCam walkThruCam:
                    if (walkThruCam.CurrentCam is VehicleCam)
                        return (ushort)walkThruCam.FollowID;
                    else if (walkThruCam.CurrentCam is CitizenCam citizenCam1)
                        return (ushort)(citizenCam1.AnotherCam?.FollowID ?? default);
                    break;
            }

            return default;
        }
        private const float _updateInterval = .25f;
        private float _nextUpdateTime;
        private float _nextLateUpdateTime;
    }
}
