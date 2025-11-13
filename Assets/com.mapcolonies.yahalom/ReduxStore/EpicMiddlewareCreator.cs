using System;
using R3;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public sealed class EpicMiddlewareCreator : IDisposable
    {
        private readonly Subject<IAction> _actionStream = new Subject<IAction>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public Middleware<PartitionedState> CreateMiddleware()
        {
            return store => next => action =>
            {
                next(action);
                _actionStream.OnNext(action);
            };
        }

        public IDisposable RunEpic(
            Func<Observable<IAction>, Observable<IAction>> epic,
            IStore<PartitionedState> store)
        {
            return epic(_actionStream)
                .Subscribe(store.Dispatch)
                .AddTo(_disposables);
        }

        public IDisposable RunEpic(
            Func<Observable<IAction>, IStore<PartitionedState>, Observable<IAction>> epic,
            IStore<PartitionedState> store)
        {
            return epic(_actionStream, store)
                .Subscribe(store.Dispatch).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _actionStream.OnCompleted();
        }
    }
}
