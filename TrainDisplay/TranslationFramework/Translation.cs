﻿using ColossalFramework.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TrainDisplay.Config;
using Log = TrainDisplay.Utils.Log;

namespace TrainDisplay.TranslationFramework
{
    public delegate void LanguageChangedEventHandler(string languageIdentifier);

    /// <summary>
    /// Handles localisation for a mod.
    /// </summary>
    public class Translation
    {
        protected List<Language> _languages = new List<Language>();
        protected Language _currentLanguage = null;
        protected Language _displayLanguage = null;
        protected bool _languagesLoaded = false;
        protected bool _loadLanguageAutomatically = true;
        private const string fallbackLanguage = "en";
        public Language CurrentLanguage
        {
            get
            {
                if (_currentLanguage == null)
                {
                    LoadLanguages();
                }
                return _currentLanguage;
            }
        }

        public Language DisplayLanguage
        {
            get
            {
                if (_displayLanguage == null)
                {
                    LoadLanguages();
                }
                return _displayLanguage;
            }
        }

        public Translation(bool loadLanguageAutomatically = true)
        {
            _loadLanguageAutomatically = loadLanguageAutomatically;
            LocaleManager.eventLocaleChanged += SetCurrentLanguage;
        }

        private void SetCurrentLanguage()
        {
            if (_languages == null || _languages.Count == 0 || !LocaleManager.exists)
            {
                return;
            }
            _currentLanguage = _languages.Find(l => l._uniqueName == LocaleManager.instance.language) ??
                               _languages.Find(l => l._uniqueName == fallbackLanguage);
        }

        public void SetDisplayLanguage()
        {
            if (_languages == null || _languages.Count == 0 || !LocaleManager.exists)
            {
                return;
            }
            string readableName = TrainDisplayConfig.Instance.DisplayLanguage;


            _displayLanguage = readableName == "A_TD_SETTINGS_SYSTEM_LANGUAGE"
                ? _currentLanguage
                : _languages.Find(l => l._readableName == readableName) ?? _currentLanguage;
        }


        /// <summary>
        /// Loads all languages up if not already loaded.
        /// </summary>
        public void LoadLanguages()
        {
            if (!_languagesLoaded && _loadLanguageAutomatically)
            {
                _languagesLoaded = true;
                RefreshLanguages();
                SetCurrentLanguage();
                SetDisplayLanguage();
            }
        }

        /// <summary>
        /// Forces a reload of the languages, even if they're already loaded
        /// </summary>
        public void RefreshLanguages()
        {
            _languages.Clear();

            string basePath = Util.AssemblyPath;

            if (basePath != "")
            {
                string languagePath = basePath + Path.DirectorySeparatorChar + "Locale";

                if (Directory.Exists(languagePath))
                {
                    string[] languageFiles = Directory.GetFiles(languagePath);

                    foreach (string languageFile in languageFiles)
                    {
                        StreamReader reader = new StreamReader(languageFile);
                        Language loadedLanguage = DeserialiseLanguage(reader);

                        if (loadedLanguage != null)
                        {
                            _languages.Add(loadedLanguage);
                        }
                    }
                }
                else
                {
                    Log.Warning("Can't load any localisation files");
                }
            }
        }

        /// <summary>
        /// Deserialise a language file using a TextReader
        /// </summary>
        /// <param name="reader">The text to deserialise</param>
        /// <returns>A deserialised language</returns>
        protected Language DeserialiseLanguage(TextReader reader)
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(Language));

            Language loadedLanguage = (Language)xmlSerialiser.Deserialize(reader);
            reader.Close();

            return loadedLanguage;
        }

        /// <summary>
        /// Returns a list of languages which are available to the mod. This will return readable languages for use on the UI
        /// </summary>
        /// <returns>A list of languages available.</returns>
        public List<string> AvailableLanguagesReadable()
        {
            LoadLanguages();

            List<string> languageNames = new List<string>();

            foreach (Language availableLanguage in _languages)
            {
                languageNames.Add(availableLanguage._readableName);
            }

            return languageNames;
        }

        /// <summary>
        /// Returns a list of languages which are available to the mod. This will return language IDs for searching.
        /// </summary>
        /// <returns>A list of languages available.</returns>
        public List<string> AvailableLanguages()
        {
            LoadLanguages();

            List<string> languageNames = new List<string>();

            foreach (Language availableLanguage in _languages)
            {
                languageNames.Add(availableLanguage._uniqueName);
            }

            return languageNames;
        }

        /// <summary>
        /// Returns a list of Language unique IDs that have the name
        /// </summary>
        /// <param name="name">The name of the language to get IDs for</param>
        /// <returns>A list of IDs that match</returns>
        public List<string> GetLanguageIDsFromName(string name)
        {
            List<string> returnLanguages = new List<string>();

            foreach (Language availableLanguage in _languages)
            {
                if (availableLanguage._readableName == name)
                {
                    returnLanguages.Add(availableLanguage._uniqueName);
                }
            }

            return returnLanguages;
        }

        /// <summary>
        /// Returns whether you can translate into a specific translation ID
        /// </summary>
        /// <param name="translationId">The ID of the translation to check</param>
        /// <returns>Whether a translation into this ID is possible</returns>
        public bool HasTranslation(string translationId)
        {
            LoadLanguages();

            return _currentLanguage != null && _currentLanguage._conversionDictionary.ContainsKey(translationId);
        }

        /// <summary>
        /// Gets a translation for a specific translation ID
        /// </summary>
        /// <param name="translationId">The ID to return the translation for</param>
        /// <returns>A translation of the translationId</returns>
        public string GetTranslation(string translationId, bool displayLang = false)
        {
            LoadLanguages();

            var lang = displayLang ? _displayLanguage : _currentLanguage;

            string translatedText = translationId;

            if (lang != null)
            {
                if (HasTranslation(translationId))
                {
                    translatedText = lang._conversionDictionary[translationId];
                }
                else
                {
                    Log.Warning("Returned translation for language \"" + lang._uniqueName + "\" doesn't contain a suitable translation for \"" + translationId + "\"");
                }
            }
            else
            {
                Log.Warning("Can't get a translation for \"" + translationId + "\" as there is not a language defined");
                Log.Warning(Util.AssemblyPath);
            }

            return translatedText;
        }
    }


}
