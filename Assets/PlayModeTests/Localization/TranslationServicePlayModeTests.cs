using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.TestTools;
using com.mapcolonies.core.Localization;
using com.mapcolonies.core.Localization.Constants;

namespace PlayModeTests.Localization
{
    public class TranslationServicePlayModeTests
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
        public IEnumerator Missing_File_Does_Not_Throw_And_Unknown_Key_Passthrough()
        {
            var svc = new TranslationService();

            try
            {
                var initTask = svc.InitializeService(LocalizationConstants.EnglishLocaleIdentifier);
                yield return new WaitUntil(() => initTask.IsCompleted);

                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.EnglishLocaleIdentifier);

                Assert.AreEqual("unknown_key", svc.Translate("unknown_key"));
            }
            finally
            {
                svc.Dispose();
                LocalizationSettings.SelectedLocale = null;
            }
        }

        [UnityTest]
        public IEnumerator Runtime_Language_Switch_Reflects_In_Translate()
        {
            string json = @"
{
  ""ShowTranslationWarnings"": true,
  ""Words"": [
    { ""Key"": ""start"", ""English"": ""Start"", ""Hebrew"": ""התחלה"" },
    { ""Key"": ""exit"",  ""English"": ""Exit"",  ""Hebrew"": ""יציאה"" }
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
                Assert.AreEqual("Start", svc.Translate("start"));
                Assert.AreEqual("Exit", svc.Translate("exit"));

                LocalizationSettings.SelectedLocale =
                    LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.HebrewLocaleIdentifier);
                Assert.AreEqual("התחלה", svc.Translate("start"));
                Assert.AreEqual("יציאה", svc.Translate("exit"));
            }
            finally
            {
                svc.Dispose();
                LocalizationSettings.SelectedLocale = null;
            }
        }
    }
}
