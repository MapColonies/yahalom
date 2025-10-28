using com.mapcolonies.yahalom.Configuration;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Redux
{
    public static class RootReducer
    {
        public static AppState Reduce(AppState state, IAction action)
        {
            return state.With(ConfigurationReducer.Reduce(state.ConfigurationState, action));
        }
    }
}
