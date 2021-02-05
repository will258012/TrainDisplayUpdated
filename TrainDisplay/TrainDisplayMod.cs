using ICities;
using UnityEngine;
using ColossalFramework.UI;
using TrainDisplay.TranslationFramework;

namespace TrainDisplay
{

    public class TrainDisplayMod : IUserMod, ILoadingExtension
    {

        public static Translation translation = new Translation();

        public string Name
        {
            get { return "Train Display Mod"; }
        }

        public string Description
        {
            get { return "Japanese Style Train Display"; }
        }

        /*
        public void OnSettingsUI(UIHelperBase helper)
        {
            TrainDisplayConfiguration config = Configuration<TrainDisplayConfiguration>.Load();

            helper.AddSlider("Display Width", 128, 2048, 1, config.DisplayWidth, width =>
            {
                config.DisplayWidth = (int)width;
                Configuration<TrainDisplayConfiguration>.Save();
            });
        }
        */

        public void OnCreated(ILoading loading)
        {
        }
        public void OnLevelLoaded(LoadMode mode)
        {
            TrainDisplayMain.Initialize(mode);
        }

        public void OnLevelUnloading()
        {
            TrainDisplayMain.Deinitialize();
        }
        public void OnReleased()
        {
        }
    }
}