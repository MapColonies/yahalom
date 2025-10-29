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
    public class TranslationServicePlayMode_MissingFileTests
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
        public IEnumerator Missing_File_Does_Not_Throw_And_Unknown_Key_Passthrough()
        {
            var svc = new TranslationService();
            var initTask = svc.InitializeService("en");
            yield return new WaitUntil(() => initTask.IsCompleted);

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            Assert.AreEqual("unknown_key", svc.Translate("unknown_key"));
        }
    }
}
