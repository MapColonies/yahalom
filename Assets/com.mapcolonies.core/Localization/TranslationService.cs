using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

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
    public string value;
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
    private string _remoteConfigUrl;
    private string _localFilePath;

    private bool showTranslationWarnings;

    private Dictionary<string, string> _hardCodedTranslations = new Dictionary<string, string>();
    private Dictionary<string, string> _fileTranslations = new Dictionary<string, string>();
    private Dictionary<string, string> _remoteTranslations = new Dictionary<string, string>();

    private Dictionary<string, string> _mergedTranslations = new Dictionary<string, string>();

    private bool _isInitialized;

    private TranslationService()
    {
    }

    public async void InitializeService(string targetStringTableName, string hebrewLocaleIdentifier, string remoteConfigUrl, string localFilePath)
    {
        if (_isInitialized) return;

        _targetStringTableName = targetStringTableName;
        _hebrewLocaleIdentifier = hebrewLocaleIdentifier;
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
        _hardCodedTranslations = new Dictionary<string, string>
        {
            { "welcome", "ברוך הבא" },
            { "test", "בדיקה (קשיח)" }
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
            return JsonUtility.FromJson<TranslationConfig>(jsonContent);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"TranslationService: Failed to parse local file. Error: {ex.Message}");
            return null;
        }
    }

    private async Task<TranslationConfig> LoadFromRemoteAsync()
    {
        Debug.Log($"TranslationService: Loading from remote: {_remoteConfigUrl}");

        using (UnityWebRequest www = UnityWebRequest.Get(_remoteConfigUrl))
        {
            var operation = www.SendWebRequest();
            await operation;

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonContent = www.downloadHandler.text;
                    return JsonUtility.FromJson<TranslationConfig>(jsonContent);
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

        if (hebrewLocale == null)
        {
            Debug.LogError($"TranslationService: Hebrew locale '{_hebrewLocaleIdentifier}' not found. Make sure it's added to Localization Settings.");
            return;
        }

        Debug.Log($"TranslationService: Applying translations to String Table '{_targetStringTableName}' for locale '{hebrewLocale.Identifier.Code}'...");

        var tableOperation = LocalizationSettings.StringDatabase.GetTableAsync(_targetStringTableName, hebrewLocale);
        await tableOperation.Task;

        if (tableOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"TranslationService: Could not load String Table '{_targetStringTableName}'. Error: {tableOperation.OperationException}");
            return;
        }

        StringTable stringTable = tableOperation.Result;

        // Add/Update entries in the table
        foreach (var entry in _mergedTranslations)
        {
            // AddEntry will update the value if the key already exists
            stringTable.AddEntry(entry.Key, entry.Value);
        }

        Debug.Log($"TranslationService: Successfully updated String Table '{_targetStringTableName}'.");
    }

    public string Translate(string key)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning($"TranslationService: Translate called before service is initialized. Returning key '{key}'.");
            return key;
        }

        if (_mergedTranslations.TryGetValue(key, out string value))
        {
            return value;
        }

        if (showTranslationWarnings)
        {
            Debug.LogWarning($"TranslationService: Translation not found for key: {key}");
        }

        return key;
    }

    private static Dictionary<string, string> ConfigToDictionary(TranslationConfig config)
    {
        if (config == null || config.words == null)
        {
            return new Dictionary<string, string>();
        }

        try
        {
            return config.words.GroupBy(e => e.key)
                .ToDictionary(g => g.Key, g => g.Last().value);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"TranslationService: Error converting config list to dictionary. {ex.Message}");
            return new Dictionary<string, string>();
        }
    }
}
