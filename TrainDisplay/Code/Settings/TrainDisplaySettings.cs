using AlgernonCommons.XML;
using ColossalFramework.IO;
using System.IO;
using System.Xml.Serialization;

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


        [XmlElement("DisplayWidth")]
        public int XMLDisplayWidth { get => DisplayWidth; set => DisplayWidth = value; }
        [XmlIgnore]
        internal static int DisplayWidth = 512;

        [XmlElement("StationNameAngle")]
        public float XMLStationNameAngle { get => StationNameAngle; set => StationNameAngle = value; }
        [XmlIgnore]
        internal static float StationNameAngle = -90f;

        [XmlElement("IsTextShrinked")]
        public bool XMLIsTextShrinked { get => IsTextShrinked; set => IsTextShrinked = value; }
        [XmlIgnore]
        internal static bool IsTextShrinked = false;

        [XmlElement("StationSuffix")]
        public string XMLStationSuffix { get => StationSuffix; set => StationSuffix = value; }
        [XmlIgnore]
        internal static string StationSuffix = "\"駅\",\"站\",\" Station\",\" Sta.\",\" Sta\"";

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
