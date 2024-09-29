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
                    var vehicleId = ModSupport.FollowVehicleID;
                    if (!DisplayUI.Instance.enabled && !HasShownWarning)
                    {
                        if (vehicleId != default && vehicleId != DisplayUIManager.Instance.FollowId)
                        {
                            DisplayUI.Instance.enabled = DisplayUIManager.Instance.SetDisplay(vehicleId);
                            if (DisplayUI.Instance.enabled) StartCoroutine(DisplayUI.Instance.UpdateWidth());
                        }
                    }
                    else
                    {
                        if (vehicleId != DisplayUIManager.Instance.FollowId)
                        {
                            DisplayUI.Instance.enabled = DisplayUIManager.Instance.SetDisplay(vehicleId);
                            HasShownWarning = false;
                        }
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

        private const float _updateInterval = .25f;
        private float _nextUpdateTime;
        private float _nextLateUpdateTime;
    }
}
