using System;
using com.mapcolonies.yahalom.ReduxStore;
using R3;

namespace com.mapcolonies.yahalom.DataManagement
{
    public class BaseDataManager : IDisposable
    {
        protected IReduxStoreManager ReduxStoreManager;
        protected readonly CompositeDisposable Disposables = new CompositeDisposable();

        public BaseDataManager(IReduxStoreManager reduxStoreManager)
        {
            ReduxStoreManager = reduxStoreManager;
        }

        public void Dispose()
        {
            Disposables?.Dispose();
        }
    }
}
