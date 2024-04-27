using ICities;
using System;
using TrainDisplay.UI;
using TrainDisplay.Utils;
using UnityEngine;

namespace TrainDisplay
{

    public class TrainDisplayMain : MonoBehaviour
    {

        public static TrainDisplayMain instance;
        private static TrainDisplayConfiguration config;
        public static TrainDisplayConfiguration Config
        {
            get
            {
                if (config == null)
                {
                    config = Configuration<TrainDisplayConfiguration>.Load();
                }
                return config;
            }
        }

        VehicleManager vManager;
        public static void Initialize(LoadMode mode)
        {
            GameObject gameObject = new GameObject();
            instance = gameObject.AddComponent<TrainDisplayMain>();

        }

        public static void Deinitialize()
        {
            if (instance != null)
            {
                Destroy(instance);
            }
        }

        void Awake()
        {
            vManager = VehicleManager.instance;
        }

        public DisplayUI displayUi;
        private bool showingDisplay = false;

        void Start()
        {
            displayUi = DisplayUI.Instance;
            displayUi.enabled = false;
        }

        //int debugcounter = 0;

        void Update()
        {
            var _ID = CSkyL.ModSupport.FollowVehicleID;
            var newShowing = _ID != default;
            // Toggle Showing
            try
            {
                if (newShowing != showingDisplay)
                {
                    if (newShowing)
                    {
                        newShowing = DisplayUIManager.Instance.SetTrain(_ID);
                        Log.Message($"Start to show display:{_ID}");
                    }
                    displayUi.updateWidth();
                    showingDisplay = newShowing;
                    displayUi.enabled = newShowing;
                }
            }
            catch (Exception e) { }

            // When showing
            if (showingDisplay)
            {
                DisplayUIManager.Instance.updateNext();
            }
        }
    }
}
