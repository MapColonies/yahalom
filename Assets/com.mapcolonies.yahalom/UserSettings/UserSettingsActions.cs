using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.UserSettings
{
    public class UserSettingsActions
    {
        public const string LoadAction = "usersettings/load_usersettings_action";
        public const string SaveAction = "usersettings/save_usersettings_action";

        public static void AddActions(SliceReducerSwitchBuilder<UserSettingsState> builder)
        {
            builder.AddCase(LoadUserSettingsActionCreator(), UserSettingsReducer.Reduce);
        }

        public static IAction<UserSettingsState> LoadUserSettingsAction(UserSettingsState payload)
        {
            return new ActionCreator<UserSettingsState>(LoadAction).Invoke(payload);
        }

        public static IAction<UserSettingsState> SaveUserSettingsAction(UserSettingsState payload)
        {
            return new ActionCreator<UserSettingsState>(SaveAction).Invoke(payload);
        }

        public static IActionCreator<UserSettingsState> LoadUserSettingsActionCreator()
        {
            return new ActionCreator<UserSettingsState>(LoadAction);
        }
    }
}
