using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.AppSettings
{
    public static class AppSettingsReducer
    {
        public const string SliceName = "appsettings";

        public static AppSettingsState Reduce(AppSettingsState state, IAction<AppSettingsState> action)
        {
            return action.type switch
            {
                AppSettingsActions.LoadAction => action.payload,
                _ => state
            };
        }
    }
}
