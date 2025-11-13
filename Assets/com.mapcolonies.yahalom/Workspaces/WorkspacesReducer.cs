using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Workspaces
{
    public class WorkspacesReducer
    {
        public const string SliceName = "workspaces";

        public static WorkspacesState Reduce(WorkspacesState state, IAction<WorkspacesState> action)
        {
            return action.type switch
            {
                WorkspacesActions.LoadAction => action.payload,
                _ => state
            };
        }
    }
}
