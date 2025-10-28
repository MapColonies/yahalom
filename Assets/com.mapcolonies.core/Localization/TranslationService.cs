using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using com.mapcolonies.core.Localization.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Networking;

namespace com.mapcolonies.core.Localization
{
    public class TranslationService
    {
        private static TranslationService _instance;

        public static TranslationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TranslationService();
                }

                return _instance;
            }
        }

        private string _targetStringTableName;
        private string _hebrewLocaleIdentifier;
        private string _englishLocaleIdentifier;
        private string _remoteConfigUrl;
        private string _localFilePath;

        private bool _showTranslationWarnings;

        private Dictionary<string, TranslationEntry> _hardCodedTranslations = new Dictionary<string, TranslationEntry>();
        private Dictionary<string, TranslationEntry> _fileTranslations = new Dictionary<string, TranslationEntry>();
        private Dictionary<string, TranslationEntry> _remoteTranslations = new Dictionary<string, TranslationEntry>();
        private Dictionary<string, TranslationEntry> _mergedTranslations = new Dictionary<string, TranslationEntry>();

        private bool _isInitialized;

        private TranslationService()
        {
        }

        public async Task InitializeService(string targetStringTableName, string hebrewLocaleIdentifier, string englishLocaleIdentifier, string remoteConfigUrl, string localFilePath)
        {
            if (_isInitialized) return;

            _targetStringTableName = targetStringTableName;
            _hebrewLocaleIdentifier = hebrewLocaleIdentifier;
            _englishLocaleIdentifier = englishLocaleIdentifier;
            _remoteConfigUrl = remoteConfigUrl;
            _localFilePath = localFilePath;

            Debug.Log("TranslationService: Initializing...");

            await LoadHardCodedTranslations();
            TranslationConfig fileConfig = await LoadFromFileAsync();

            if (fileConfig != null)
            {
                _fileTranslations = ConfigToDictionary(fileConfig);
                _showTranslationWarnings = fileConfig.ShowTranslationWarnings;
            }

            TranslationConfig remoteConfig = await LoadFromRemoteAsync();

            if (remoteConfig != null)
            {
                _remoteTranslations = ConfigToDictionary(remoteConfig);
                _showTranslationWarnings = remoteConfig.ShowTranslationWarnings;
            }

            MergeTranslations();
            await ApplyToUnityLocalization();

            _isInitialized = true;
            Debug.Log("TranslationService: Initialization complete.");
        }

        private async Task LoadHardCodedTranslations()
        {
            _hardCodedTranslations = new Dictionary<string, TranslationEntry>();
            await LocalizationSettings.InitializationOperation.Task;

            Locale enLocale = LocalizationSettings.AvailableLocales.GetLocale(_englishLocaleIdentifier);
            Locale heLocale = LocalizationSettings.AvailableLocales.GetLocale(_hebrewLocaleIdentifier);

            if (enLocale == null && heLocale == null)
            {
                Debug.LogWarning($"TranslationService: No matching locales found for '{_englishLocaleIdentifier}' or '{_hebrewLocaleIdentifier}'.");
                return;
            }

            StringTable enTable = null;
            StringTable heTable = null;

            if (enLocale != null)
            {
                enTable = await LocalizationSettings.StringDatabase.GetTableAsync(_targetStringTableName, enLocale).Task;
            }

            if (heLocale != null)
            {
                heTable = await LocalizationSettings.StringDatabase.GetTableAsync(_targetStringTableName, heLocale).Task;
            }

            if (enTable == null && heTable == null)
            {
                Debug.LogWarning($"TranslationService: StringTable '{_targetStringTableName}' not found for EN or HE at runtime.");
                return;
            }

            var allKeys = new HashSet<string>();

            if (enTable != null)
            {
                foreach (var entry in enTable.Values) allKeys.Add(entry.Key);
            }

            if (heTable != null)
            {
                foreach (var entry in heTable.Values) allKeys.Add(entry.Key);
            }

            foreach (var key in allKeys)
            {
                var enVal = enTable?.GetEntry(key)?.LocalizedValue;
                var heVal = heTable?.GetEntry(key)?.LocalizedValue;

                _hardCodedTranslations[key] = new TranslationEntry
                {
                    Key     = key,
                    English = enVal,
                    Hebrew  = heVal
                };
            }
        }

        private async Task<TranslationConfig> LoadFromFileAsync()
        {
            string path = Path.Combine(Application.streamingAssetsPath, _localFilePath);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"TranslationService: Local translation file not found at {path}");
                return null;
            }

            try
            {
                string jsonContent = await Task.Run(() => File.ReadAllText(path));
                return JsonConvert.DeserializeObject<TranslationConfig>(jsonContent);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"TranslationService: Failed to parse local file. Error: {ex.Message}");
                return null;
            }
        }

        private async Task<TranslationConfig> LoadFromRemoteAsync()
        {
            if (string.IsNullOrEmpty(_remoteConfigUrl))
            {
                Debug.Log("TranslationService: Remote config URL is empty, skipping.");
                return null;
            }

            Debug.Log($"TranslationService: Loading from remote: {_remoteConfigUrl}");

            using (UnityWebRequest www = UnityWebRequest.Get(_remoteConfigUrl))
            {
                var operation = www.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        string jsonContent = www.downloadHandler.text;
                        return JsonConvert.DeserializeObject<TranslationConfig>(jsonContent);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"TranslationService: Failed to parse remote config. Error: {ex.Message}");
                        return null;
                    }
                }

                Debug.LogError($"TranslationService: Failed to download remote config. Error: {www.error}");
                return null;
            }
        }

        private void MergeTranslations()
        {
            _mergedTranslations.Clear();

            foreach (var entry in _hardCodedTranslations)
            {
                _mergedTranslations[entry.Key] = entry.Value;
            }

            foreach (var entry in _fileTranslations)
            {
                _mergedTranslations[entry.Key] = entry.Value;
            }

            foreach (var entry in _remoteTranslations)
            {
                _mergedTranslations[entry.Key] = entry.Value;
            }

            Debug.Log($"TranslationService: Merged {_mergedTranslations.Count} total translations.");
        }

        private async Task ApplyToUnityLocalization()
        {
            await LocalizationSettings.InitializationOperation.Task;

            var hebrewLocale = LocalizationSettings.AvailableLocales.GetLocale(_hebrewLocaleIdentifier);
            var englishLocale = LocalizationSettings.AvailableLocales.GetLocale(_englishLocaleIdentifier);

            if (hebrewLocale == null)
            {
                Debug.LogError($"TranslationService: Hebrew locale '{_hebrewLocaleIdentifier}' not found. Make sure it's added to Localization Settings.");
                return;
            }

            if (englishLocale == null)
            {
                Debug.LogError($"TranslationService: English locale '{_englishLocaleIdentifier}' not found. Make sure it's added to Localization Settings.");
                return;
            }

            Debug.Log($"TranslationService: Applying translations to String Table '{_targetStringTableName}' for locale '{hebrewLocale.Identifier.Code}'...");

            var hebrewTableOperation = LocalizationSettings.StringDatabase.GetTableAsync(_targetStringTableName, hebrewLocale);
            var englishTableOperation = LocalizationSettings.StringDatabase.GetTableAsync(_targetStringTableName, englishLocale);

            await hebrewTableOperation.Task;
            await englishTableOperation.Task;

            if (hebrewTableOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"TranslationService: Could not load Hebrew String Table '{_targetStringTableName}'. Error: {hebrewTableOperation.OperationException}");
                return;
            }

            if (englishTableOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"TranslationService: Could not load English String Table '{_targetStringTableName}'. Error: {englishTableOperation.OperationException}");
                return;
            }

            StringTable hebrewStringTable = hebrewTableOperation.Result;
            StringTable englishStringTable = englishTableOperation.Result;

            foreach (var entry in _mergedTranslations)
            {
                hebrewStringTable.AddEntry(entry.Key, entry.Value.Hebrew);
                englishStringTable.AddEntry(entry.Key, entry.Value.English);
            }

            Debug.Log($"TranslationService: Successfully updated String Table '{_targetStringTableName}'.");
        }

        public TranslationEntry GetTranslationEntry(string key)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning($"TranslationService: GetTranslationEntry called before service is initialized.");
                return new TranslationEntry { Key = key, English = key, Hebrew = key };
            }

            if (_mergedTranslations.TryGetValue(key, out TranslationEntry entry))
            {
                return entry;
            }

            return null;
        }

        private static bool IsLocale(string targetCode)
        {
            Locale selected = LocalizationSettings.SelectedLocale;
            string code = selected.Identifier.Code;

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(targetCode))
                return false;

            return code == targetCode;
        }

        public string Translate(string key)
        {
            TranslationEntry entry = GetTranslationEntry(key);

            if (entry == null)
            {
                if (_showTranslationWarnings)
                {
                    Debug.LogWarning($"TranslationService: Translation not found for key: {key}");
                }
                return key;
            }

            if (IsLocale(_hebrewLocaleIdentifier))
            {
                if (!string.IsNullOrEmpty(entry.Hebrew)) return entry.Hebrew;
                if (!string.IsNullOrEmpty(entry.English)) return entry.English;
                return key;
            }

            if (IsLocale(_englishLocaleIdentifier))
            {
                if (!string.IsNullOrEmpty(entry.English)) return entry.English;
                if (!string.IsNullOrEmpty(entry.Hebrew)) return entry.Hebrew;
                return key;
            }

            if (!string.IsNullOrEmpty(entry.English)) return entry.English;
            if (!string.IsNullOrEmpty(entry.Hebrew)) return entry.Hebrew;
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
                return config.Words.GroupBy(e => e.Key).ToDictionary(g => g.Key, g => g.Last());
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"TranslationService: Error converting config list to dictionary. {ex.Message}");
                return new Dictionary<string, TranslationEntry>();
            }
        }
    }
}
