using System;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.InitPipeline
{
    public class RegisterScopeUnit : IInitUnit
    {
        private readonly LifetimeScope _parent;
        public string Name { get; }
        public float Weight { get; }
        
        private readonly Action<IContainerBuilder> _installers; 
        private readonly Func<IObjectResolver, UniTask> _afterBuild;
        private LifetimeScope _child;

        public RegisterScopeUnit(string name, float weight, LifetimeScope parentScope,
            Action<IContainerBuilder> installers = null, Func<IObjectResolver, UniTask> afterBuild = null)
        {
            Name = name;
            Weight = weight;
            
            _parent = parentScope;
            _installers = installers;
            _afterBuild = afterBuild;
        }

        public async UniTask RunAsync()
        {
            _child = _parent.CreateChild(_installers, Name);
            
            if (_afterBuild != null)
                await _afterBuild(_child.Container);
            
            await UniTask.Yield();
        }
        
        public void Dispose()
        {
            if (_child != null)
            {
                _child.Dispose();
                _child = null;
            }
        }
    }
}