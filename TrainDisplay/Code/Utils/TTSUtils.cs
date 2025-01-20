using ColossalFramework;
using SpeechLib;
using TrainDisplay.Settings;

namespace TrainDisplay.Utils
{
    public class TTSUtils
    {
        public static SpVoice SpVoice { get; } = new SpVoice();
        public static void Speak(string text)
        {
            if (!TrainDisplaySettings.TTS) return;
            SpVoice.SetRate(0);
            SpVoice.SetVolume(100);
            SpVoice.SetPriority(SPVPRIORITY.SPVPRI_NORMAL);
            SpVoice.Speak(text, 1/*async*/, out _);
        }
        public static void Stop() => Speak(string.Empty);
    }
}
