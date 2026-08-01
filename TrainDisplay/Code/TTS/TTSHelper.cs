using AlgernonCommons;
using TrainDisplay.Settings;
using UnityEngine;

namespace TrainDisplay.TTS
{
    internal interface ITTSProvider
    {
        bool IsAvailable { get; }
        int VoiceIndex { get; set; }
        string VoiceName { get; set; }
        string[] VoiceNames { get; }
        void Speak(string text);
        void Stop();
    }

    public class TTSHelper
    {
        public static TTSHelper Instance { get; } = new();

        private readonly ITTSProvider provider;

        private TTSHelper()
        {
            try
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                        provider = new TTSWindows();
                        break;

                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                        provider = new TTSMacOS();
                        break;
                }
            }
            catch (System.Exception e)
            {
                TrainDisplaySettings.TTS = false;
                Logging.LogException(e, "Failed to initialize TTS");
            }
        }

        internal bool IsAvailable => provider?.IsAvailable == true;

        public int VoiceIndex
        {
            get => provider?.VoiceIndex ?? default;
            set
            {
                if (provider != null)
                {
                    provider.VoiceIndex = value;
                }
            }
        }

        public string VoiceName
        {
            get => provider?.VoiceName ?? string.Empty;
            set
            {
                if (provider != null)
                {
                    provider.VoiceName = value;
                }
            }
        }

        public string[] VoiceNames => provider?.VoiceNames ?? [];

        public void Speak(string text)
        {
            if (!TrainDisplaySettings.TTS)
            {
                return;
            }

            if (!IsAvailable)
            {
                TrainDisplaySettings.TTS = false;
                Logging.Error("TTS is not available on this platform");
                return;
            }

            try
            {
                provider.Speak(text);
            }
            catch (System.Exception e)
            {
                if (provider is TTSMacOS)
                {
                    TrainDisplaySettings.TTS = false;
                }
                Logging.LogException(e);
            }
        }

        public void Stop()
        {
            if (provider == null)
            {
                return;
            }

            try
            {
                provider.Stop();
            }
            catch (System.Exception e)
            {
                Logging.LogException(e);
            }
        }
    }
}
