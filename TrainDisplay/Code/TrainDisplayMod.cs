using AlgernonCommons;
using AlgernonCommons.Translation;
using Epic.OnlineServices.Lobby;
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
    }
    public sealed class TrainDisplayLoading : LoadingBase<OptionsPanel>
    {

        /// <summary>
        /// Performs any actions upon successful level loading completion.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.).</param>
        protected override void LoadedActions(LoadMode mode)
        {
            base.LoadedActions(mode);
            gameObject.AddComponent<TrainDisplayMain>();
        }


        /// <summary>
        /// Called by the game when exiting a level.
        /// </summary>
        public override void OnLevelUnloading()
        {
            Object.Destroy(gameObject);
            base.OnLevelUnloading();
        }
        private GameObject gameObject = new GameObject();
    }

}