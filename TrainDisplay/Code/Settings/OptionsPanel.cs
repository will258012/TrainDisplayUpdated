﻿using AlgernonCommons;
using AlgernonCommons.Translation;
using AlgernonCommons.UI;
using ColossalFramework.UI;
using System;
using TrainDisplay.UI;
using TrainDisplay.Utils;
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
        private const float SliderMargin = 60f;
        protected override void Setup()
        {
            var currentY = Margin;
            var headerWidth = OptionsPanelManager<OptionsPanel>.PanelWidth - (Margin * 2f);

            //Add Scrollbar. 
            var scrollPanel = AddUIComponent<UIScrollablePanel>();
            scrollPanel.relativePosition = new Vector2(0, Margin);
            scrollPanel.autoSize = false;
            scrollPanel.autoLayout = false;
            scrollPanel.width = width - 15f;
            scrollPanel.height = height - 15f;
            scrollPanel.clipChildren = true;
            scrollPanel.builtinKeyNavigation = true;
            scrollPanel.scrollWheelDirection = UIOrientation.Vertical;
            scrollPanel.eventVisibilityChanged += (_, isShow) => { if (isShow) scrollPanel.Reset(); };
            UIScrollbars.AddScrollbar(this, scrollPanel);

            var languageDropDown = UIDropDowns.AddPlainDropDown(scrollPanel, LeftMargin, currentY, Translations.Translate("LANGUAGE_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
            };
            languageDropDown.parent.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += languageDropDown.parent.height + Margin;

            var loggingCheck = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("DETAIL_LOGGING"));
            loggingCheck.isChecked = Logging.DetailLogging;
            loggingCheck.eventCheckChanged += (_, isChecked) => Logging.DetailLogging = isChecked;
            currentY += loggingCheck.height + Margin;
            #region Display
            UISpacers.AddTitleSpacer(scrollPanel, LeftMargin, currentY, headerWidth, Translations.Translate("SETTINGS_GROUPNAME_DISPLAY"));
            currentY += TitleMargin;

            var displayWidth = UISliders.AddPlainSliderWithIntegerValue(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_DISPLAY_WIDTH"), 128f, 1024f, 1f, DisplayUI.Width);
            displayWidth.eventValueChanged += (_, value) => DisplayUI.Width = (int)value;
            currentY += displayWidth.height + SliderMargin;

            string[] displayLineDirItems = {
            Translations.Translate("SETTINGS_DISPLAY_ROWDIR_L2R"),
            Translations.Translate("SETTINGS_DISPLAY_ROWDIR_R2L")
            };
            var displayRowDir = UIDropDowns.AddPlainDropDown(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_DISPLAY_ROWDIR"), displayLineDirItems, (int)TrainDisplaySettings.DisplayRowDirection);
            displayRowDir.eventSelectedIndexChanged += (_, index) => TrainDisplaySettings.DisplayRowDirection = (TrainDisplaySettings.DisplayRowDirections)index;
            displayRowDir.parent.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += displayRowDir.parent.height + LeftMargin;

            var stationNameAngle = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_STATION_NAME_ANGLE"), -90f, 90f, 1f, TrainDisplaySettings.StationNameAngle, new UISliders.SliderValueFormat(valueMultiplier: 1, roundToNearest: 1f, numberFormat: "N0", suffix: "°"));
            stationNameAngle.eventValueChanged += (_, value) => TrainDisplaySettings.StationNameAngle = value;
            currentY += stationNameAngle.height + SliderMargin;

            var maxItem = UISliders.AddPlainSliderWithIntegerValue(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_MAXSTATIONNUM"), 2, 10, 1, DisplayUI.MaxStationNum);
            maxItem.eventValueChanged += (_, value) => DisplayUI.MaxStationNum = (int)value;
            currentY += maxItem.height + SliderMargin;

            var textShrinked = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_TEXT_SHRINKED"));
            textShrinked.isChecked = TrainDisplaySettings.IsTextShrinked;
            textShrinked.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsTextShrinked = isChecked;
            currentY += textShrinked.height + Margin;

            var stationSuffix = UITextFields.AddPlainTextfield(scrollPanel, Translations.Translate("SETTINGS_STATION_SUFFIX"));
            stationSuffix.parent.tooltip = Translations.Translate("SETTINGS_STATION_SUFFIX_TOOLTIP");
            stationSuffix.parent.relativePosition = new Vector2(LeftMargin, currentY);
            stationSuffix.size = new Vector2(650f, 30f);
            stationSuffix.textScale = 1.2f;
            stationSuffix.text = TrainDisplaySettings.StationSuffix;
            stationSuffix.eventTextChanged += (_, text)
                => TrainDisplaySettings.StationSuffix = text.Replace("“", "\"")
                                                    .Replace("”", "\"")
                                                    .Replace("，", ",");//Replace Chinese punctuation with English 
            currentY += stationSuffix.parent.height + Margin;

            var stationSuffixWhiteList = UITextFields.AddPlainTextfield(scrollPanel, Translations.Translate("SETTINGS_STATION_SUFFIX_WHITELIST"));
            stationSuffixWhiteList.parent.tooltip = Translations.Translate("SETTINGS_STATION_SUFFIX_WHITELISTTOOLTIP");
            stationSuffixWhiteList.parent.relativePosition = new Vector2(LeftMargin, currentY);
            stationSuffixWhiteList.size = new Vector2(650f, 30f);
            stationSuffixWhiteList.textScale = 1.2f;
            stationSuffixWhiteList.text = TrainDisplaySettings.StationSuffixWhiteList;
            stationSuffixWhiteList.eventTextChanged += (_, text)
                => TrainDisplaySettings.StationSuffixWhiteList = text.Replace("“", "\"")
                                                                    .Replace("”", "\"")
                                                                    .Replace("，", ",");
            currentY += stationSuffixWhiteList.parent.height + Margin;
            #endregion
            #region Vehicle Types
            UISpacers.AddTitleSpacer(scrollPanel, LeftMargin, currentY, headerWidth, Translations.Translate("SETTINGS_DISPLAY_VEHICLE_TYPE"));
            currentY += TitleMargin;

            var train = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_TRAIN"));
            train.isChecked = TrainDisplaySettings.IsTrain;
            train.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsTrain = isChecked;
            currentY += train.height + Margin;

            var metro = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_METRO"));
            metro.isChecked = TrainDisplaySettings.IsMetro;
            metro.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsMetro = isChecked;
            currentY += metro.height + Margin;

            var monorail = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_MONORAIL"));
            monorail.isChecked = TrainDisplaySettings.IsMonorail;
            monorail.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsMonorail = isChecked;
            currentY += monorail.height + Margin;

            var tram = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_TRAM"));
            tram.isChecked = TrainDisplaySettings.IsTram;
            tram.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsTram = isChecked;
            currentY += tram.height + Margin;

            var bus = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_BUS"));
            bus.isChecked = TrainDisplaySettings.IsBus;
            bus.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsBus = isChecked;
            currentY += bus.height + Margin;

            var trolleybus = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_TROLLEYBUS"));
            trolleybus.isChecked = TrainDisplaySettings.IsTrolleybus;
            trolleybus.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsTrolleybus = isChecked;
            currentY += trolleybus.height + Margin;

            var ferry = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_FERRY"));
            ferry.isChecked = TrainDisplaySettings.IsFerry;
            ferry.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsFerry = isChecked;
            currentY += ferry.height + Margin;

            var blimp = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_BLIMP"));
            blimp.isChecked = TrainDisplaySettings.IsBlimp;
            blimp.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsBlimp = isChecked;
            currentY += blimp.height + Margin;

            var copter = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_IS_COPTER"));
            copter.isChecked = TrainDisplaySettings.IsCopter;
            copter.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.IsCopter = isChecked;
            currentY += copter.height + Margin;
            #endregion
            #region TTS
            UISpacers.AddTitleSpacer(scrollPanel, LeftMargin, currentY, headerWidth, "TTS");
            currentY += TitleMargin;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var enableTTS = UICheckBoxes.AddPlainCheckBox(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_TTS"));
                enableTTS.tooltip = Translations.Translate("SETTINGS_TTS_TOOLTIP");
                enableTTS.isChecked = TrainDisplaySettings.TTS;
                enableTTS.eventCheckChanged += (_, isChecked) => TrainDisplaySettings.TTS = isChecked;
                currentY += enableTTS.height + Margin;

                var voicesDropDown = UIDropDowns.AddPlainDropDown(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_TTS_VOICECHOICE"), TTSHelper.Instance.VoiceNames, TTSHelper.Instance.VoiceIndex,500f);
                voicesDropDown.eventSelectedIndexChanged += (control, index) => TTSHelper.Instance.VoiceIndex = index;
                voicesDropDown.parent.relativePosition = new Vector2(LeftMargin, currentY);
                currentY += voicesDropDown.parent.height + Margin;

                var TTSDeparting = UITextFields.AddPlainTextfield(scrollPanel, Translations.Translate("SETTINGS_TTS_DEPARTING"));
                TTSDeparting.parent.relativePosition = new Vector2(LeftMargin, currentY);
                TTSDeparting.tooltip = Translations.Translate("SETTINGS_TTS_FORMATTOOLTIP");
                TTSDeparting.size = new Vector2(650f, 30f);
                TTSDeparting.textScale = 1.2f;
                TTSDeparting.text = TrainDisplaySettings.TTSDeparting;
                TTSDeparting.eventTextChanged += (_, text) => TrainDisplaySettings.TTSDeparting = text;
                currentY += TTSDeparting.parent.height + Margin;

                var TTSArriving = UITextFields.AddPlainTextfield(scrollPanel, Translations.Translate("SETTINGS_TTS_ARRIVING"));
                TTSArriving.parent.relativePosition = new Vector2(LeftMargin, currentY);
                TTSArriving.tooltip = Translations.Translate("SETTINGS_TTS_FORMATTOOLTIP");
                TTSArriving.size = new Vector2(650f, 30f);
                TTSArriving.textScale = 1.2f;
                TTSArriving.text = TrainDisplaySettings.TTSArriving;
                TTSArriving.eventTextChanged += (_, text) => TrainDisplaySettings.TTSArriving = text;
                currentY += TTSArriving.parent.height + Margin;
            }
            else
            {
                var TTSNotAvailable = UISpacers.AddTitle(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_TTS_NOTAVAILABLE"));
                TrainDisplaySettings.TTS = false;
                currentY += TitleMargin;
            }
            #endregion

            var reset = UIButtons.AddButton(scrollPanel, LeftMargin, currentY, Translations.Translate("SETTINGS_RESETBTN"), 200f, 40f);
            reset.eventClicked += (c, _) => ResetSettings();
        }
        private void ResetSettings()
        {
            TrainDisplaySettings.ResetToDefaults();
            TrainDisplaySettings.Save();
            OptionsPanelManager<OptionsPanel>.LocaleChanged();
        }
    }
}
