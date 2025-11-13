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
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

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

            Locale enLocale = LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.EnglishLocaleIdentifier);
            Locale heLocale = LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.HebrewLocaleIdentifier);

            if (enLocale == null && heLocale == null)
            {
                Debug.LogWarning($"TranslationService: No matching locales found for '{LocalizationConstants.EnglishLocaleIdentifier}' or '{LocalizationConstants.HebrewLocaleIdentifier}'.");
                return null;
            }

            StringTable enTable = null;
            StringTable heTable = null;

            if (enLocale != null)
            {
                enTable = await LocalizationSettings.StringDatabase.GetTableAsync(TargetStringTableName, enLocale).Task;
            }

            if (heLocale != null)
            {
                heTable = await LocalizationSettings.StringDatabase.GetTableAsync(TargetStringTableName, heLocale).Task;
            }

            if (enTable == null && heTable == null)
            {
                Debug.LogWarning($"TranslationService: StringTable '{TargetStringTableName}' not found for EN or HE at runtime.");
                return null;
            }

            HashSet<string> allKeys = new HashSet<string>();

            if (enTable != null)
            {
                foreach (StringTableEntry entry in enTable.Values) allKeys.Add(entry.Key);
            }

            if (heTable != null)
            {
                foreach (StringTableEntry entry in heTable.Values) allKeys.Add(entry.Key);
            }

            foreach (string key in allKeys)
            {
                string enVal = enTable?.GetEntry(key)?.LocalizedValue;
                string heVal = heTable?.GetEntry(key)?.LocalizedValue;

                hardCodedTranslations[key] = new TranslationEntry
                {
                    Key = key,
                    English = enVal,
                    Hebrew = heVal
                };
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
            Locale hebrewLocale = LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.HebrewLocaleIdentifier);
            Locale englishLocale = LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.EnglishLocaleIdentifier);

            if (hebrewLocale == null || englishLocale == null)
            {
                Debug.LogError($"TranslationService: One or both locales not found. HE: '{LocalizationConstants.HebrewLocaleIdentifier}', EN: '{LocalizationConstants.EnglishLocaleIdentifier}'. Make sure they are in Localization Settings.");
                return;
            }

            AsyncOperationHandle<StringTable> hebrewTableOperation = LocalizationSettings.StringDatabase.GetTableAsync(TargetStringTableName, hebrewLocale);
            AsyncOperationHandle<StringTable> englishTableOperation = LocalizationSettings.StringDatabase.GetTableAsync(TargetStringTableName, englishLocale);

            await hebrewTableOperation.Task;
            await englishTableOperation.Task;

            if (hebrewTableOperation.Status != AsyncOperationStatus.Succeeded || englishTableOperation.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"TranslationService: Could not load String Tables. HE Error: {hebrewTableOperation.OperationException} | EN Error: {englishTableOperation.OperationException}");
                return;
            }

            StringTable hebrewStringTable = hebrewTableOperation.Result;
            StringTable englishStringTable = englishTableOperation.Result;

            foreach (KeyValuePair<string, TranslationEntry> entry in _translations)
            {
                hebrewStringTable.AddEntry(entry.Key, entry.Value.Hebrew);
                englishStringTable.AddEntry(entry.Key, entry.Value.English);
            }
        }

        private TranslationEntry GetTranslationEntry(string key)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning($"TranslationService: GetTranslationEntry called before service is initialized.");
                return new TranslationEntry { Key = key, English = key, Hebrew = key };
            }

            if (_translations.TryGetValue(key, out TranslationEntry entry))
            {
                return entry;
            }

            return null;
        }

        private static bool IsLocale(string targetCode)
        {
            Locale selected = LocalizationSettings.SelectedLocale;

            if (selected == null)
            {
                Debug.LogWarning("IsLocale: LocalizationSettings.SelectedLocale is null.");
                return false;
            }

            string code = selected.Identifier.Code;
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(targetCode))
                return false;

            return code == targetCode;
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

            if (IsLocale(LocalizationConstants.HebrewLocaleIdentifier))
            {
                if (!string.IsNullOrEmpty(entry.Hebrew)) return entry.Hebrew;
                return key;
            }

            if (IsLocale(LocalizationConstants.EnglishLocaleIdentifier))
            {
                if (!string.IsNullOrEmpty(entry.English)) return entry.English;
                return key;
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
