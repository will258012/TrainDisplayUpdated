extern alias FPSCamera;
using AlgernonCommons;
using AlgernonCommons.Notifications;
using AlgernonCommons.Translation;
using FPSCamera.FPSCamera.UI;
using ICities;
using TrainDisplay.Settings;
using TrainDisplay.UI;
using UnityEngine;
namespace TrainDisplay
{

    public sealed class TrainDisplayMod : OptionsMod<OptionsPanel>, IUserMod
    {
        public override string BaseName => "Train Display - Updated";
        public string Description => Translations.Translate("MOD_DESCRIPTION");

        public override void LoadSettings() => TrainDisplaySettings.Load();
        public override void SaveSettings() => TrainDisplaySettings.Save();
        public override WhatsNewMessage[] WhatsNewMessages => new WhatsNewMessage[]
      {
            new WhatsNewMessage
            {
                Version = AssemblyUtils.CurrentVersion,
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "WHATSNEW_L1",
                    "WHATSNEW_L2",
                    "WHATSNEW_L3"
                }
            }
      };
        public override void OnEnabled()
        {
            base.OnEnabled();
            Logging.EventExceptionOccured += (message) => ErrorNotification.ShowNotification(Name, 3233229958, message);
            FPSCameraAPI.Helper.CheckFPSCamera();
        }
    }
    public sealed class TrainDisplayLoading : LoadingBase<OptionsPanel>
    {
        protected override bool CreatedChecksPassed() => FPSCameraAPI.Helper.IsFPSCameraInstalledAndEnabled;
        protected override void LoadedActions(LoadMode mode)
        {
            base.LoadedActions(mode);
            gameObject = new GameObject("TrainDisplay");
            gameObject.AddComponent<DisplayUI>();
            gameObject.AddComponent<DisplayUIManager>();

        }
        public override void OnLevelUnloading()
        {
            Object.Destroy(gameObject);
            base.OnLevelUnloading();
        }
        private GameObject gameObject = null;
    }
}