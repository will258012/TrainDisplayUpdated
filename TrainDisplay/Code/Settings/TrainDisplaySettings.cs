using AlgernonCommons;
using AlgernonCommons.Notifications;
using AlgernonCommons.Translation;
using AlgernonCommons.XML;
using ColossalFramework.IO;
using System.IO;
using System.Xml.Serialization;
using TrainDisplay.UI;
using TrainDisplay.Utils;
using UnityEngine;

namespace TrainDisplay.Settings
{
    [XmlRoot("TrainDisplay")]
    public sealed class TrainDisplaySettings : SettingsXMLBase
    {
        // Settings file name
        [XmlIgnore]
        private static readonly string SettingsFileName = Path.Combine(DataLocation.localApplicationData, "TrainDisplay.xml");

        internal static void Load() => XMLFileUtils.Load<TrainDisplaySettings>(SettingsFileName);

        internal static void Save() => XMLFileUtils.Save<TrainDisplaySettings>(SettingsFileName);

        internal static void ResetToDefaults()
        {
            Translations.CurrentLanguage = "default";
            Logging.DetailLogging = false;
            WhatsNew.LastNotifiedVersionString = "0.0";

            DisplayUI.Width = 512f;
            DisplayRowDirection = DisplayRowDirections.L2R;
            StationNameAngle = -90f;
            DisplayUI.Position = new Vector2(0f, Screen.height - DisplayUI.Height);
            DisplayUI.MaxStationNum = 6;
            StationSuffix = @"""駅"",""站"","" Station"","" Sta."","" Sta""";
            StationSuffixWhiteList =
                @"""Train Station"",""Railway Station"",""Bus Station"",""Subway Station"",""Metro Station"",""Transit Station"",""地铁站"",""火车站"",""汽车站"",""轻轨站"",""高铁站"",""总站""";
            IsTextShrinked = IsTrain = IsMetro = IsMonorail = IsTram = IsBus = IsTrolleybus = IsFerry = IsBlimp = IsCopter = true;

            TTS = false;
            TTSHelper.Instance.VoiceIndex = default;
            TTSDeparting = "This train is bound for: {1}. Next station: {0}";
            TTSArriving = "We are now arriving at: {0}";
        }
        #region Display
        [XmlElement("DisplayPos")]
        public Vector2 XMLDisplayPos { get => DisplayUI.Position; set => DisplayUI.Position = value; }

        [XmlElement("DisplayWidth")]
        public float XMLDisplayWidth { get => DisplayUI.Width; set => DisplayUI.Width = value; }
        public enum DisplayRowDirections
        {
            L2R,
            R2L
        }
        [XmlElement("DisplayRowDirection")]
        public DisplayRowDirections XMLDisplayRowDirection { get => DisplayRowDirection; set => DisplayRowDirection = value; }
        [XmlIgnore]
        internal static DisplayRowDirections DisplayRowDirection = DisplayRowDirections.L2R;

        [XmlElement("StationNameAngle")]
        public float XMLStationNameAngle { get => StationNameAngle; set => StationNameAngle = value; }
        [XmlIgnore]
        internal static float StationNameAngle = -90f;

        [XmlElement("MaxStationNum")]
        public int XMLMaxStationNum { get => DisplayUI.MaxStationNum; set => DisplayUI.MaxStationNum = value; }

        [XmlElement("IsTextShrinked")]
        public bool XMLIsTextShrinked { get => IsTextShrinked; set => IsTextShrinked = value; }
        [XmlIgnore]
        internal static bool IsTextShrinked = true;

        [XmlElement("StationSuffix")]
        public string XMLStationSuffix { get => StationSuffix; set => StationSuffix = value; }
        [XmlIgnore]
        internal static string StationSuffix = @"""駅"",""站"","" Station"","" Sta."","" Sta""";
        [XmlElement("StationSuffixWhiteList")]
        public string XMLStationSuffixWhiteList { get => StationSuffixWhiteList; set => StationSuffixWhiteList = value; }

        [XmlIgnore]
        internal static string StationSuffixWhiteList =
            @"""Train Station"",""Railway Station"",""Bus Station"",""Subway Station"",""Metro Station"",""Transit Station"",""地铁站"",""火车站"",""汽车站"",""轻轨站"",""高铁站"",""总站""";
        #endregion
        #region Vehicle Types
        [XmlElement("IsTrain")]
        public bool XMLIsTrain { get => IsTrain; set => IsTrain = value; }
        [XmlIgnore]
        internal static bool IsTrain = true;

        [XmlElement("IsMetro")]
        public bool XMLIsMetro { get => IsMetro; set => IsMetro = value; }
        [XmlIgnore]
        internal static bool IsMetro = true;

        [XmlElement("IsMonorail")]
        public bool XMLIsMonorail { get => IsMonorail; set => IsMonorail = value; }
        [XmlIgnore]
        internal static bool IsMonorail = true;

        [XmlElement("IsTram")]
        public bool XMLIsTram { get => IsTram; set => IsTram = value; }
        [XmlIgnore]
        internal static bool IsTram = true;

        [XmlElement("IsBus")]
        public bool XMLIsBus { get => IsBus; set => IsBus = value; }
        [XmlIgnore]
        internal static bool IsBus = true;

        [XmlElement("IsTrolleybus")]
        public bool XMLIsTrolleybus { get => IsTrolleybus; set => IsTrolleybus = value; }
        [XmlIgnore]
        internal static bool IsTrolleybus = true;

        [XmlElement("IsFerry")]
        public bool XMLIsFerry { get => IsFerry; set => IsFerry = value; }
        [XmlIgnore]
        internal static bool IsFerry = true;

        [XmlElement("IsBlimp")]
        public bool XMLIsBlimp { get => IsBlimp; set => IsBlimp = value; }
        [XmlIgnore]
        internal static bool IsBlimp = true;

        [XmlElement("IsCopter")]
        public bool XMLIsCopter { get => IsCopter; set => IsCopter = value; }
        [XmlIgnore]
        internal static bool IsCopter = true;
        #endregion
        #region TTS
        [XmlElement("TTS")]
        public bool XMLTTS { get => TTS; set => TTS = value; }
        [XmlIgnore]
        internal static bool TTS = false;
        [XmlElement("TTSVoiceName")]
        public string XMLTTSVoiceName { get => TTSHelper.Instance.VoiceName; set => TTSHelper.Instance.VoiceName = value; }
        [XmlElement("TTSDeparting")]
        public string XMLTTSDeparting { get => TTSDeparting; set => TTSDeparting = value; }
        [XmlIgnore]
        internal static string TTSDeparting = "This train is bound for: {1}. Next station: {0}";
        [XmlElement("TTSArriving")]
        public string XMLTTSArriving { get => TTSArriving; set => TTSArriving = value; }
        [XmlIgnore]
        internal static string TTSArriving = "We are now arriving at: {0}";
        #endregion
    }
}
