using System;
using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.InitPipeline
{
    public class ActionUnit : IInitUnit
    {
        private readonly Func<UniTask> _action;
        public string Name { get; }
        public float Weight { get; }

        public ActionUnit(string name, float weight, Func<UniTask> action)
        {
            Name = name; 
            Weight = weight; 
            _action = action;
        }
        
        public UniTask RunAsync()
        {
            return _action.Invoke();
        }
    }
}