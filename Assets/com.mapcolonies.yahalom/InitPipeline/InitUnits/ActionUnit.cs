using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.mapcolonies.yahalom.InitPipeline.InitUnits
{
    public class ActionUnit : InitUnitBase
    {
        private readonly Func<UniTask> _action;

        public ActionUnit(string name, float weight, InitPolicy policy, Func<UniTask> action) : base(name, weight,
            policy)
        {
            _action = action;
        }

        public override async UniTask RunAsync()
        {
            Debug.Log($"Running {Name} action unit");
            await HandlePolicy(() => _action());
        }
    }
}
