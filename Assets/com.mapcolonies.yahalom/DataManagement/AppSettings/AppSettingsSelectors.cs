using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.DataManagement.AppSettings
{
    public class AppSettingsSelectors
    {
        public static readonly Selector<AppSettingsState, string> UserSettingsPath = (state) => state.UserSettingsPath;
        public static readonly Selector<AppSettingsState, string> OfflineConfigurationPathSelector = (state) => state.OfflineConfigurationPath;
        public static readonly Selector<AppSettingsState, string> RemoteConfigurationUrlSelector = (state) => state.RemoteConfigurationUrl;
        public static readonly Selector<AppSettingsState, string> WorkspacesPathSelector = (state) => state.WorkspacesPath;
    }
}
