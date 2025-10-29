using System.Collections.Generic;
using com.mapcolonies.core.Localization.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace EditorTests.Localization.SerializationTests
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
                    new TranslationEntry { Key = "hello", English = "Hello", Hebrew = "שלום" }
                }
            };

            var json = JsonConvert.SerializeObject(cfg);
            var back = JsonConvert.DeserializeObject<TranslationConfig>(json);

            Assert.NotNull(back);
            Assert.True(back.ShowTranslationWarnings);
            Assert.AreEqual(1, back.Words.Count);
            Assert.AreEqual("hello", back.Words[0].Key);
            Assert.AreEqual("שלום", back.Words[0].Hebrew);
        }

        [Test]
        public void Deserialize_Works_With_CamelCase_Keys()
        {
            var json = @"{
  ""showTranslationWarnings"": false,
  ""words"": [
    { ""key"": ""start"", ""english"": ""Start"", ""hebrew"": ""התחלה"" }
  ]
}";
            var cfg = JsonConvert.DeserializeObject<TranslationConfig>(json);

            Assert.NotNull(cfg);
            Assert.False(cfg.ShowTranslationWarnings);
            Assert.AreEqual("start", cfg.Words[0].Key);
            Assert.AreEqual("Start", cfg.Words[0].English);
            Assert.AreEqual("התחלה", cfg.Words[0].Hebrew);
        }
    }
}
