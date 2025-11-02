using System;

namespace com.mapcolonies.yahalom.AppSettings
{
    [Serializable]
    public record AppSettingsState()
    {
        public string ConfigurationPath
        {
            get;
            set;
        }

        public string UserSettingsPath
        {
            get;
            set;
        }

        public bool OfflineMode
        {
            get;
            set;
        }
    }
}
