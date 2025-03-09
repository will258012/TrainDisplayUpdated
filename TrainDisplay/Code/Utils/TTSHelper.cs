using AlgernonCommons;
using Microsoft.Win32;
using SpeechLib;
using System;
using TrainDisplay.Settings;
namespace TrainDisplay.Utils
{
    public class TTSHelper
    {
        public static TTSHelper Instance = new TTSHelper();
        public int VoiceIndex { get; set; } = default;
        public string VoiceName
        {
            get => _voiceName;
            set => SetVoiceFromName(value);
        }
        private string _voiceName => GetFriendlyName(CurrentVoice);
        private SpObjectToken CurrentVoice => Voices[VoiceIndex];
        public void Speak(string text)
        {
            if (!TrainDisplaySettings.TTS) return;
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                TrainDisplaySettings.TTS = false;
                Logging.Error("TTS is only available on Windows");
                return;
            }
            try
            {
                InnerSpeak(text);
            }
            catch (Exception e)
            {
                Logging.LogException(e);
            }
        }
        private void InnerSpeak(string text)
        {
            foreach (var i in Voices)
            {
                i.GetId(out var id);
                Logging.Message(id);
            }

            var voice = new SpVoice();
            voice.SetRate(0);
            voice.SetVolume(100);
            voice.SetPriority(SPVPRIORITY.SPVPRI_NORMAL);
            if (CurrentVoice != null)
                voice.SetVoice(CurrentVoice);
            voice.Speak(text, 1/*async*/, out _);
        }
        private SpObjectToken[] _voices;
        private SpObjectToken[] Voices
        {
            get
            {
                if (_voices == null)
                {
                    new SpVoice().GetVoice(out var ppToken);
                    ppToken.GetCategory(out var tokenCategory);
                    tokenCategory.EnumTokens(null, null, out var tokens);
                    tokens.GetCount(out var tokenCount);
                    _voices = new SpObjectToken[tokenCount];
                    for (uint i = 0; i < tokenCount; i++)
                    {
                        tokens.Item(i, out _voices[i]);
                    }
                }
                return _voices;
            }
        }

        /// <summary>
        /// Gets a list of friendly names of <see cref="Voices"/>.
        /// </summary>
        public string[] VoiceNames
        {
            get
            {
                var result = new string[Voices.Length];
                for (int i = 0; i < Voices.Length; i++)
                {
                    result[i] = GetFriendlyName(Voices[i]);
                }

                return result;
            }
        }
        /// <summary>
        /// Gets the friendly name of a <see cref="SpObjectToken"/>.
        /// </summary>
        /// <param name="token">A instance of <see cref="SpObjectToken"/>.</param>
        /// <returns>A friendly name like "Microsoft Huihui - Chinese (Simplified, PRC)".</returns>
        private static string GetFriendlyName(SpObjectToken token)
        {
            token.GetId(out string tokenId);
            string registryPath = tokenId.Replace(@"HKEY_LOCAL_MACHINE\", string.Empty);
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    string friendlyName = key.GetValue("") as string;
                    if (!string.IsNullOrEmpty(friendlyName))
                    {
                        return friendlyName;
                    }
                }
            }
            Logging.Error($"Failed to get friendly name for id {tokenId}, returns id instead");
            return tokenId;
        }
        private void SetVoiceFromName(string voiceName)
        {
            for (int i = 0; i < Voices.Length; i++)
            {
                if (GetFriendlyName(Voices[i]) == voiceName)
                {
                    VoiceIndex = i;
                    return;
                }
            }

            Logging.Error($"Voice '{voiceName}' not found");
            VoiceIndex = default;
        }


        public void Stop() => Speak(string.Empty);
    }
}
