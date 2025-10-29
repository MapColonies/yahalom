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
    public class TranslationServiceEditor_InitAndPassthroughTests
    {
        [Test]
        public void Translate_Before_Initialize_Returns_Key()
        {
            var svc = new TranslationService();
            // Deliberately do not initialize
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
