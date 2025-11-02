using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.Configuration;
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
            Slice<ConfigurationState, PartitionedState> configurationSlice = StoreFactory.CreateSlice(ConfigurationReducer.SliceName, new ConfigurationState(), (configurationSliceBuilder) =>
            {
                configurationSliceBuilder.AddCase(ConfigurationActions.LoadConfigurationActionCreator(), ConfigurationReducer.Reduce);
            });

            Slice<AppSettingsState, PartitionedState> appSettingsSlice = StoreFactory.CreateSlice(AppSettingsReducer.SliceName, new AppSettingsState(), (appSettingsSliceBuilder) =>
            {
                appSettingsSliceBuilder.AddCase(AppSettingsActions.LoadAppSettingsActionCreator(), AppSettingsReducer.Reduce);
            });

            Store = StoreFactory.CreateStore(new ISlice<PartitionedState>[] { configurationSlice, appSettingsSlice });
            return UniTask.CompletedTask;
        }
    }
}
