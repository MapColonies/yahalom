using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.AppSettings
{
    public static class AppSettingsActions
    {
        public const string LoadAction = "appsettings/load_appsettings_action";

        #region Actions

        public static IAction<AppSettingsState> LoadAppSettingsAction(AppSettingsState payload)
        {
            return new ActionCreator<AppSettingsState>(LoadAction).Invoke(payload);
        }

        #endregion

        #region Creators

        public static IActionCreator<AppSettingsState> LoadAppSettingsActionCreator()
        {
            return new ActionCreator<AppSettingsState>(LoadAction);
        }

        #endregion
    }
}
