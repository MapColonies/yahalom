using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Workspaces
{
    public class WorkspacesActions
    {
        public const string LoadAction = "workspaces/load_workspaces_action";

        public static void AddActions(SliceReducerSwitchBuilder<WorkspacesState> builder)
        {
            builder.AddCase(LoadWorkspacesActionCreator(), WorkspacesReducer.Reduce);
        }

        #region Actions

        public static IAction<WorkspacesState> LoadWorkspacesAction(WorkspacesState payload)
        {
            return new ActionCreator<WorkspacesState>(LoadAction).Invoke(payload);
        }

        #endregion

        #region Creators

        public static IActionCreator<WorkspacesState> LoadWorkspacesActionCreator()
        {
            return new ActionCreator<WorkspacesState>(LoadAction);
        }

        #endregion
    }
}
