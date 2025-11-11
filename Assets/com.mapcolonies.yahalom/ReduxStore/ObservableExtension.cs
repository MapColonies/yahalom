using System;
using R3;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public static class ObservableExtension
    {
        public static Observable<IAction> OfAction(this Observable<IAction> source, string actionType)
        {
            if (string.IsNullOrEmpty(actionType)) throw new ArgumentNullException(nameof(actionType));
            return source.Where(actionType, static (action, type) => action.type == type);
        }

        public static Observable<IAction> OfAction(this Observable<IAction> source, ActionCreator action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return source.Where(action, static (input, expected) => input.type == expected.type);
        }

        public static Observable<IAction> OfAction<T>(this Observable<IAction> source, ActionCreator<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return source.Where(action, static (input, expected) => input.type == expected.type);
        }

        public static Observable<IAction> Dispatch(this Observable<IAction> source, IAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return source.Select(action, static (_, self) => self);
        }

        public static Observable<IAction> Dispatch<T>(this Observable<IAction> source, IAction<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return source.Select(action, static (_, self) => self as IAction);
        }
    }
}
