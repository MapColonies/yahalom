using System.Collections;
using com.mapcolonies.core.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationTest : MonoBehaviour
{
    public TranslationService translationService;
    public string hebrewLocaleIdentifier;
    public string englishLocaleIdentifier;
    public string localid;

    private Locale _hebrewLocale;
    private Locale _englishLocale;

    void Start()
    {
        var bb = new TranslationService();
        bb.InitializeService(localid);
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
}
