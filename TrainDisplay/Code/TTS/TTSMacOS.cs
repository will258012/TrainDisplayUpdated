using AlgernonCommons;
using AlgernonCommons.Translation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using TrainDisplay.Settings;

namespace TrainDisplay.TTS;

internal sealed class TTSMacOS : ITTSProvider
{
    private const string SayPath = "/usr/bin/say";
    private const int VoiceEnumerationTimeout = 5000;

    private readonly object processLock = new object();
    private readonly string[] installedVoices;
    private SpeechProcessState currentSpeech;
    private int voiceIndex;

    internal TTSMacOS()
    {
        try
        {
            if (!File.Exists(SayPath))
            {
                throw new FileNotFoundException("The macOS text-to-speech command was not found", SayPath);
            }

            installedVoices = EnumerateVoices();
            if (installedVoices.Length == 0)
            {
                throw new InvalidOperationException("No macOS text-to-speech voices were found");
            }

            IsAvailable = true;
        }
        catch (Exception e)
        {
            installedVoices = [];
            TrainDisplaySettings.TTS = false;
            Logging.LogException(e, "Failed to initialize macOS TTS");
        }
    }

    public bool IsAvailable { get; }

    public int VoiceIndex
    {
        get => voiceIndex;
        set => voiceIndex = Math.Max(0, Math.Min(value, installedVoices.Length));
    }

    public string VoiceName
    {
        get => voiceIndex == 0 ? string.Empty : installedVoices[voiceIndex - 1];
        set => SetVoiceFromName(value);
    }

    public string[] VoiceNames
    {
        get
        {
            var result = new string[installedVoices.Length + 1];
            result[0] = Translations.Translate("SETTINGS_TTS_SYSTEM_DEFAULT");
            Array.Copy(installedVoices, 0, result, 1, installedVoices.Length);
            return result;
        }
    }

    public void Speak(string text)
    {
        lock (processLock)
        {
            StopCurrentProcess();

            var startInfo = new ProcessStartInfo
            {
                FileName = SayPath,
                Arguments = BuildArguments(),
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = startInfo };
            if (!process.Start())
            {
                process.Close();
                throw new InvalidOperationException("Failed to start macOS TTS");
            }

            currentSpeech = new SpeechProcessState(process);
            bool monitorQueued = false;
            try
            {
                monitorQueued = ThreadPool.QueueUserWorkItem(MonitorSpeechProcess, currentSpeech);
                if (!monitorQueued)
                {
                    throw new InvalidOperationException("Failed to monitor macOS TTS");
                }
                process.StandardInput.Write(text ?? string.Empty);
                process.StandardInput.Close();
            }
            catch
            {
                StopCurrentProcess();
                if (!monitorQueued)
                {
                    process.Close();
                }
                throw;
            }
        }
    }

    public void Stop()
    {
        lock (processLock)
        {
            StopCurrentProcess();
        }
    }

    private static string[] EnumerateVoices()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = SayPath,
            Arguments = "-v ?",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        var output = new StringBuilder();
        var error = new StringBuilder();
        using (var outputComplete = new ManualResetEvent(false))
        using (var errorComplete = new ManualResetEvent(false))
        using (var process = new Process { StartInfo = startInfo })
        {
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data == null)
                {
                    outputComplete.Set();
                }
                else
                {
                    lock (output)
                    {
                        output.AppendLine(e.Data);
                    }
                }
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data == null)
                {
                    errorComplete.Set();
                }
                else
                {
                    lock (error)
                    {
                        error.AppendLine(e.Data);
                    }
                }
            };

            if (!process.Start())
            {
                throw new InvalidOperationException("Failed to query macOS TTS voices");
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            if (!process.WaitForExit(VoiceEnumerationTimeout))
            {
                process.Kill();
                process.WaitForExit();
                outputComplete.WaitOne(VoiceEnumerationTimeout);
                errorComplete.WaitOne(VoiceEnumerationTimeout);
                throw new TimeoutException("Timed out while querying macOS TTS voices");
            }

            process.WaitForExit();
            if (!outputComplete.WaitOne(VoiceEnumerationTimeout) || !errorComplete.WaitOne(VoiceEnumerationTimeout))
            {
                throw new TimeoutException("Timed out while reading the macOS TTS voice list");
            }
            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"Failed to query macOS TTS voices: {error.ToString().Trim()}");
            }

            var result = new List<string>();
            using (var reader = new StringReader(output.ToString()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int descriptionStart = line.IndexOf(" #", StringComparison.Ordinal);
                    string voiceAndLocale = (descriptionStart >= 0 ? line.Substring(0, descriptionStart) : line).TrimEnd();
                    int localeStart = voiceAndLocale.LastIndexOfAny([' ', '\t']);
                    if (localeStart <= 0)
                    {
                        continue;
                    }

                    string voiceName = voiceAndLocale.Substring(0, localeStart).TrimEnd();
                    if (voiceName.Length > 0 && !result.Contains(voiceName))
                    {
                        result.Add(voiceName);
                    }
                }
            }
            return result.ToArray();
        }
    }

    private string BuildArguments()
    {
        int wordsPerMinute = (int)Math.Round(175d * Math.Pow(1.08d, TrainDisplaySettings.TTSRate));
        string arguments = $"-r {wordsPerMinute}";
        if (voiceIndex > 0)
        {
            arguments = $"-v {QuoteArgument(installedVoices[voiceIndex - 1])} {arguments}";
        }
        return arguments;
    }

    private static string QuoteArgument(string value) => $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";

    private void SetVoiceFromName(string voiceName)
    {
        if (string.IsNullOrEmpty(voiceName))
        {
            VoiceIndex = 0;
            return;
        }

        int index = Array.IndexOf(installedVoices, voiceName);
        if (index >= 0)
        {
            VoiceIndex = index + 1;
            return;
        }

        Logging.Error($"Voice '{voiceName}' not found; using the first installed voice");
        VoiceIndex = installedVoices.Length > 0 ? 1 : 0;
    }

    private void StopCurrentProcess()
    {
        if (currentSpeech == null)
        {
            return;
        }

        var speech = currentSpeech;
        speech.IntentionallyStopped = true;
        currentSpeech = null;
        try
        {
            if (!speech.Process.HasExited)
            {
                speech.Process.Kill();
            }
        }
        catch (InvalidOperationException)
        {
            // The process exited between checking and stopping it.
        }
    }

    private void MonitorSpeechProcess(object state)
    {
        var speech = (SpeechProcessState)state;
        string error = string.Empty;
        int exitCode = 0;
        Exception monitorException = null;
        try
        {
            error = speech.Process.StandardError.ReadToEnd();
            speech.Process.WaitForExit();
            exitCode = speech.Process.ExitCode;
        }
        catch (Exception e)
        {
            monitorException = e;
        }

        bool intentionallyStopped;
        lock (processLock)
        {
            intentionallyStopped = speech.IntentionallyStopped;
            if (ReferenceEquals(currentSpeech, speech))
            {
                currentSpeech = null;
            }
        }

        speech.Process.Close();
        if (intentionallyStopped)
        {
            return;
        }

        if (monitorException != null)
        {
            TrainDisplaySettings.TTS = false;
            Logging.LogException(monitorException, "Failed to monitor macOS TTS");
        }
        else if (exitCode != 0)
        {
            TrainDisplaySettings.TTS = false;
            Logging.Error($"macOS TTS failed with exit code {exitCode}: {error.Trim()}");
        }
    }

    private sealed class SpeechProcessState
    {
        internal SpeechProcessState(Process process) => Process = process;

        internal Process Process { get; }
        internal bool IntentionallyStopped { get; set; }
    }
}
