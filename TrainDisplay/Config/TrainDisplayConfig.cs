namespace TrainDisplay.Config
{
    [ConfigPath("TrainDisplay.xml")]
    public sealed class TrainDisplayConfig : ConfigBase<TrainDisplayConfig>
    {
        public int DisplayWidth { get; set; } = 512;

        public string DisplayLanguage { get; set; } = "A_TD_SETTINGS_SYSTEM_LANGUAGE";

        public bool IsTextShrinked { get; set; } = false;
        public string StationSuffix { get; set; } = "\"駅\",\"站\",\" Station\",\" Sta.\",\" Sta\"";
        public bool IsTrain { get; set; } = true;
        public bool IsMetro { get; set; } = true;
        public bool IsMonorail { get; set; } = true;
        public bool IsTram { get; set; } = true;
        public bool IsBus { get; set; } = true;
        public bool IsTrolleybus { get; set; } = true;
        public bool IsFerry { get; set; } = true;
        public bool IsBlimp { get; set; } = true;
    }
}
