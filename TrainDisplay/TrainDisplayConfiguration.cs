namespace TrainDisplay
{
    [ConfigurationPath("AsmapeTrainDisplay.xml")]
    public class TrainDisplayConfiguration
    {
        public int DisplayWidth { get; set; } = 512;

        public string DisplayLanguage { get; set; } = "A_TD_SETTINGS_SYSTEM_LANGUAGE";

        public bool IsTextShrinked { get; set; } = false;
    }
}
