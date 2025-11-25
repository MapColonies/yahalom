using Cysharp.Threading.Tasks;
using VContainer;

namespace com.mapcolonies.yahalom.AppMode
{
    public class AppModeSwitcher
    {
        private IAppMode _currentAppMode;
        private IObjectResolver _resolver;

        public void RegisterChildScope(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public async UniTask SetInitialMode<T>() where T : IAppMode
        {
            _currentAppMode = _resolver.Resolve<T>();
            await _currentAppMode.EnterMode();
        }

        public async UniTask ChangeMode<T>() where T : IAppMode
        {
            if (_currentAppMode == (IAppMode)_resolver.Resolve<T>())
                return;

            await _currentAppMode.ExitMode();
            _currentAppMode = _resolver.Resolve<T>();
            await _currentAppMode.EnterMode();
        }
    }
}
