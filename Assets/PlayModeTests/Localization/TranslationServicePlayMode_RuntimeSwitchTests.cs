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

namespace PlayModeTests.Localization
{
    public class TranslationServicePlayMode_RuntimeSwitchTests
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
            var initTask = svc.InitializeService("en");
            yield return new WaitUntil(() => initTask.IsCompleted);

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            Assert.AreEqual("Start", svc.Translate("start"));
            Assert.AreEqual("Exit",  svc.Translate("exit"));

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("he-IL");
            Assert.AreEqual("התחלה", svc.Translate("start"));
            Assert.AreEqual("יציאה",  svc.Translate("exit"));
        }
    }
}
