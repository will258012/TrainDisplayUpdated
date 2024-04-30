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

        public static void Initialize()
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

        void Awake() { }

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
            // Toggle Showing
            ushort vehicleID = CSkyL.ModSupport.FollowVehicleID;
            bool newShowing = vehicleID != default;

            if (newShowing != showingDisplay)
            {
                if (newShowing)
                {
                    newShowing = DisplayUIManager.Instance.SetTrain(vehicleID);
                }

                displayUi.updateWidth();
                showingDisplay = newShowing;
                displayUi.enabled = newShowing;
            }
            // When showing
            if (showingDisplay)
            {
                DisplayUIManager.Instance.updateNext();
            }

        }
    }
}
