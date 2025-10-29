using System.Collections;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.TestTools;
using com.mapcolonies.core.Localization;

using Tests.Localization.Helpers;

namespace EditorTests.Localization
{
    public class TranslationServiceEditor_DuplicateKeysTests
    {
        private string _jsonPath;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            string dir = TranslationTestHelper.EnsureTranslationsDir();
            _jsonPath = Path.Combine(dir, "Yahalom_HardCoded_Translations.json");
            if (File.Exists(_jsonPath)) File.Delete(_jsonPath);
            yield return TranslationTestHelper.EnsureLocalesAsync();
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            if (File.Exists(_jsonPath)) File.Delete(_jsonPath);
            yield return null;
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
            var initTask = svc.InitializeService("en");
            yield return new WaitUntil(() => initTask.IsCompleted);

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            Assert.AreEqual("App Title", svc.Translate("title"));

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("he-IL");
            Assert.AreEqual("כותרת האפליקציה", svc.Translate("title"));
        }
    }
}
