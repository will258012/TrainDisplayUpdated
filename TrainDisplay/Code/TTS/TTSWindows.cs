using AlgernonCommons;
using Microsoft.Win32;
using SpeechLib;
using System;
using TrainDisplay.Settings;

namespace TrainDisplay.TTS;
internal sealed class TTSWindows : ITTSProvider
{
    private const int Async = 1;
    private const int PurgeBeforeSpeak = 2;

    private readonly SpVoice voice = new SpVoice();
    private readonly SpObjectToken[] voices;
    private int voiceIndex;

    internal TTSWindows()
    {
        voices = GetVoices();
        if (voices.Length > 0)
        {
            VoiceIndex = 0;
        }
    }

    public bool IsAvailable => voices.Length > 0;

    public int VoiceIndex
    {
        get => voiceIndex;
        set
        {
            voiceIndex = voices.Length == 0 ? 0 : Math.Max(0, Math.Min(value, voices.Length - 1));
            if (voices.Length > 0)
            {
                voice.SetVoice(voices[voiceIndex]);
            }
        }
    }

    public string VoiceName
    {
        get => voices.Length == 0 ? string.Empty : GetFriendlyName(voices[voiceIndex]);
        set => SetVoiceFromName(value);
    }

    public string[] VoiceNames
    {
        get
        {
            var result = new string[voices.Length];
            for (int i = 0; i < voices.Length; i++)
            {
                result[i] = GetFriendlyName(voices[i]);
            }
            return result;
        }
    }

    public void Speak(string text)
    {
        voice.SetRate(TrainDisplaySettings.TTSRate);
        voice.SetVolume(100);
        voice.SetPriority(SPVPRIORITY.SPVPRI_NORMAL);
        voice.Speak(text, Async | PurgeBeforeSpeak, out _);
    }

    public void Stop() => voice.Speak(string.Empty, Async | PurgeBeforeSpeak, out _);

    private SpObjectToken[] GetVoices()
    {
        voice.GetVoice(out var currentVoice);
        currentVoice.GetCategory(out var tokenCategory);
        tokenCategory.EnumTokens(null, null, out var tokens);
        tokens.GetCount(out var tokenCount);
        var result = new SpObjectToken[tokenCount];
        for (uint i = 0; i < tokenCount; i++)
        {
            tokens.Item(i, out result[i]);
        }
        return result;
    }

    private static string GetFriendlyName(SpObjectToken token)
    {
        token.GetId(out string tokenId);
        string registryPath = tokenId.Replace(@"HKEY_LOCAL_MACHINE\", string.Empty);
        using (var key = Registry.LocalMachine.OpenSubKey(registryPath))
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
        for (int i = 0; i < voices.Length; i++)
        {
            if (GetFriendlyName(voices[i]) == voiceName)
            {
                VoiceIndex = i;
                return;
            }
        }

        if (!string.IsNullOrEmpty(voiceName))
        {
            Logging.Error($"Voice '{voiceName}' not found; using the first installed voice");
        }
        VoiceIndex = 0;
    }
}
