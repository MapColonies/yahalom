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
using UnityEngine.ResourceManagement.AsyncOperations;

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

        private string _remoteConfigUrl;
        private bool _showTranslationWarnings;

        private const string HebrewLocaleIdentifier = "he-IL";
        private const string EnglishLocaleIdentifier = "en";
        private const string LocalFilePath = "Translations/Yahalom_HardCoded_Translations.json";
        private const string TargetStringTableName = "Yahalom_HardCoded_Translations";

        private Dictionary<string, TranslationEntry> _hardCodedTranslations = new Dictionary<string, TranslationEntry>();
        private Dictionary<string, TranslationEntry> _fileTranslations = new Dictionary<string, TranslationEntry>();
        private Dictionary<string, TranslationEntry> _remoteTranslations = new Dictionary<string, TranslationEntry>();
        private Dictionary<string, TranslationEntry> _mergedTranslations = new Dictionary<string, TranslationEntry>();

        private bool _isInitialized;

        private TranslationService()
        {
        }

        public async Task InitializeService()
        {
            if (_isInitialized) return;

            //TODO: Get from global config file
            _remoteConfigUrl = string.Empty;

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

            Locale enLocale = LocalizationSettings.AvailableLocales.GetLocale(EnglishLocaleIdentifier);
            Locale heLocale = LocalizationSettings.AvailableLocales.GetLocale(HebrewLocaleIdentifier);

            if (enLocale == null && heLocale == null)
            {
                Debug.LogWarning($"TranslationService: No matching locales found for '{EnglishLocaleIdentifier}' or '{HebrewLocaleIdentifier}'.");
                return;
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
                return;
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

                _hardCodedTranslations[key] = new TranslationEntry
                {
                    Key = key,
                    English = enVal,
                    Hebrew = heVal
                };
            }
        }

        private static async Task<TranslationConfig> LoadFromFileAsync()
        {
            string path = Path.Combine(Application.streamingAssetsPath, LocalFilePath);

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
        }

        private async Task ApplyToUnityLocalization()
        {
            await LocalizationSettings.InitializationOperation.Task;

            Locale hebrewLocale = LocalizationSettings.AvailableLocales.GetLocale(HebrewLocaleIdentifier);
            Locale englishLocale = LocalizationSettings.AvailableLocales.GetLocale(EnglishLocaleIdentifier);

            if (hebrewLocale == null)
            {
                Debug.LogError($"TranslationService: Hebrew locale '{HebrewLocaleIdentifier}' not found. Make sure it's added to Localization Settings.");
                return;
            }

            if (englishLocale == null)
            {
                Debug.LogError($"TranslationService: English locale '{EnglishLocaleIdentifier}' not found. Make sure it's added to Localization Settings.");
                return;
            }

            AsyncOperationHandle<StringTable> hebrewTableOperation = LocalizationSettings.StringDatabase.GetTableAsync(TargetStringTableName, hebrewLocale);
            AsyncOperationHandle<StringTable> englishTableOperation = LocalizationSettings.StringDatabase.GetTableAsync(TargetStringTableName, englishLocale);

            await hebrewTableOperation.Task;
            await englishTableOperation.Task;

            if (hebrewTableOperation.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"TranslationService: Could not load Hebrew String Table '{TargetStringTableName}'. Error: {hebrewTableOperation.OperationException}");
                return;
            }

            if (englishTableOperation.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"TranslationService: Could not load English String Table '{TargetStringTableName}'. Error: {englishTableOperation.OperationException}");
                return;
            }

            StringTable hebrewStringTable = hebrewTableOperation.Result;
            StringTable englishStringTable = englishTableOperation.Result;

            foreach (KeyValuePair<string, TranslationEntry> entry in _mergedTranslations)
            {
                hebrewStringTable.AddEntry(entry.Key, entry.Value.Hebrew);
                englishStringTable.AddEntry(entry.Key, entry.Value.English);
            }
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

            if (IsLocale(HebrewLocaleIdentifier))
            {
                if (!string.IsNullOrEmpty(entry.Hebrew)) return entry.Hebrew;
                //Ask asaf about ux
                if (!string.IsNullOrEmpty(entry.English)) return entry.English;
                return key;
            }

            if (IsLocale(EnglishLocaleIdentifier))
            {
                if (!string.IsNullOrEmpty(entry.English)) return entry.English;
                //Ask asaf about ux
                if (!string.IsNullOrEmpty(entry.Hebrew)) return entry.Hebrew;
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
