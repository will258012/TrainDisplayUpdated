extern alias FPSCamera;
using FPSCamera::FPSCamera.Cam;
using Controller = FPSCamera::FPSCamera.Cam.Controller.FPSCamController;
using TrainDisplay.UI;
using UnityEngine;
using AlgernonCommons;

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

            // Toggle Showing
            var cam = Controller.Instance.FPSCam;
            // Perform check
            var status = cam != null && 
                cam.IsActivated && 
                cam is VehicleCam vehicle && 
                vehicle.FollowID != default && 
                DisplayUIManager.Instance.SetDisplay((ushort) vehicle.FollowID);

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
        private void LateUpdate()
        {
            if (IsShowingDisplay)
            {
                DisplayUIManager.Instance.UpdateNext();
            }
        }
    }
}
