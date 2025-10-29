using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace com.mapcolonies.yahalom.SceneController
{
    public interface ISceneController
    {
        UniTask SwitchToAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single);
    }

    public class SceneController : ISceneController
    {
        public async UniTask SwitchToAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, mode);

            if (op == null)
            {
                throw new InvalidOperationException($"Failed to start loading scene '{sceneName}'. " +
                                                    $"Make sure it’s added to Build Settings.");
            }

            while (!op.isDone)
            {
                await UniTask.Yield();
            }
        }
    }
}
