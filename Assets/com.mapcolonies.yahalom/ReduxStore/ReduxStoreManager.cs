using com.mapcolonies.yahalom.Configuration;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public class ReduxStoreManager
    {
        #region Slices Definition
            public const string ConfigurationSlice = "configuration";
        #endregion

        #region Actions Definitions
            public const string SetConfigurationAction = "configuration/SetConfigurationAction";
        #endregion

        public IStore<PartitionedState> Store
        {
            get;
            private set;
        }

        public UniTask Create()
        {
            Slice<ConfigurationState, PartitionedState> slice = StoreFactory.CreateSlice(ConfigurationSlice, new ConfigurationState(), (configurationSliceBuilder) =>
            {
                configurationSliceBuilder.AddCase(new ActionCreator<ConfigurationState>(SetConfigurationAction), ConfigurationReducer.Reduce);
            });

            Store = StoreFactory.CreateStore(new[] { slice });
            return UniTask.CompletedTask;
        }
    }
}
