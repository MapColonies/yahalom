using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.DataManagement.Workspaces
{
    public class WorkspacesActions
    {
        public const string LoadAction = "workspaces/load_workspaces_action";
        public const string AddWorkspace = "workspaces/add_workspace_action";

        public static void AddActions(SliceReducerSwitchBuilder<WorkspacesState> builder)
        {
            builder.AddCase(LoadWorkspacesActionCreator(), WorkspacesReducer.Reduce);
            builder.AddCase(LoadWorkspacesActionCreator(), WorkspacesReducer.Reduce);
        }

        #region Actions

        public static IAction<WorkspacesState> LoadWorkspacesAction(WorkspacesState payload)
        {
            return new ActionCreator<WorkspacesState>(LoadAction).Invoke(payload);
        }

        public static IAction<WorkspacesState> AddWorkspaceAction(WorkspacesState payload)
        {
            return new ActionCreator<WorkspacesState>(AddWorkspace).Invoke(payload);
        }

        #endregion

        #region Creators

        public static IActionCreator<WorkspacesState> LoadWorkspacesActionCreator()
        {
            return new ActionCreator<WorkspacesState>(LoadAction);
        }

        public static IActionCreator<WorkspacesState> AddWorkspaceActionCreator()
        {
            return new ActionCreator<WorkspacesState>(AddWorkspace);
        }
        #endregion
    }
}
