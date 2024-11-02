﻿using AlgernonCommons;
using AlgernonCommons.Notifications;
using AlgernonCommons.Translation;
using HarmonyLib;
using ICities;
using TrainDisplay.Settings;
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
    }
    public sealed class TrainDisplayLoading : LoadingBase<OptionsPanel>
    {
        protected override bool CreatedChecksPassed()
        {
            if (AccessTools.TypeByName("FPSCamera.Utils.ModSupport, FPSCamera") == null)
            {
                Logging.Error("FPScamera not detected");
                WasFPSCameraNotFound = true;
                return false;
            }
            return true;
        }
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (WasFPSCameraNotFound)
            {
                var notification = NotificationBase.ShowNotification<ListNotification>();
                notification.AddParas(Translations.Translate("FPSCAMERA_NOT_DETECTED"));
            }
            base.OnLevelLoaded(mode);
        }

        /// <summary>
        /// Performs any actions upon successful level loading completion.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.).</param>
        protected override void LoadedActions(LoadMode mode)
        {
            base.LoadedActions(mode);
            gameObject = new GameObject("TrainDisplay");
            gameObject.AddComponent<DisplayUIManager>();
        }

        /// <summary>
        /// Called by the game when exiting a level.
        /// </summary>
        public override void OnLevelUnloading()
        {
            Object.Destroy(gameObject);
            WasFPSCameraNotFound = false;
            base.OnLevelUnloading();
        }
        private GameObject gameObject = null;
        private bool WasFPSCameraNotFound = false;
    }

}