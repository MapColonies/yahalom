using System.Collections;
using System.IO;
using com.mapcolonies.core.Localization;
using com.mapcolonies.core.Localization.Constants;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.Localization.Settings;
using UnityEngine.TestTools;

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
            TranslationTestHelper.TearDownTranslationsDir();

            yield return null;
        }

        [UnityTest]
        public IEnumerator Missing_File_Does_Not_Throw_And_Unknown_Key_Passthrough()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TranslationTestHelper.SetTestFilePath(TranslationTestHelper.TestTranslationsDirectory);
                TranslationService svc = new TranslationService();

                try
                {
                    await svc.InitializeService(LocalizationConstants.EnglishLocaleIdentifier);
                    LocalizationSettings.SelectedLocale =
                        LocalizationSettings.AvailableLocales.GetLocale(LocalizationConstants.EnglishLocaleIdentifier);

                    Assert.AreEqual("unknown_key", svc.Translate("unknown_key"));
                }
                finally
                {
                    svc.Dispose();
                    LocalizationSettings.SelectedLocale = null;
                }
            });
        }

        [UnityTest]
        public IEnumerator Runtime_Language_Switch_Reflects_In_Translate()
        {
            return UniTask.ToCoroutine(async () =>
            {
                string json = $@"
{{
  ""ShowTranslationWarnings"": true,
  ""Words"": [
    {{
      ""Key"": ""start"",
      ""localizedValues"": {{
        ""{LocalizationConstants.EnglishLocaleIdentifier}"": ""Start"",
        ""{LocalizationConstants.HebrewLocaleIdentifier}"": ""התחלה""
      }}
    }},
    {{
      ""Key"": ""exit"",
      ""localizedValues"": {{
        ""{LocalizationConstants.EnglishLocaleIdentifier}"": ""Exit"",
        ""{LocalizationConstants.HebrewLocaleIdentifier}"": ""יציאה""
      }}
    }}
  ]
}}";
                TranslationTestHelper.WriteJson(_jsonPath, json);

                TranslationTestHelper.SetTestFilePath(TranslationTestHelper.TestTranslationsDirectory);
                TranslationService svc = new TranslationService();

                try
                {
                    await svc.InitializeService(LocalizationConstants.EnglishLocaleIdentifier);

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
            });
        }
    }
}
