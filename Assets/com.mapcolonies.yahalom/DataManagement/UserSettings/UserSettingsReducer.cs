using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.DataManagement.UserSettings
{
    public class UserSettingsReducer
    {
        public const string SliceName = "usersettings";

        public static UserSettingsState Reduce(UserSettingsState state, IAction<UserSettingsState> action)
        {
            return action.type switch
            {
                UserSettingsActions.LoadAction => action.payload,
                _ => state
            };
        }
    }
}
