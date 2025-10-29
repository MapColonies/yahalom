using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace PlayModeTests.Localization
{
    public static class TranslationTestHelper
    {
        public static string EnsureTranslationsDir()
        {
            string dir = Path.Combine(Application.streamingAssetsPath, "Translations");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }

        public static IEnumerator EnsureLocalesAsync()
        {
            yield return LocalizationSettings.InitializationOperation;

            var available = LocalizationSettings.AvailableLocales;

            var en = available.GetLocale("en");

            if (en == null)
            {
                en = Locale.CreateLocale(SystemLanguage.English);
                available.AddLocale(en);
            }

            var he = available.GetLocale("he-IL");

            if (he == null)
            {
                he = Locale.CreateLocale("he-IL");
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
