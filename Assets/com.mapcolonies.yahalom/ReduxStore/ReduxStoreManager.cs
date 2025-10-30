using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.Configuration;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public class ReduxStoreManager
    {
        #region Slices Definition

        public const string ConfigurationSlice = "configuration";
        public const string AppSettingsSlice = "appSettings";

        #endregion

        #region Actions Definitions

        public const string SetConfigurationAction = "configuration/SetConfigurationAction";
        public const string SetAppSettingsAction = "appSettings/SetAppSettingsAction";

        #endregion

        public IStore<PartitionedState> Store
        {
            get;
            private set;
        }

        public UniTask Create()
        {
            Slice<ConfigurationState, PartitionedState> configurationSlice = StoreFactory.CreateSlice(ConfigurationSlice, new ConfigurationState(), (configurationSliceBuilder) =>
            {
                configurationSliceBuilder.AddCase(new ActionCreator<ConfigurationState>(SetConfigurationAction), ConfigurationReducer.Reduce);
            });

            Slice<AppSettingsState, PartitionedState> appSettingsSlice = StoreFactory.CreateSlice(AppSettingsSlice, new AppSettingsState(), (appSettingsSliceBuilder) =>
            {
                appSettingsSliceBuilder.AddCase(new ActionCreator<AppSettingsState>(SetAppSettingsAction), AppSettingsReducer.Reduce);
            });

            Store = StoreFactory.CreateStore(new ISlice<PartitionedState>[] { configurationSlice, appSettingsSlice });
            return UniTask.CompletedTask;
        }
    }
}
