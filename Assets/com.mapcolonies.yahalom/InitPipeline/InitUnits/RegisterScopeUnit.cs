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

        public LifetimeScope Child
        {
            get;
            private set;
        }

        public RegisterScopeUnit(string name, float weight, LifetimeScope parentScope, InitPolicy policy,
            Action<IContainerBuilder> installers = null, Func<IObjectResolver, UniTask> afterBuild = null) : base(name,
            weight, policy)
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
                Child = _parent.CreateChild(_installers, Name);

                if (_afterBuild != null) await _afterBuild(Child.Container);
            });
        }

        public void Dispose()
        {
            if (!Child) return;

            Child.Dispose();
            Child = null;
        }
    }
}
