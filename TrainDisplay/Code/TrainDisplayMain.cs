extern alias FPSCamera;
using AlgernonCommons;
using FPSCamera.FPSCamera.Utils;
using TrainDisplay.UI;
using UnityEngine;

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
            _nextUpdateTime = _nextLateUpdateTime = 0f;
            FPSCamera.FPSCamera.Cam.Controller.FPSCamController.Instance.OnCameraEnabled += OnCameraEnabled;
        }
        private void OnDestroy() => FPSCamera.FPSCamera.Cam.Controller.FPSCamController.Instance.OnCameraEnabled -= OnCameraEnabled;
        private void Update()
        {
            try
            {
                // If it's time to perform the next update based on the time interval,
                if (Time.time >= _nextUpdateTime)
                {
                    // Set the next update time
                    _nextUpdateTime = Time.time + _updateInterval;

                    // Retrieve the ID of the vehicle currently being followed
                    var vehicleId = ModSupport.FollowVehicleID;

                    // If the UI is not enabled and a warning hasn't been shown,
                    if (!DisplayUI.Instance.enabled && !HasShownWarning)
                    {
                        // Check if a valid vehicle is being followed and it's different from the current follow ID.
                        if (vehicleId != default && vehicleId != DisplayUIManager.Instance.FollowId)
                        {
                            // Enable the UI and update the display if a new vehicle is being followed.
                            DisplayUI.Instance.enabled = DisplayUIManager.Instance.SetDisplay(vehicleId);

                            // If the UI is enabled, start a coroutine to update the display width.
                            if (DisplayUI.Instance.enabled)
                                StartCoroutine(DisplayUI.Instance.UpdateWidth());
                        }
                    }
                    else
                    {
                        // Otherwise, If the vehicle ID is default (no vehicle is followed),
                        if (vehicleId == default)
                        {
                            // Disable self, the UI, and reset the warning flag.  
                            enabled = DisplayUI.Instance.enabled = HasShownWarning = false;
                            Logging.Message("TrainDisplayMain is disabled");
                        }
                        // If a new vehicle is being followed,
                        else if (vehicleId != DisplayUIManager.Instance.FollowId)
                        {
                            // Otherwise, Update the display with the new vehicle ID and reset the warning flag.
                            DisplayUI.Instance.enabled = DisplayUIManager.Instance.SetDisplay(vehicleId);
                            HasShownWarning = false;
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
                // If it's time to perform the next update based on the time interval and the UI is enabled,
                if (Time.time >= _nextLateUpdateTime && DisplayUI.Instance.enabled)
                {
                    // Set the next update time.
                    _nextLateUpdateTime = Time.time + _updateInterval;
                    // Trigger update.
                    DisplayUIManager.Instance.UpdateNext();
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
            Logging.Message("TrainDisplayMain is enabled");
        }
        private const float _updateInterval = .25f;
        private float _nextUpdateTime;
        private float _nextLateUpdateTime;
    }
}
