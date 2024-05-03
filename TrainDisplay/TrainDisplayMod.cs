using ICities;
using System.Collections.Generic;
using System.Linq;
using TrainDisplay.TranslationFramework;

namespace TrainDisplay
{

    public class TrainDisplayMod : IUserMod, ILoadingExtension
    {

        public static Translation translation = new Translation();

        public string Name
        {
            get { return "Train Display - Updated"; }
        }

        public string Description
        {
            get { return "Japanese Style Train Display"; }
        }

        private static List<string> _displayLanguageOptions = null;
        private static List<string> DisplayLanguageOptions
        {
            get
            {
                if (_displayLanguageOptions == null)
                {
                    _displayLanguageOptions = new List<string>
                    {
                        "A_TD_SETTINGS_SYSTEM_LANGUAGE"
                    };
                    var languages = translation.AvailableLanguagesReadable();
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

            helper.AddTextfield(
                translation.GetTranslation("A_TD_SETTINGS_DISPLAY_WIDTH"),
                config.DisplayWidth + "",
                width =>
                {
                    int displayWidth;
                    if (!int.TryParse(width, out displayWidth))
                    {
                        displayWidth = 512;
                    }
                    config.DisplayWidth = displayWidth;
                    Configuration<TrainDisplayConfiguration>.Save();
                });

            helper.AddDropdown(
                translation.GetTranslation("A_TD_SETTINGS_DISPLAY_LANGUAGE"),
                DisplayLanguageOptions.Select(s => translation.GetTranslation(s)).ToArray(),
                DisplayLanguageOptions.IndexOf(config.DisplayLanguage),
                index =>
                {
                    config.DisplayLanguage = DisplayLanguageOptions[index];
                    translation.SetDisplayLanguage();
                    Configuration<TrainDisplayConfiguration>.Save();
                }
            );

            helper.AddCheckbox(
                translation.GetTranslation("A_TD_SETTINGS_TEXT_SHRINKED"),
                config.IsTextShrinked,
                shrinked =>
                {
                    config.IsTextShrinked = shrinked;
                    Configuration<TrainDisplayConfiguration>.Save();
                }
            );

            helper.AddTextfield(
                translation.GetTranslation("A_TD_SETTINGS_STATION_SUFFIX"),
                config.StationSuffix + "",
                suffix =>
                {
                    config.StationSuffix = suffix;
                    Configuration<TrainDisplayConfiguration>.Save();
                }
            );
        }

        public void OnCreated(ILoading loading)
        {

        }
        public void OnLevelLoaded(LoadMode mode)
        {
            TrainDisplayMain.Initialize();
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