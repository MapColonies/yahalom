using System;

namespace com.mapcolonies.yahalom.AppSettings
{
    [Serializable]
    public record AppSettingsState()
    {
        public string OfflineConfigurationPath
        {
            get;
            set;
        }

        public string RemoteConfigurationUrl
        {
            get;
            set;
        }

        public string UserSettingsPath
        {
            get;
            set;
        }
    }
}
