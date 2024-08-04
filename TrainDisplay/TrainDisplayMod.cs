using ICities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrainDisplay.Config;
using TrainDisplay.TranslationFramework;

namespace TrainDisplay
{

    public class TrainDisplayMod : IUserMod, ILoadingExtension
    {

        public static Translation translation = new Translation();

        public string Name => $"Train Display - Updated v{Version}";
        public string Description => "Japanese Style Train Display";
        private string Version
        {
            get
            {
                var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
            }
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
            var config = TrainDisplayConfig.Instance;

            helper.AddTextfield(
                translation.GetTranslation("A_TD_SETTINGS_DISPLAY_WIDTH"),
                config.DisplayWidth + "",
                width =>
                {
                    if (!int.TryParse(width, out int displayWidth))
                    {
                        displayWidth = 512;
                    }
                    config.DisplayWidth = displayWidth;
                    TrainDisplayConfig.Save();
                });

            helper.AddDropdown(
                translation.GetTranslation("A_TD_SETTINGS_DISPLAY_LANGUAGE"),
                DisplayLanguageOptions.Select(s => translation.GetTranslation(s)).ToArray(),
                DisplayLanguageOptions.IndexOf(config.DisplayLanguage),
                index =>
                {
                    config.DisplayLanguage = DisplayLanguageOptions[index];
                    translation.SetDisplayLanguage();
                    TrainDisplayConfig.Save();
                }
            );

            helper.AddCheckbox(
                translation.GetTranslation("A_TD_SETTINGS_TEXT_SHRINKED"),
                config.IsTextShrinked,
                shrinked =>
                {
                    config.IsTextShrinked = shrinked;
                    TrainDisplayConfig.Save();
                }
            );

            helper.AddTextfield(
                translation.GetTranslation("A_TD_SETTINGS_STATION_SUFFIX"),
                config.StationSuffix + "",
                suffix =>
                {
                    config.StationSuffix = suffix;
                    TrainDisplayConfig.Save();
                }
            );
            helper.AddSpace(10);
            UIHelperBase group = helper.AddGroup(translation.GetTranslation("A_TD_SETTINGS_DISPLAY_VEHICLE_TYPE"));
            group.AddCheckbox(translation.GetTranslation("A_TD_SETTINGS_IS_TRAIN"), config.IsTrain, check =>
            {
                config.IsTrain = check;
                TrainDisplayConfig.Save();
            });
            group.AddCheckbox(translation.GetTranslation("A_TD_SETTINGS_IS_METRO"), config.IsMetro, check =>
            {
                config.IsMetro = check;
                TrainDisplayConfig.Save();
            });
            group.AddCheckbox(translation.GetTranslation("A_TD_SETTINGS_IS_MONORAIL"), config.IsMonorail, check =>
            {
                config.IsMonorail = check;
                TrainDisplayConfig.Save();
            });
            group.AddCheckbox(translation.GetTranslation("A_TD_SETTINGS_IS_TRAM"), config.IsTram, check =>
            {
                config.IsTram = check;
                TrainDisplayConfig.Save();
            });
            group.AddCheckbox(translation.GetTranslation("A_TD_SETTINGS_IS_BUS"), config.IsBus, check =>
            {
                config.IsBus = check;
                TrainDisplayConfig.Save();
            });
            group.AddCheckbox(translation.GetTranslation("A_TD_SETTINGS_IS_TROLLEYBUS"), config.IsTrolleybus, check =>
            {
                config.IsTrolleybus = check;
                TrainDisplayConfig.Save();
            });
            group.AddCheckbox(translation.GetTranslation("A_TD_SETTINGS_IS_FERRY"), config.IsFerry, check =>
            {
                config.IsFerry = check;
                TrainDisplayConfig.Save();
            });
            group.AddCheckbox(translation.GetTranslation("A_TD_SETTINGS_IS_BLIMP"), config.IsBlimp, check =>
            {
                config.IsBlimp = check;
                TrainDisplayConfig.Save();
            });
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