using System;
using System.Collections.Generic;
using System.Threading;
using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.Configuration;
using com.mapcolonies.yahalom.UserSettings;
using Cysharp.Threading.Tasks;
using R3;
using Unity.AppUI.Redux;
using VContainer;
using Action = Unity.AppUI.Redux.Action;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public delegate Func<DispatchDelegate, DispatchDelegate> MiddlewareDelegate(IReduxStoreManager store);
    public delegate UniTask DispatchDelegate(IAction action, CancellationToken token = default);

    public class ReduxStoreManager : IReduxStoreManager
    {
        private IStore<PartitionedState> _store;
        private SimpleListCore<Func<DispatchDelegate, DispatchDelegate>> _middlewares;
        private readonly SynchronizationContext _mainThreadContext = SynchronizationContext.Current;

        public UniTask Create()
        {
            Slice<ConfigurationState, PartitionedState> configurationSlice = StoreFactory.CreateSlice(ConfigurationReducer.SliceName, new ConfigurationState(), ConfigurationActions.AddActions);
            Slice<AppSettingsState, PartitionedState> appSettingsSlice = StoreFactory.CreateSlice(AppSettingsReducer.SliceName, new AppSettingsState(), AppSettingsActions.AddActions);
            Slice<UserSettingsState, PartitionedState> userSettingsSlice = StoreFactory.CreateSlice(UserSettingsReducer.SliceName, new UserSettingsState(), UserSettingsActions.AddActions);

            EpicMiddleware<ConfigurationState, NoDependencies> epicMiddleware1 = EpicMiddleware.Default<ConfigurationState>();


            _store = StoreFactory.CreateStore(
                new ISlice<PartitionedState>[]
                {
                    configurationSlice,
                    appSettingsSlice,
                    userSettingsSlice
                });

            AddMiddleware(epicMiddleware1.Create());

            return UniTask.CompletedTask;
        }

        public async UniTask DispatchAsync(IAction action, CancellationToken token = default)
        {
            DispatchDelegate next = (a, _) =>
            {
                UniTaskCompletionSource<bool> tcs = new UniTaskCompletionSource<bool>();

                _mainThreadContext.Post(_ =>
                {
                    try
                    {
                        _store.Dispatch(a);
                        tcs.TrySetResult(true);
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                }, null);

                return tcs.Task;
            };

            ReadOnlyMemory<Func<DispatchDelegate, DispatchDelegate>> middlewares = _middlewares.AsMemory();

            for (int index = middlewares.Length - 1; index >= 0; index--)
            {
                DispatchDelegate oldNext = next;
                Func<DispatchDelegate, DispatchDelegate> current = middlewares.Span[index];
                next = async (a, t) => await current(oldNext)(a, t);
            }

            await next(action, token);
        }

        public async UniTask DispatchAsync(string actionType, CancellationToken token = default)
        {
            await DispatchAsync(new ActionCreator(actionType).Invoke(), token);
        }

        public async UniTask DispatchAsync<T>(string actionType, T payload, CancellationToken token = default)
        {
            await DispatchAsync(new ActionCreator<T>(actionType).Invoke(payload), token);
        }

        public void Dispatch<T>(string actionType, T payload)
        {
            Dispatch(new ActionCreator<T>(actionType).Invoke(payload));
        }

        public void Dispatch(string actionType)
        {
            Dispatch(new ActionCreator(actionType).Invoke());
        }

        public void Dispatch(IAction action)
        {
            DispatchDelegate next = (a, _) =>
            {
                _store.Dispatch(a);
                return UniTask.CompletedTask;
            };

            ReadOnlyMemory<Func<DispatchDelegate, DispatchDelegate>> middlewares = _middlewares.AsMemory();

            for (int index = middlewares.Length - 1; index >= 0; index--)
            {
                DispatchDelegate oldNext = next;
                Func<DispatchDelegate, DispatchDelegate> current = middlewares.Span[index];

                next = (a, t) =>
                {
                    current(oldNext)(a);
                    return UniTask.CompletedTask;
                };
            }

            next(action);
        }

        public void Dispatch(Action action)
        {

        }

        private void AddMiddleware(MiddlewareDelegate middleware)
        {
            Func<DispatchDelegate, DispatchDelegate> setUpdMiddleware = middleware(this);
            _middlewares.Add(setUpdMiddleware);
        }

        public TSelected GetState<TSliceState, TSelected>(string sliceName, Selector<TSliceState, TSelected> selector)
        {
            return _store.GetState(sliceName, selector);
        }

        public Observable<TSelected> Select<TState, TSelected>(string sliceName, Selector<TState, TSelected> selector)
        {
            return _store.Select(sliceName, selector);
        }

        public Observable<TState> Select<TState>(string sliceName)
        {
            return _store.Select<TState>(sliceName);
        }

        public IDisposable Select<TState, TSelected>(string sliceName, Selector<TState, TSelected> selector, System.Action<TSelected> onNext)
        {
            return _store.Select(sliceName, selector, onNext);
        }

        public IDisposable Select<TState>(string sliceName, System.Action<TState> onNext)
        {
            return _store.Select(sliceName, onNext);
        }

        public IDisposable SelectWhere<TState, TSelected>(string sliceName, Selector<TState, TSelected> selector, Func<TSelected, bool> predicate, System.Action<TSelected> onNext)
        {
            return _store.SelectWhere(sliceName, selector, predicate, onNext);
        }

        public IDisposable SelectWhere<TState>(string sliceName, Func<TState, bool> predicate, System.Action<TState> onNext)
        {
            return _store.SelectWhere(sliceName, predicate, onNext);
        }

        public IDisposable SelectDebounce<TState, TSelected>(string sliceName, TimeSpan dueTime, System.Action<TState> onNext)
        {
            return _store.SelectDebounce<TState, TSelected>(sliceName, dueTime, onNext);
        }

        public IDisposable SelectDebounce<TState, TSelected>(string sliceName, Selector<TState, TSelected> selector, TimeSpan dueTime, System.Action<TSelected> onNext)
        {
            return _store.SelectDebounce(sliceName, selector, dueTime, onNext);
        }

        public TState GetState<TState>(string name)
        {
            return _store.GetState<TState>(name);
        }

        public Dictionary<string, object> GetState()
        {
            return _store.GetState();
        }
    }
}
