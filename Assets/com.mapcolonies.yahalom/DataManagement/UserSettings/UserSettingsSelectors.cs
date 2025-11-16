using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.DataManagement.UserSettings
{
    public class UserSettingsSelectors
    {
        public static readonly Selector<UserSettingsState, bool> OfflineSelector = (state) => state.Offline;
    }
}
