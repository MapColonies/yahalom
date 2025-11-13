using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.mapcolonies.core.Localization.Constants;
using com.mapcolonies.core.Localization.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace com.mapcolonies.core.Localization
{
    public interface ITranslationService
    {
        UniTask InitializeService(string localeIdentifier);
        string Translate(string key);
    }

    public class TranslationService : ITranslationService, IDisposable
    {
        private string _remoteConfigUrl;
        private bool _showTranslationWarnings;

        private static readonly string LocalFilePath =
            Path.ChangeExtension(
                Path.Combine("Translations", LocalizationConstants.TranslationsFileName),
                ".json"
            );

        private const string TargetStringTableName = "Yahalom_HardCoded_Translations";

        private Dictionary<string, TranslationEntry> _translations = new Dictionary<string, TranslationEntry>();

        private bool _isInitialized;

        public TranslationService()
        {
        }

        public async UniTask InitializeService(string localeIdentifier)
        {
            if (_isInitialized) return;

            //TODO: Get from global config file
            _remoteConfigUrl = string.Empty;

            await SetLanguage(localeIdentifier);
            await SetTranslations();
            await ApplyToUnityLocalization();

            _isInitialized = true;
        }

        private async UniTask SetLanguage(string localeIdentifier)
        {
            await LocalizationSettings.InitializationOperation.Task;

            if (!string.IsNullOrWhiteSpace(localeIdentifier))
            {
                ILocalesProvider locales = LocalizationSettings.AvailableLocales;
                Locale target = locales.GetLocale(localeIdentifier);

                if (target != null)
                {
                    LocalizationSettings.SelectedLocale = target;
                }
                else
                {
                    Debug.LogWarning($"TranslationService: Requested locale '{localeIdentifier}' not found.");
                }
            }
        }

        private async UniTask<Dictionary<string, TranslationEntry>> LoadHardCodedTranslations()
        {
            Dictionary<string, TranslationEntry> hardCodedTranslations = new Dictionary<string, TranslationEntry>();
            await LocalizationSettings.InitializationOperation.Task;

            HashSet<string> allKeys = new HashSet<string>();
            Dictionary<Locale, StringTable> loadedTables = new Dictionary<Locale, StringTable>();

            foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
            {
                AsyncOperationHandle<StringTable> tableHandle = LocalizationSettings.StringDatabase.GetTableAsync(TargetStringTableName, locale);
                StringTable table = await tableHandle.Task;

                if (table != null)
                {
                    loadedTables.Add(locale, table);

                    foreach (StringTableEntry entry in table.Values)
                    {
                        allKeys.Add(entry.Key);
                    }
                }
            }

            if (loadedTables.Count == 0)
            {
                Debug.LogWarning($"TranslationService: StringTable '{TargetStringTableName}' not found for ANY locale at runtime.");
                return hardCodedTranslations;
            }

            foreach (string key in allKeys)
            {
                TranslationEntry newEntry = new TranslationEntry { Key = key };

                foreach (KeyValuePair<Locale, StringTable> tablePair in loadedTables)
                {
                    Locale locale = tablePair.Key;
                    StringTable table = tablePair.Value;

                    string localeCode = locale.Identifier.Code;
                    string localizedValue = table.GetEntry(key)?.LocalizedValue;

                    if (!string.IsNullOrEmpty(localizedValue))
                    {
                        newEntry.LocalizedValues[localeCode] = localizedValue;
                    }
                }

                hardCodedTranslations[key] = newEntry;
            }

            return hardCodedTranslations;
        }

        private static async UniTask<TranslationConfig> LoadFromFileAsync()
        {
            string path = Path.Combine(Application.streamingAssetsPath, LocalFilePath);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"TranslationService: Local translation file not found at {path}");
                return null;
            }

            try
            {
                string jsonContent = await File.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject<TranslationConfig>(jsonContent);
            }
            catch (Exception ex)
            {
                Debug.LogError($"TranslationService: Failed to parse local file. Error: {ex.Message}");
                return null;
            }
        }

        private async UniTask<TranslationConfig> LoadFromRemoteAsync()
        {
            if (string.IsNullOrEmpty(_remoteConfigUrl))
            {
                Debug.LogWarning("TranslationService: Remote config URL is empty, skipping.");
                return null;
            }

            using (UnityWebRequest www = UnityWebRequest.Get(_remoteConfigUrl))
            {
                try
                {
                    await www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        string jsonContent = www.downloadHandler.text;
                        return JsonConvert.DeserializeObject<TranslationConfig>(jsonContent);
                    }

                    Debug.LogError($"TranslationService: Failed to download remote config. Error: {www.error}");
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"TranslationService: Failed to parse remote config. Error: {ex.Message} (URL: {_remoteConfigUrl})");
                    return null;
                }
            }
        }

        private async UniTask SetTranslations()
        {
            Dictionary<string, TranslationEntry> hardCodedTranslations = await LoadHardCodedTranslations();

            Dictionary<string, TranslationEntry> fileTranslations = new Dictionary<string, TranslationEntry>();
            TranslationConfig fileConfig = await LoadFromFileAsync();

            if (fileConfig != null)
            {
                fileTranslations = ConfigToDictionary(fileConfig);
                _showTranslationWarnings = fileConfig.ShowTranslationWarnings;
            }

            Dictionary<string, TranslationEntry> remoteTranslations = new Dictionary<string, TranslationEntry>();
            TranslationConfig remoteConfig = await LoadFromRemoteAsync();

            if (remoteConfig != null)
            {
                remoteTranslations = ConfigToDictionary(remoteConfig);
                _showTranslationWarnings = remoteConfig.ShowTranslationWarnings;
            }

            _translations = hardCodedTranslations
                .Concat(fileTranslations)
                .Concat(remoteTranslations)
                .GroupBy(kvp => kvp.Key)
                .ToDictionary(g => g.Key, g => g.Last().Value);
        }

        private async UniTask ApplyToUnityLocalization()
        {
            await LocalizationSettings.InitializationOperation.Task;
            Dictionary<string, StringTable> loadedStringTables = new Dictionary<string, StringTable>();

            foreach (KeyValuePair<string, TranslationEntry> entryPair in _translations)
            {
                string key = entryPair.Key;
                TranslationEntry entry = entryPair.Value;

                foreach (KeyValuePair<string, string> localizedPair in entry.LocalizedValues)
                {
                    string localeCode = localizedPair.Key;
                    string localizedValue = localizedPair.Value;

                    if (string.IsNullOrEmpty(localizedValue)) continue;

                    if (!loadedStringTables.TryGetValue(localeCode, out StringTable targetTable))
                    {
                        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

                        if (locale == null)
                        {
                            Debug.LogWarning($"TranslationService: Cannot apply translation for key '{key}'. Locale '{localeCode}' not found in Localization Settings.");
                            continue;
                        }

                        AsyncOperationHandle<StringTable> tableHandle = LocalizationSettings.StringDatabase.GetTableAsync(TargetStringTableName, locale);
                        targetTable = await tableHandle.Task;

                        if (targetTable == null)
                        {
                            Debug.LogError($"TranslationService: Could not load String Table '{TargetStringTableName}' for locale '{localeCode}'.");
                            continue;
                        }

                        loadedStringTables[localeCode] = targetTable;
                    }

                    targetTable.AddEntry(key, localizedValue);
                }
            }
        }

        private TranslationEntry GetTranslationEntry(string key)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning($"TranslationService: GetTranslationEntry called before service is initialized.");
                return null;
            }

            if (_translations.TryGetValue(key, out TranslationEntry entry))
            {
                return entry;
            }

            return null;
        }

        public string Translate(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            TranslationEntry entry = GetTranslationEntry(key);

            if (entry == null)
            {
                if (_showTranslationWarnings)
                {
                    Debug.LogWarning($"TranslationService: Translation not found for key: {key}");
                }

                return key;
            }

            Locale selectedLocale = LocalizationSettings.SelectedLocale;

            if (selectedLocale == null)
            {
                Debug.LogWarning("TranslationService.Translate: SelectedLocale is null.");
                return key;
            }

            string currentLocaleCode = selectedLocale.Identifier.Code;

            if (entry.LocalizedValues.TryGetValue(currentLocaleCode, out string translatedValue))
            {
                if (!string.IsNullOrEmpty(translatedValue))
                {
                    return translatedValue;
                }
            }

            if (entry.LocalizedValues.TryGetValue(LocalizationConstants.EnglishLocaleIdentifier, out string englishValue))
            {
                if (!string.IsNullOrEmpty(englishValue))
                {
                    return englishValue;
                }
            }

            if (entry.LocalizedValues.Count > 0)
            {
                string anyValue = entry.LocalizedValues.Values.FirstOrDefault(v => !string.IsNullOrEmpty(v));

                if (anyValue != null)
                {
                    return anyValue;
                }
            }

            if (_showTranslationWarnings)
            {
                Debug.LogWarning($"TranslationService: Translation found for key '{key}', but no value exists for current locale '{currentLocaleCode}' or fallback 'en'.");
            }

            return key;
        }

        private static Dictionary<string, TranslationEntry> ConfigToDictionary(TranslationConfig config)
        {
            if (config == null || config.Words == null)
            {
                return new Dictionary<string, TranslationEntry>();
            }

            try
            {
                return config.Words
                    .Where(e => e != null && e.Key != null)
                    .GroupBy(e => e.Key)
                    .ToDictionary(g => g.Key, g => g.Last());
            }
            catch (Exception ex)
            {
                Debug.LogError($"TranslationService: Error converting config list to dictionary. {ex.Message}");
                return new Dictionary<string, TranslationEntry>();
            }
        }

        public void Dispose()
        {
            _isInitialized = false;
            _translations?.Clear();
        }
    }
}
