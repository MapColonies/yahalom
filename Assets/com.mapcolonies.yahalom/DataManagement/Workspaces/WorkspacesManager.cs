using System;
using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.DataManagement.AppSettings;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using R3;
using Unity.AppUI.Redux;
using UnityEngine;

namespace com.mapcolonies.yahalom.DataManagement.Workspaces
{
    public class WorkspacesManager : BaseDataManager
    {
        private const string WorkspacesListFile = "workspaces.json";
        private bool _exists;
        private string _workspacesFullPath;

        public WorkspacesManager(IReduxStoreManager reduxStoreManager, ActionsMiddleware actionsMiddleware) : base(reduxStoreManager)
        {
            ReduxStoreManager.Store.Select<WorkspacesState>(WorkspacesReducer.SliceName)
                .DistinctUntilChanged()
                .Debounce(TimeSpan.FromSeconds(30))
                .Where(_ => !_exists)
                .SubscribeAwait(async (state, token) =>
                {
                    await JsonUtilityEx.SavePersistentJsonAsync(_workspacesFullPath, state);
                })
                .AddTo(Disposables);

            actionsMiddleware.Actions.OfActionType(WorkspacesActions.AddWorkspace)
                .Do(_ =>
                {
                    Debug.Log("Start adding workspaces");
                })
                .Subscribe()
                .AddTo(Disposables);
        }

        public async UniTask Load()
        {
            string workspacesPath = ReduxStoreManager.Store.GetState(AppSettingsReducer.SliceName, AppSettingsSelectors.WorkspacesPathSelector);
            _workspacesFullPath = FileIOUtility.GetFullPath(workspacesPath, WorkspacesListFile);

            _exists = await FileIOUtility.FileExistsAsync(_workspacesFullPath);

            WorkspacesState workspacesState;
            if (_exists)
            {
                workspacesState = await JsonUtilityEx.LoadPersistentJsonAsync<WorkspacesState>(_workspacesFullPath);
            }
            else
            {
                workspacesState = new WorkspacesState();
            }

            ReduxStoreManager.Store.Dispatch(WorkspacesActions.LoadWorkspacesAction(workspacesState));
            ReduxStoreManager.Store.Dispatch(WorkspacesActions.AddWorkspaceAction("MySuperWorkspace"));
        }
    }
}
