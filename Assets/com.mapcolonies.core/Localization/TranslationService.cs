using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

[System.Serializable]
public class TranslationConfig
{
    public bool showTranslationWarnings;
    public List<TranslationEntry> words = new List<TranslationEntry>();
}

[System.Serializable]
public class TranslationEntry
{
    public string key;
    public string english;
    public string hebrew;
}

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

    private bool showTranslationWarnings;

    private Dictionary<string, TranslationEntry> _hardCodedTranslations = new Dictionary<string, TranslationEntry>();
    private Dictionary<string, TranslationEntry> _fileTranslations = new Dictionary<string, TranslationEntry>();
    private Dictionary<string, TranslationEntry> _remoteTranslations = new Dictionary<string, TranslationEntry>();
    private Dictionary<string, TranslationEntry> _mergedTranslations = new Dictionary<string, TranslationEntry>();

    private bool _isInitialized;

    private TranslationService()
    {
    }

    public async void InitializeService(string targetStringTableName, string hebrewLocaleIdentifier, string englishLocaleIdentifier, string remoteConfigUrl, string localFilePath)
    {
        if (_isInitialized) return;

        _targetStringTableName = targetStringTableName;
        _hebrewLocaleIdentifier = hebrewLocaleIdentifier;
        _englishLocaleIdentifier = englishLocaleIdentifier;
        _remoteConfigUrl = remoteConfigUrl;
        _localFilePath = localFilePath;

        Debug.Log("TranslationService: Initializing...");

        LoadHardCodedTranslations();

        TranslationConfig fileConfig = await LoadFromFileAsync();

        if (fileConfig != null)
        {
            _fileTranslations = ConfigToDictionary(fileConfig);
            showTranslationWarnings = fileConfig.showTranslationWarnings;
        }

        TranslationConfig remoteConfig = await LoadFromRemoteAsync();

        if (remoteConfig != null)
        {
            _remoteTranslations = ConfigToDictionary(remoteConfig);
            showTranslationWarnings = remoteConfig.showTranslationWarnings;
        }

        MergeTranslations();
        await ApplyToUnityLocalization();

        _isInitialized = true;
        Debug.Log("TranslationService: Initialization complete.");
    }

    private void LoadHardCodedTranslations()
    {
        _hardCodedTranslations = new Dictionary<string, TranslationEntry>
        {
            { "welcome", new TranslationEntry { key = "welcome", english = "Welcome", hebrew = "ברוך הבא (קשיח)" } },
            { "test", new TranslationEntry { key = "test", english = "Test", hebrew = "בדיקה (קשיח)" } }
        };
    }

    private async Task<TranslationConfig> LoadFromFileAsync()
    {
        string path = Path.Combine(Application.streamingAssetsPath, _localFilePath);
        Debug.Log($"TranslationService: Loading from file: {path}");

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

            // Await the operation without blocking the main thread
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
            hebrewStringTable.AddEntry(entry.Key, entry.Value.hebrew);
            englishStringTable.AddEntry(entry.Key, entry.Value.english);
        }

        Debug.Log($"TranslationService: Successfully updated String Table '{_targetStringTableName}'.");
    }

    public TranslationEntry GetTranslationEntry(string key)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning($"TranslationService: GetTranslationEntry called before service is initialized.");
            return new TranslationEntry { key = key, english = key, hebrew = key };
        }

        if (_mergedTranslations.TryGetValue(key, out TranslationEntry entry))
        {
            return entry;
        }

        return null;
    }

    public string Translate(string key)
    {
        var entry = GetTranslationEntry(key);
        if (entry != null)
        {
            return entry.hebrew;
        }

        if (showTranslationWarnings)
        {
            Debug.LogWarning($"TranslationService: Translation not found for key: {key}");
        }

        return key;
    }

    private static Dictionary<string, TranslationEntry> ConfigToDictionary(TranslationConfig config)
    {
        if (config == null || config.words == null)
        {
            return new Dictionary<string, TranslationEntry>();
        }

        try
        {
            return config.words.GroupBy(e => e.key).ToDictionary(g => g.Key, g => g.Last());
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"TranslationService: Error converting config list to dictionary. {ex.Message}");
            return new Dictionary<string, TranslationEntry>();
        }
    }
}
