using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationTest : MonoBehaviour
{
    public string targetStringTableName;
    public string hebrewLocaleIdentifier;
    public string englishLocaleIdentifier;
    //public string remoteConfigUrl = "http://your-server.com/remote-translations.json";
    public string localFilePath;


    private Locale _hebrewLocale;
    private Locale _englishLocale;

    void Start()
    {
        Debug.Log("TranslationServiceTester: Initializing Translation Service...");

        TranslationService.Instance.InitializeService(
            targetStringTableName,
            hebrewLocaleIdentifier,
            englishLocaleIdentifier,
            "",
            localFilePath
        );

        StartCoroutine(TestTranslationAfterDelay(3.0f));
        StartCoroutine(FetchLocales());
    }

    private IEnumerator FetchLocales()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;

        // Find the locales using the identifiers
        _hebrewLocale = LocalizationSettings.AvailableLocales.GetLocale(hebrewLocaleIdentifier);
        _englishLocale = LocalizationSettings.AvailableLocales.GetLocale(englishLocaleIdentifier);

        if (_hebrewLocale == null)
        {
            Debug.LogError($"LocalizationTest: Could not find Hebrew locale with identifier '{hebrewLocaleIdentifier}'");
        }

        if (_englishLocale == null)
        {
            Debug.LogError($"LocalizationTest: Could not find English locale with identifier '{englishLocaleIdentifier}'");
        }
    }

    public void SetLanguageToHebrew()
    {
        if (_hebrewLocale != null)
        {
            LocalizationSettings.SelectedLocale = _hebrewLocale;
            Debug.Log($"Language changed to: {_hebrewLocale.LocaleName}");
        }
        else
        {
            Debug.LogError("LocalizationTest: Hebrew locale is not ready. Cannot switch language.");
        }
    }

    public void SetLanguageToEnglish()
    {
        if (_englishLocale != null)
        {
            LocalizationSettings.SelectedLocale = _englishLocale;
            Debug.Log($"Language changed to: {_englishLocale.LocaleName}");
        }
        else
        {
            Debug.LogError("LocalizationTest: English locale is not ready. Cannot switch language.");
        }
    }

    private IEnumerator TestTranslationAfterDelay(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);

        Debug.Log("========== TRANSLATION TEST STARTING ==========");

        var service = TranslationService.Instance;

        // 1. Test "welcome" (Translate method)
        string welcome = service.Translate("yahalom.welcome");
        Debug.Log($"Test for 'welcome' (Hebrew): Result = {welcome}");

        // 2. Test "test" (Translate method)
        string test = service.Translate("yahalom.test");
        Debug.Log($"Test for 'test' (Hebrew): Result = {test}");

        // 3. Test "play" (assuming it's in your local file)
        string play = service.Translate("yahalom.play");
        Debug.Log($"Test for 'play' (Hebrew): Result = {play}");

        // 4. Test "non_existent_key"
        string missing = service.Translate("yahalom.non_existent_key");
        Debug.Log($"Test for 'non_existent_key': Result = {missing} (Expected: non_existent_key)");

        // --- NEW TEST ---
        // 5. Test GetTranslationEntry
        Debug.Log("--- Testing GetTranslationEntry ---");
        var entry = service.GetTranslationEntry("yahalom.welcome");
        if (entry != null)
        {
            Debug.Log($"GetTranslationEntry('welcome'): Key={entry.key}, English={entry.english}, Hebrew={entry.hebrew}");
        }
        else
        {
            Debug.LogError("GetTranslationEntry('welcome'): Entry was null!");
        }

        var missingEntry = service.GetTranslationEntry("yahalom.non_existent_key");
        Debug.Log($"GetTranslationEntry('non_existent_key'): Result = {(missingEntry == null ? "null (as expected)" : "found (unexpected)")}");
        // --- END NEW TEST ---

        Debug.Log("========== TRANSLATION TEST COMPLETE ==========");
    }
}
