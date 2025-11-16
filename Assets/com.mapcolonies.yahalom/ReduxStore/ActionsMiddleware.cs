using System;
using R3;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public class ActionsMiddleware : IDisposable
    {
        public readonly Subject<IAction> Actions = new Subject<IAction>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Middleware<PartitionedState> Create()
        {
            return store => next => action =>
            {
                Actions.OnNext(action);
                next(action); //next middleware
            };
        }
        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
