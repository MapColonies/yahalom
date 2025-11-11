using System;
using System.Linq;
using R3;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public static class EpicMiddleware
    {
        public static EpicMiddleware<TState, NoDependencies> Default<TState>()
        {
            return new EpicMiddleware<TState, NoDependencies>();
        }

        public static EpicMiddleware<TState, TDependencies> Default<TState, TDependencies>(TDependencies dependencies)
        {
            return new EpicMiddleware<TState, TDependencies>(dependencies);
        }
    }

    public class EpicMiddleware<TState, TDependencies> : IDisposable
    {
        private IReduxStoreManager _store;
        private readonly Subject<IAction> _actionSubject = new Subject<IAction>();
        private readonly Subject<Epic<TState, TDependencies>> _epicSubject = new Subject<Epic<TState, TDependencies>>();
        private readonly TDependencies _dependencies;
        private DisposableBag _disposables;

        private ReactiveProperty<TState> _stateProperty = new ReactiveProperty<TState>();

        internal EpicMiddleware()
        {
        }

        internal EpicMiddleware(TDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public MiddlewareDelegate Create()
        {
            return store =>
            {
                if (_store != null) throw new Exception("EpicMiddleware can only be used with one store.");

                _store = store;

                TState initialState = (TState) store.GetState().First(x => x.Value is TState).Value;
                _stateProperty = new ReactiveProperty<TState>(initialState);
                _stateProperty.AddTo(ref _disposables);

                ReadOnlyReactiveProperty<TState> readOnlyStateProperty = _stateProperty;

                _epicSubject.Select(this, (epic, self) =>
                {
                    Observable<IAction> output = epic(_actionSubject, readOnlyStateProperty, self._dependencies);
                    if (output == null) throw new Exception("Epic must return an observable.");
                    return output;
                })
                .Merge()
                .Subscribe(this, static (action, self) =>
                {
                    self._store.Dispatch(action);
                })
                .AddTo(ref _disposables);
                return next => async (action, token) =>
                {
                    await next(action, token);
                    TState state = (TState) store.GetState().First(x => x.Value is TState).Value;

                    _stateProperty.OnNext(state);
                    _actionSubject.OnNext(action);
                };
            };
        }

        public void Run(Epic<TState, TDependencies> root)
        {
            _epicSubject.OnNext(root);
        }

        public void Run(Epic<TState> root)
        {
            _epicSubject.OnNext((action, state, _) => root(action, state));
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _actionSubject?.Dispose();
        }
    }
}
