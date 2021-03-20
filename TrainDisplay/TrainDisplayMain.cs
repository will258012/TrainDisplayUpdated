using ICities;
using ColossalFramework.UI;
using UnityEngine;
using TrainDisplay.UI;
using TrainDisplay.Utils;
using System;

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
		private ushort followInstance = 0;

		void Start()
		{
			displayUi = DisplayUI.Instance;
			displayUi.enabled = false;
		}

		//int debugcounter = 0;

		void Update()
        {

			// Toggle Showing
			try
			{
				bool newShowing = FPSCamera.FPSCamera.instance.vehicleCamera.following;
				if (newShowing != showingDisplay)
				{
					if (newShowing)
					{
						FPSCamera.VehicleCamera vCamera = FPSCamera.FPSCamera.instance.vehicleCamera;
						followInstance = CodeUtils.ReadPrivate<FPSCamera.VehicleCamera, ushort>(vCamera, "followInstance");
						newShowing = DisplayUIManager.Instance.SetTrain(followInstance);
					}
					displayUi.updateWidth();
					showingDisplay = newShowing;
					displayUi.enabled = newShowing;
				}
			} catch (Exception e) { }

			// When showing
			if (showingDisplay)
            {
				DisplayUIManager.Instance.updateNext();
			}
        }
	}
}
