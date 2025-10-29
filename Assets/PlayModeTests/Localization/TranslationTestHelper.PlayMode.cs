using System.Collections;
using System.IO;
using System.Text;
using com.mapcolonies.core.Localization.Constants;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace PlayModeTests.Localization
{
    public static class TranslationTestHelper
    {
        public static string EnsureTranslationsDir()
        {
            string dir = Path.Combine(Application.streamingAssetsPath, LocalizationConstants.TranslationsFileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }

        public static IEnumerator EnsureLocalesAsync()
        {
            yield return LocalizationSettings.InitializationOperation;

            var available = LocalizationSettings.AvailableLocales;

            var en = available.GetLocale(LocalizationConstants.EnglishLocaleIdentifier);

            if (en == null)
            {
                en = Locale.CreateLocale(SystemLanguage.English);
                available.AddLocale(en);
            }

            var he = available.GetLocale(LocalizationConstants.HebrewLocaleIdentifier);

            if (he == null)
            {
                he = Locale.CreateLocale(LocalizationConstants.HebrewLocaleIdentifier);
                available.AddLocale(he);
            }

            LocalizationSettings.SelectedLocale = en;
        }

        public static void WriteJson(string path, string json)
        {
            File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}
