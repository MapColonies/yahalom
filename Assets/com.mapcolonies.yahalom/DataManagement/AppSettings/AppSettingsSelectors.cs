using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.DataManagement.AppSettings
{
    public class AppSettingsSelectors
    {
        public static readonly Selector<AppSettingsState, string> UserSettingsPath = (state) => state.UserSettingsFile;
        public static readonly Selector<AppSettingsState, string> OfflineConfigurationPathSelector = (state) => state.OfflineConfigurationFile;
        public static readonly Selector<AppSettingsState, string> RemoteConfigurationUrlSelector = (state) => state.RemoteConfigurationUrl;
        public static readonly Selector<AppSettingsState, string> WorkspacesPathSelector = (state) => state.WorkspacesDirectory;
    }
}
