using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.Configuration;
using com.mapcolonies.yahalom.UserSettings;
using com.mapcolonies.yahalom.Workspaces;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public class ReduxStoreManager : IReduxStoreManager
    {
        public IStore<PartitionedState> Store
        {
            get;
            private set;
        }

        public UniTask Create()
        {
            Slice<ConfigurationState, PartitionedState> configurationSlice = StoreFactory.CreateSlice(ConfigurationReducer.SliceName, new ConfigurationState(), ConfigurationActions.AddActions);
            Slice<AppSettingsState, PartitionedState> appSettingsSlice = StoreFactory.CreateSlice(AppSettingsReducer.SliceName, new AppSettingsState(), AppSettingsActions.AddActions);
            Slice<UserSettingsState, PartitionedState> userSettingsSlice = StoreFactory.CreateSlice(UserSettingsReducer.SliceName, new UserSettingsState(), UserSettingsActions.AddActions);
            Slice<WorkspacesState, PartitionedState> workspacesSlice = StoreFactory.CreateSlice(WorkspacesReducer.SliceName, new WorkspacesState(), WorkspacesActions.AddActions);

            Store = StoreFactory.CreateStore(new ISlice<PartitionedState>[]
            {
                configurationSlice,
                appSettingsSlice,
                userSettingsSlice,
                workspacesSlice
            });
            return UniTask.CompletedTask;
        }
    }
}
