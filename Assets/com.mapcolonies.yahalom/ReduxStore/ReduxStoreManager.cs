using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.Configuration;
using com.mapcolonies.yahalom.UserSettings;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;
using VContainer;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public class ReduxStoreManager : IReduxStoreManager
    {
        public IStore<PartitionedState> Store
        {
            get;
            private set;
        }

        public EpicMiddlewareCreator Epics
        {
            get;
            private set;
        }

        public UniTask Create(IObjectResolver resolver)
        {
            Slice<ConfigurationState, PartitionedState> configurationSlice = StoreFactory.CreateSlice(ConfigurationReducer.SliceName, new ConfigurationState(), ConfigurationActions.AddActions);
            Slice<AppSettingsState, PartitionedState> appSettingsSlice = StoreFactory.CreateSlice(AppSettingsReducer.SliceName, new AppSettingsState(), AppSettingsActions.AddActions);
            Slice<UserSettingsState, PartitionedState> userSettingsSlice = StoreFactory.CreateSlice(UserSettingsReducer.SliceName, new UserSettingsState(), UserSettingsActions.AddActions);

            Epics = resolver.Resolve<EpicMiddlewareCreator>();
            Middleware<PartitionedState> epicMiddleware = Epics.CreateMiddleware();

            Store = StoreFactory.CreateStore(
                new ISlice<PartitionedState>[]
                {
                    configurationSlice,
                    appSettingsSlice,
                    userSettingsSlice
                },
                StoreFactory.ApplyMiddleware(epicMiddleware));

            return UniTask.CompletedTask;
        }
    }
}
