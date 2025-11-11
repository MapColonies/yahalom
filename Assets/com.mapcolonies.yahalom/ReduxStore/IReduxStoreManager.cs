using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity.AppUI.Redux;
using VContainer;
using Action = Unity.AppUI.Redux.Action;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public interface IReduxStoreManager
    {
        UniTask Create();
        Dictionary<string, object> GetState();
        TState GetState<TState>(string name);
        TSelected GetState<TSliceState, TSelected>(string sliceName, Selector<TSliceState, TSelected> selector);
        Observable<TSelected> Select<TState, TSelected>(string sliceName, Selector<TState, TSelected> selector);
        Observable<TState> Select<TState>(string sliceName);
        IDisposable Select<TState, TSelected>(string sliceName, Selector<TState, TSelected> selector, System.Action<TSelected> onNext);
        IDisposable Select<TState>(string sliceName, System.Action<TState> onNext);
        IDisposable SelectWhere<TState, TSelected>(string sliceName, Selector<TState, TSelected> selector, Func<TSelected, bool> predicate, System.Action<TSelected> onNext);
        IDisposable SelectWhere<TState>(string sliceName, Func<TState, bool> predicate, System.Action<TState> onNext);
        IDisposable SelectDebounce<TState, TSelected>(string sliceName, TimeSpan dueTime, System.Action<TState> onNext);
        IDisposable SelectDebounce<TState, TSelected>(string sliceName, Selector<TState, TSelected> selector, TimeSpan dueTime, System.Action<TSelected> onNext);
        UniTask DispatchAsync(IAction action, CancellationToken token = default);
        UniTask DispatchAsync(string actionType, CancellationToken token = default);
        UniTask DispatchAsync<T>(string actionType, T payload, CancellationToken token = default);
        void Dispatch<T>(string actionType, T payload);
        void Dispatch(string actionType);
        void Dispatch(IAction action);

    }
}
