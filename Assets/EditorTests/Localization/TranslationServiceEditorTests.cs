using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.TestTools;
using com.mapcolonies.core.Localization;
using com.mapcolonies.core.Localization.Constants;

namespace EditorTests.Localization
{
    public class TranslationServiceEditorTests
    {
        private string _jsonPath;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            string dir = TranslationTestHelper.EnsureTranslationsDir();
            _jsonPath = Path.Combine(dir, $"{LocalizationConstants.TranslationsFileName}.json");
            if (File.Exists(_jsonPath)) File.Delete(_jsonPath);
            yield return TranslationTestHelper.EnsureLocalesAsync();
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            LocalizationSettings.SelectedLocale = null;
            if (File.Exists(_jsonPath)) File.Delete(_jsonPath);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Initialize_From_File_And_Translate_English_And_Hebrew()
        {
            string json = @"
{
  ""ShowTranslationWarnings"": true,
  ""Words"": [
    { ""Key"": ""hello"", ""English"": ""hello"", ""Hebrew"": ""שלום"" },
    { ""Key"": ""home"",  ""English"": ""home"",  ""Hebrew"": ""בית"" }
  ]
}";
            TranslationTestHelper.WriteJson(_jsonPath, json);

            var svc = new TranslationService();

            try
            {
                var initTask = svc.InitializeService(LocalizationConstants.EnglishLocaleIdentifier);
                yield return new WaitUntil(() => initTask.IsCompleted);

                LocalizationSettings.SelectedLocale =
                    LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.EnglishLocaleIdentifier);
                Assert.AreEqual("hello", svc.Translate("hello"));
                Assert.AreEqual("home", svc.Translate("home"));

                LocalizationSettings.SelectedLocale =
                    LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.HebrewLocaleIdentifier);
                Assert.AreEqual("שלום", svc.Translate("hello"));
                Assert.AreEqual("בית", svc.Translate("home"));

                Assert.AreEqual("missing_key", svc.Translate("missing_key"));
            }
            finally
            {
                svc.Dispose();
                LocalizationSettings.SelectedLocale = null;
            }
        }

        [UnityTest]
        public IEnumerator Duplicate_Keys_Prefer_Last_Entry()
        {
            string json = @"
{
  ""ShowTranslationWarnings"": false,
  ""Words"": [
    { ""Key"": ""title"", ""English"": ""Title"",      ""Hebrew"": ""כותרת"" },
    { ""Key"": ""title"", ""English"": ""App Title"",  ""Hebrew"": ""כותרת האפליקציה"" }
  ]
}";
            TranslationTestHelper.WriteJson(_jsonPath, json);

            var svc = new TranslationService();

            try
            {
                var initTask = svc.InitializeService(LocalizationConstants.EnglishLocaleIdentifier);
                yield return new WaitUntil(() => initTask.IsCompleted);

                LocalizationSettings.SelectedLocale =
                    LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.EnglishLocaleIdentifier);
                Assert.AreEqual("App Title", svc.Translate("title"));

                LocalizationSettings.SelectedLocale =
                    LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.HebrewLocaleIdentifier);
                Assert.AreEqual("כותרת האפליקציה", svc.Translate("title"));
            }
            finally
            {
                svc.Dispose();
                LocalizationSettings.SelectedLocale = null;
            }
        }

        [Test]
        public void TranslateBeforeInitializeReturnsKey()
        {
            var svc = new TranslationService();

            try
            {
                var en = LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.EnglishLocaleIdentifier)
                         ?? Locale.CreateLocale(SystemLanguage.English);

                if (LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.EnglishLocaleIdentifier) == null)
                    LocalizationSettings.AvailableLocales.AddLocale(en);

                LocalizationSettings.SelectedLocale = en;

                string key = "hello";
                string result = svc.Translate(key);

                Assert.AreEqual(key, result, "Translate before InitializeService should return the key.");
            }
            finally
            {
                svc.Dispose();
                LocalizationSettings.SelectedLocale = null;
            }
        }
    }
}
