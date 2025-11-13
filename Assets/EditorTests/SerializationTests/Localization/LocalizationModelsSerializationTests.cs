using System.Collections.Generic;
using com.mapcolonies.core.Localization.Constants; // Added
using com.mapcolonies.core.Localization.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace EditorTests.SerializationTests.Localization
{
    public class SimpleLocalizationModelsSerializationTests
    {
        [Test]
        public void Serialize_And_Deserialize_RoundTrip_Works()
        {
            var cfg = new TranslationConfig
            {
                ShowTranslationWarnings = true,
                Words = new List<TranslationEntry>
                {
                    new TranslationEntry
                    {
                        Key = "hello",
                        LocalizedValues = new Dictionary<string, string>
                        {
                            { LocalizationConstants.EnglishLocaleIdentifier, "Hello" },
                            { LocalizationConstants.HebrewLocaleIdentifier, "שלום" }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
            var back = JsonConvert.DeserializeObject<TranslationConfig>(json);

            Assert.NotNull(back);
            Assert.True(back.ShowTranslationWarnings);
            Assert.AreEqual(1, back.Words.Count);
            Assert.AreEqual("hello", back.Words[0].Key);
            Assert.AreEqual("שלום", back.Words[0].LocalizedValues[LocalizationConstants.HebrewLocaleIdentifier]);
            Assert.AreEqual("Hello", back.Words[0].LocalizedValues[LocalizationConstants.EnglishLocaleIdentifier]);
        }

        [Test]
        public void Deserialize_Works_With_CamelCase_Keys()
        {
            // Updated JSON to use the new 'localizedValues' object
            var json = $@"{{
  ""showTranslationWarnings"": false,
  ""words"": [
    {{
      ""key"": ""start"",
      ""localizedValues"": {{
        ""{LocalizationConstants.EnglishLocaleIdentifier}"": ""Start"",
        ""{LocalizationConstants.HebrewLocaleIdentifier}"": ""התחלה""
      }}
    }}
  ]
}}";
            var cfg = JsonConvert.DeserializeObject<TranslationConfig>(json);

            Assert.NotNull(cfg);
            Assert.False(cfg.ShowTranslationWarnings);
            Assert.AreEqual("start", cfg.Words[0].Key);
            Assert.AreEqual("Start", cfg.Words[0].LocalizedValues[LocalizationConstants.EnglishLocaleIdentifier]);
            Assert.AreEqual("התחלה", cfg.Words[0].LocalizedValues[LocalizationConstants.HebrewLocaleIdentifier]);
        }
    }
}
