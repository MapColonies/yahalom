using System;

namespace com.mapcolonies.yahalom.DataManagement.AppSettings
{
    [Serializable]
    public record AppSettingsState()
    {
        public string OfflineConfigurationFile
        {
            get;
            set;
        }

        public string RemoteConfigurationUrl
        {
            get;
            set;
        }

        public string UserSettingsFile
        {
            get;
            set;
        }

        public string WorkspacesDirectory
        {
            get;
            set;
        }
    }
}
