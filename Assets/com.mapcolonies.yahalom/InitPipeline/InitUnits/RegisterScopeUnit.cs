using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.InitPipeline.InitUnits
{
    public class RegisterScopeUnit : InitUnitBase
    {
        private readonly LifetimeScope _parent;
        
        private readonly Action<IContainerBuilder> _installers; 
        private readonly Func<IObjectResolver, UniTask> _afterBuild;
        private LifetimeScope _child;

        public RegisterScopeUnit(string name, float weight, LifetimeScope parentScope, InitPolicy policy,
            Action<IContainerBuilder> installers = null, Func<IObjectResolver, UniTask> afterBuild = null) :  base(name, weight, policy)
        {
            _parent = parentScope;
            _installers = installers;
            _afterBuild = afterBuild;
        }

        public override async UniTask RunAsync()
        {
            Debug.Log($"Running {Name} action unit");
            
            await HandlePolicy(async () =>
            {
                _child = _parent.CreateChild(_installers, Name);
                if (_afterBuild != null)
                {
                    await _afterBuild(_child.Container);
                }
            });
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