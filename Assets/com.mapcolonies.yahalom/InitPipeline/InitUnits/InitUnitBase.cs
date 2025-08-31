using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.mapcolonies.yahalom.InitPipeline.InitUnits
{
    public abstract class InitUnitBase : IInitUnit
    {
        public string Name { get; }
        public float Weight { get; }
        public InitPolicy Policy { get; }

        protected InitUnitBase(string name, float weight, InitPolicy policy)
        {
            Name = name;
            Weight = weight;
            Policy = policy;
        }

        public abstract UniTask RunAsync();
        
        protected async UniTask HandlePolicy(Func<UniTask> action)
        {
            try
            {
                await action();
            }
            catch (Exception e)
            {
                switch (Policy)
                {
                    case InitPolicy.Fail:
                        throw;
                    case InitPolicy.Ignore:
                        Debug.LogWarning($"Init unit {Name} is ignored. {e.Message}");
                        break;
                    case InitPolicy.Retry:
                        Debug.LogWarning($"Init unit {Name} failed. {e.Message} retry!");
                        await action();
                        break;
                    default:
                        Debug.LogError($"No policy {Policy} exists. {e.Message}");
                        break;
                }
            }
        }
    }
}