using ICities;
using UnityEngine;
using ColossalFramework.UI;
using TrainDisplay.TranslationFramework;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private static List<string> _displayLanguageOptions = null;
        private static List<string> DisplayLanguageOptions { 
            get
            {
                if (_displayLanguageOptions == null)
                {
                    _displayLanguageOptions = new List<string>
                    {
                        "A_TD_SETTINGS_SYSTEM_LANGUAGE"
                    };
                    var languages = TrainDisplayMod.translation.AvailableLanguagesReadable();
                    foreach (var language in languages)
                    {
                        _displayLanguageOptions.Add(language);
                    }
                }
                return _displayLanguageOptions;
            }
        }
        public void OnSettingsUI(UIHelperBase helper)
        {
            TrainDisplayConfiguration config = Configuration<TrainDisplayConfiguration>.Load();

            helper.AddTextfield(TrainDisplayMod.translation.GetTranslation("A_TD_SETTINGS_DISPLAY_WIDTH"), config.DisplayWidth + "", width =>
            {
                int displayWidth;
                if (!Int32.TryParse(width, out displayWidth))
                {
                    displayWidth = 512;
                }
                config.DisplayWidth = displayWidth;
                Configuration<TrainDisplayConfiguration>.Save();
            });

            helper.AddDropdown(
                TrainDisplayMod.translation.GetTranslation("A_TD_SETTINGS_DISPLAY_LANGUAGE"),
                DisplayLanguageOptions.Select(s => TrainDisplayMod.translation.GetTranslation(s)).ToArray(),
                DisplayLanguageOptions.IndexOf(config.DisplayLanguage),
                index =>
                {
                    config.DisplayLanguage = DisplayLanguageOptions[index];
                    TrainDisplayMod.translation.SetDisplayLanguage();
                }
            );
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