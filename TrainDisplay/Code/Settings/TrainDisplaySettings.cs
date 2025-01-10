using AlgernonCommons.XML;
using ColossalFramework.IO;
using System.IO;
using System.Xml.Serialization;
using TrainDisplay.UI;
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
            DisplayWidth = 512;
            StationNameAngle = -90f;
            DisplayUI.Position = new Vector2(0f, Screen.height - DisplayUI.Height);
            DisplayUI.MaxStationNum = 6;
            StationSuffix = @"""駅"",""站"","" Station"","" Sta."","" Sta""";
            StationSuffixWhiteList = @"""Railway Station"",""地铁站"",""火车站"",""汽车站"",""总站""";
            IsTextShrinked = IsTrain = IsMetro = IsMonorail = IsTram = IsBus = IsTrolleybus = IsFerry = IsBlimp = IsCopter = true;
        }
        [XmlElement("DisplayWidth")]
        public int XMLDisplayWidth { get => DisplayWidth; set => DisplayWidth = value; }
        [XmlIgnore]
        internal static int DisplayWidth = 512;

        [XmlElement("StationNameAngle")]
        public float XMLStationNameAngle { get => StationNameAngle; set => StationNameAngle = value; }
        [XmlIgnore]
        internal static float StationNameAngle = -90f;

        [XmlElement("DisplayPos")]
        public Vector2 XMLDisplayPos { get => DisplayUI.Position; set => DisplayUI.Position = value; }

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
        internal static string StationSuffixWhiteList = @"""Railway Station"",""地铁站"",""火车站"",""汽车站"",""总站""";

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
        internal static bool IsTrolleybus { get; set; } = true;

        [XmlElement("IsFerry")]
        public bool XMLIsFerry { get => IsFerry; set => IsFerry = value; }
        [XmlIgnore]
        internal static bool IsFerry { get; set; } = true;

        [XmlElement("IsBlimp")]
        public bool XMLIsBlimp { get => IsBlimp; set => IsBlimp = value; }
        [XmlIgnore]
        internal static bool IsBlimp { get; set; } = true;

        [XmlElement("IsCopter")]
        public bool XMLIsCopter { get => IsCopter; set => IsCopter = value; }
        [XmlIgnore]
        internal static bool IsCopter { get; set; } = true;

    }
}
