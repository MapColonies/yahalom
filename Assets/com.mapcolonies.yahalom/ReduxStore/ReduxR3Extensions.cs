using System;
using R3;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public static class ReduxR3Extensions
    {
        public static Observable<TSelected> Select<TState, TSelected>(
            this IStore<PartitionedState> store, string sliceName,
            Selector<TState, TSelected> selector)
        {
            Selector<PartitionedState, TSelected> combined = s => selector(s.Get<TState>(sliceName));
            return Observable.Create<TSelected>(observer =>
                {
                    IDisposableSubscription subscription = store.Subscribe(combined, observer.OnNext);
                    return Disposable.Create(subscription.Dispose);
                })
                .DistinctUntilChanged();
        }

        public static Observable<TState> Select<TState>(this IStore<PartitionedState> store, string sliceName)
        {
            return Observable.Create<TState>(observer =>
                {
                    IDisposableSubscription subscription = store.Subscribe(s => s.Get<TState>(sliceName), observer.OnNext);
                    return Disposable.Create(subscription.Dispose);
                })
                .DistinctUntilChanged();
        }

        public static IDisposable Select<TState, TSelected>(
            this IStore<PartitionedState> store, string sliceName,
            Selector<TState, TSelected> selector, System.Action<TSelected> onNext)
        {
            return store.Select(sliceName, selector).Subscribe(onNext);
        }

        public static IDisposable Select<TState>(
            this IStore<PartitionedState> store, string sliceName, System.Action<TState> onNext)
        {
            return store.Select<TState>(sliceName).Subscribe(onNext);
        }

        public static IDisposable SelectWhere<TState, TSelected>(
            this IStore<PartitionedState> store, string sliceName, Selector<TState, TSelected> selector,
            Func<TSelected, bool> predicate, System.Action<TSelected> onNext)
        {
            return store.Select(sliceName, selector)
                .Where(predicate)
                .Subscribe(onNext);
        }

        public static IDisposable SelectWhere<TState>(
            this IStore<PartitionedState> store, string sliceName,
            Func<TState, bool> predicate, System.Action<TState> onNext)
        {
            return store.Select<TState>(sliceName)
                .Where(predicate)
                .Subscribe(onNext);
        }

        public static IDisposable SelectDebounce<TState, TSelected>(
            this IStore<PartitionedState> store,
            string sliceName,
            TimeSpan dueTime, System.Action<TState> onNext)
        {
            return store.Select<TState>(sliceName)
                .Debounce(dueTime)
                .Subscribe(onNext);
        }

        public static IDisposable SelectDebounce<TState, TSelected>(
            this IStore<PartitionedState> store,
            string sliceName,
            Selector<TState, TSelected> selector,
            TimeSpan dueTime, System.Action<TSelected> onNext)
        {
            return store.Select(sliceName, selector)
                .Debounce(dueTime)
                .Subscribe(onNext);
        }
    }
}
