using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.TestTools;
using com.mapcolonies.core.Localization;

namespace EditorTests.Localization
{
    public class TranslationServiceEditorFileLoadingAndSwitchTests
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
            var initTask = svc.InitializeService("en");
            yield return new WaitUntil(() => initTask.IsCompleted);

            // EN
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            Assert.AreEqual("hello", svc.Translate("hello"));
            Assert.AreEqual("home", svc.Translate("home"));

            // HE
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("he-IL");
            Assert.AreEqual("שלום", svc.Translate("hello"));
            Assert.AreEqual("בית", svc.Translate("home"));

            Assert.AreEqual("missing_key", svc.Translate("missing_key"));
        }
    }
}
