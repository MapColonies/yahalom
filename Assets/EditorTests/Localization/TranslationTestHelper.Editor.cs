using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using com.mapcolonies.core.Localization;
using com.mapcolonies.core.Localization.Constants;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace EditorTests.Localization
{
    public static class TranslationTestHelper
    {
        public const string TestTranslationsDirectory = "TEST_Translations";

        private static string _originalPath;
        private static FieldInfo _pathField;

        public static string EnsureTranslationsDir()
        {
            string dir = Path.Combine(Application.streamingAssetsPath, TestTranslationsDirectory);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }

        public static void TearDownTranslationsDir()
        {
            if (_pathField != null && _originalPath != null)
            {
                _pathField.SetValue(null, _originalPath);
            }

            string dir = Path.Combine(Application.streamingAssetsPath, TestTranslationsDirectory);

            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }

        public static void SetTestFilePath(string testDirectoryName)
        {
            string testPath = Path.ChangeExtension(
                Path.Combine(testDirectoryName, LocalizationConstants.TranslationsFileName),
                ".json"
            );

            _pathField = typeof(TranslationService).GetField(
                "LocalFilePath",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            if (_pathField != null)
            {
                _originalPath = (string)_pathField.GetValue(null);
                _pathField.SetValue(null, testPath);
            }
            else
            {
                throw new Exception("Reflection failed: Could not find private static field 'LocalFilePath' in TranslationService.");
            }
        }

        public static IEnumerator EnsureLocalesAsync()
        {
            yield return LocalizationSettings.InitializationOperation;

            ILocalesProvider available = LocalizationSettings.AvailableLocales;

            Locale en = available.GetLocale(LocalizationConstants.EnglishLocaleIdentifier);

            if (en == null)
            {
                en = Locale.CreateLocale(SystemLanguage.English);
                available.AddLocale(en);
            }

            Locale he = available.GetLocale(LocalizationConstants.HebrewLocaleIdentifier);

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
