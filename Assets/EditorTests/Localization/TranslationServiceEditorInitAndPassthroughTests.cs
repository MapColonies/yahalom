using NUnit.Framework;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using com.mapcolonies.core.Localization;

namespace EditorTests.Localization
{
    public class TranslationServiceEditorInitAndPassthroughTests
    {
        [Test]
        public void TranslateBeforeInitializeReturnsKey()
        {
            var svc = new TranslationService();
            var en = LocalizationSettings.AvailableLocales.GetLocale("en") ?? Locale.CreateLocale(SystemLanguage.English);
            if (LocalizationSettings.AvailableLocales.GetLocale("en") == null)
                LocalizationSettings.AvailableLocales.AddLocale(en);
            LocalizationSettings.SelectedLocale = en;

            string key = "hello";
            string result = svc.Translate(key);

            Assert.AreEqual(key, result, "Translate before InitializeService should return the key.");
        }
    }
}
