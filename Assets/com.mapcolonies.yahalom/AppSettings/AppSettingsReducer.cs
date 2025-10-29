using com.mapcolonies.yahalom.ReduxStore;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.AppSettings
{
    public static class AppSettingsReducer
    {
        public static AppSettingsState Reduce(AppSettingsState state, IAction<AppSettingsState> action)
        {
            return action.type switch
            {
                ReduxStoreManager.SetAppSettingsAction => action.payload,
                _ => state
            };
        }
    }
}
