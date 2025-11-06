using R3;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public sealed class EpicMiddlewareCreator
    {
        private readonly Subject<IAction> _actionStream = new Subject<IAction>();

        public Middleware<PartitionedState> Create()
        {
            return store => next => action =>
            {
                next(action);
                _actionStream.OnNext(action);
            };
        }
    }
}
