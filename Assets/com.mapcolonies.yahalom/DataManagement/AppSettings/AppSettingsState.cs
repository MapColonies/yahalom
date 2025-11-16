using System;

namespace com.mapcolonies.yahalom.DataManagement.AppSettings
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

        public string WorkspacesPath
        {
            get;
            set;
        }
    }
}
