using System.Collections.Generic;
using Newtonsoft.Json;

namespace com.mapcolonies.core.Localization.Models
{
    [System.Serializable]
    public class TranslationConfig
    {
        public bool ShowTranslationWarnings;
        public List<TranslationEntry> Words = new List<TranslationEntry>();
    }

    [System.Serializable]
    public class TranslationEntry
    {
        [JsonProperty("key")] public string Key;

        [JsonProperty("localizedValues")] public Dictionary<string, string> LocalizedValues = new Dictionary<string, string>();
    }
}
