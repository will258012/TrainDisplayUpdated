using ICities;
using ColossalFramework.UI;
using UnityEngine;
using TrainDisplay.UI;
using TrainDisplay.Utils;

namespace TrainDisplay
{

	public class TrainDisplayMain : MonoBehaviour
	{

		public static TrainDisplayMain instance;
		public static DisplayUIManager displayUiManager;
		public static void Initialize(LoadMode mode)
		{
			GameObject gameObject = new GameObject();
			instance = gameObject.AddComponent<TrainDisplayMain>();
			displayUiManager = gameObject.AddComponent<DisplayUIManager>();
		}

		public static void Deinitialize()
		{
			Destroy(instance);
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
			//debugcounter++;
			//if (debugcounter >= 1000)
            //{
			//	debugcounter = 0;
			//	Debug.Log(String.Format("[TrainDisplay] {0} {1}", FPSCamera.FPSCamera.IsEnabled(), FPSCamera.FPSCamera.instance.vehicleCamera.following)
            //}
			bool newShowing = FPSCamera.FPSCamera.instance.vehicleCamera.following;
			if (newShowing != showingDisplay)
            {
				showingDisplay = newShowing;
				displayUi.enabled = showingDisplay;
				if (showingDisplay)
                {
					FPSCamera.VehicleCamera vCamera = FPSCamera.FPSCamera.instance.vehicleCamera;
					displayUiManager.SetTrain(CodeUtils.ReadPrivate<FPSCamera.VehicleCamera, ushort>(vCamera, "followInstance"));
                }
			}
        }
	}
}
