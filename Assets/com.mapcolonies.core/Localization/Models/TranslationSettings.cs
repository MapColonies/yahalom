using System;
using com.mapcolonies.core.Localization.Constants;

namespace com.mapcolonies.core.Localization.Models
{
    [Serializable]
    public record TranslationSettings
    {
        public string Locale
        {
            get;
            set;
        } = LocalizationConstants.HebrewLocaleIdentifier;

        public string LocalFilePath
        {
            get;
            set;
        }

        public string RemoteConfigUrl
        {
            get;
            set;
        }

        public bool ShowTranslationWarnings
        {
            get;
            set;
        }
    }
}
