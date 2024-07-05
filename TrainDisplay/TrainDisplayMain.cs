using TrainDisplay.UI;
using UnityEngine;

namespace TrainDisplay
{

    public class TrainDisplayMain : MonoBehaviour
    {

        public static TrainDisplayMain instance;

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

        void Update()
        {
            // Toggle Showing
            ushort vehicleID = CSkyL.ModSupport.FollowVehicleID;
            //vehicleID != default : check if fpscam is following a vehicle
            //DisplayUIManager.Instance.SetTrain(vehicleID) : check if it is a train which is need to display
            bool newShowing = (vehicleID != default) && DisplayUIManager.Instance.SetTrain(vehicleID);
            if (newShowing != showingDisplay)
            {
                displayUi.enabled = showingDisplay = newShowing;
                if (showingDisplay) StartCoroutine(displayUi.UpdateWidth());
            }
        }
        void LateUpdate()
        {
            if (showingDisplay)
            {
                DisplayUIManager.Instance.updateNext();
            }
        }
    }
}
