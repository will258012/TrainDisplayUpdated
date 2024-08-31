using AlgernonCommons;
using AlgernonCommons.Translation;
using AlgernonCommons.UI;
using UnityEngine;

namespace TrainDisplay.Settings
{
    public sealed class OptionsPanel : OptionsPanelBase
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;
        private const float TitleMargin = 50f;
        protected override void Setup()
        {
            var currentY = Margin;
            var headerWidth = OptionsPanelManager<OptionsPanel>.PanelWidth - (Margin * 2f);

            var languageDropDown = UIDropDowns.AddPlainDropDown(this, LeftMargin, currentY, Translations.Translate("LANGUAGE_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
            };
            languageDropDown.parent.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += languageDropDown.parent.height + Margin;

            var loggingCheck = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("DETAIL_LOGGING"));
            loggingCheck.isChecked = Logging.DetailLogging;
            loggingCheck.eventCheckChanged += (c, isChecked) => { Logging.DetailLogging = isChecked; };
            currentY += loggingCheck.height + Margin;

            var displayWidth = UITextFields.AddPlainTextfield(this, Translations.Translate("SETTINGS_DISPLAY_WIDTH"));
            displayWidth.parent.relativePosition = new Vector2(LeftMargin, currentY);
            displayWidth.text = TrainDisplaySettings.DisplayWidth.ToString() + " ";
            displayWidth.eventTextChanged += (_, text) =>
            {
                if (!int.TryParse(text, out var result))
                {
                    result = 512;
                }
                TrainDisplaySettings.DisplayWidth = result;
            };
            currentY += displayWidth.parent.height + Margin;

            var textShrinked = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("SETTINGS_TEXT_SHRINKED"));
            textShrinked.isChecked = TrainDisplaySettings.IsTextShrinked;
            textShrinked.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsTextShrinked = isChecked;
            currentY += textShrinked.height + Margin;

            var stationSuffix = UITextFields.AddPlainTextfield(this, Translations.Translate("SETTINGS_STATION_SUFFIX"));
            stationSuffix.parent.relativePosition = new Vector2(LeftMargin, currentY);
            stationSuffix.text = TrainDisplaySettings.StationSuffix + " ";
            stationSuffix.eventTextChanged += (_, text) => TrainDisplaySettings.StationSuffix = text;
            currentY += stationSuffix.parent.height + Margin;


            UISpacers.AddTitleSpacer(this, LeftMargin, currentY, headerWidth, Translations.Translate("SETTINGS_DISPLAY_VEHICLE_TYPE"));
            currentY += TitleMargin;


            var train = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_TRAIN"));
            train.isChecked = TrainDisplaySettings.IsTrain;
            train.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsTrain = isChecked;
            currentY += train.height + Margin;

            var metro = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_METRO"));
            metro.isChecked = TrainDisplaySettings.IsMetro;
            metro.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsMetro = isChecked;
            currentY += metro.height + Margin;

            var monorail = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_MONORAIL"));
            monorail.isChecked = TrainDisplaySettings.IsMonorail;
            monorail.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsMonorail = isChecked;
            currentY += monorail.height + Margin;

            var tram = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_TRAM"));
            tram.isChecked = TrainDisplaySettings.IsTram;
            tram.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsTram = isChecked;
            currentY += tram.height + Margin;

            var bus = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_BUS"));
            bus.isChecked = TrainDisplaySettings.IsBus;
            bus.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsBus = isChecked;
            currentY += bus.height + Margin;

            var trolleybus = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_TROLLEYBUS"));
            trolleybus.isChecked = TrainDisplaySettings.IsTrolleybus;
            trolleybus.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsTrolleybus = isChecked;
            currentY += trolleybus.height + Margin;

            var ferry = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_FERRY"));
            ferry.isChecked = TrainDisplaySettings.IsFerry;
            ferry.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsFerry = isChecked;
            currentY += ferry.height + Margin;

            var blimp = UICheckBoxes.AddPlainCheckBox(this, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_BLIMP"));
            blimp.isChecked = TrainDisplaySettings.IsBlimp;
            blimp.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsBlimp = isChecked;
            currentY += blimp.height + Margin;

        }
    }
}
