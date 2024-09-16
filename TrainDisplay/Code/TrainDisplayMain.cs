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

        private bool IsShowingDisplay = false;
        internal bool HasShownWarning = false;

        private void Awake()
        {
            Instance = this;
            DisplayUI.Instance.enabled = false;
            Logging.Message("Now Running");
        }

        private void Update()
        {
            try
            {
                // Toggle Showing
                var vehicleId = GetCurrentVehicleID();
                var status = false;
                if (vehicleId != default)
                    // Perform check
                    status = DisplayUIManager.Instance.SetDisplay(vehicleId);

                if (status != IsShowingDisplay)
                {
                    DisplayUI.Instance.enabled = IsShowingDisplay = status;
                    if (IsShowingDisplay)
                    {
                        StartCoroutine(DisplayUI.Instance.UpdateWidth());
                    }
                    else
                    {
                        HasShownWarning = false;
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
                if (IsShowingDisplay)
                {
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
    }
}
