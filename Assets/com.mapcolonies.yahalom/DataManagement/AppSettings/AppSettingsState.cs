using System;
using com.mapcolonies.core.Services.LoggerService;
using com.mapcolonies.core.Localization.Models;

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

        public LoggerSettings LoggerSettings
        {
            get;
            set;
        } = new LoggerSettings();

        public TranslationSettings TranslationSettings
        {
            get;
            set;
        } = new TranslationSettings();
    }
}
