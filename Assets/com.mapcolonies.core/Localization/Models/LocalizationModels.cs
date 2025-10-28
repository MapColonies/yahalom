using System.Collections.Generic;

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
        public string Key;
        public string English;
        public string Hebrew;
    }
}
