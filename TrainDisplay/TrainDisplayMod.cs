using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace TrainDisplay
{

    public class TrainDisplayMod : IUserMod, ILoadingExtension
    {

        public string Name
        {
            get { return "Train Display Mod"; }
        }

        public string Description
        {
            get { return "I made this with VS2017"; }
        }

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