using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace com.mapcolonies.yahalom.SceneManagement
{
    public interface ISceneController
    {
        UniTask SwitchSceneAsync(string sceneName);
    }

    public class SceneController : ISceneController
    {
        private string _currentLoadedScene;

        public async UniTask SwitchSceneAsync(string newSceneName)
        {
            if (!string.IsNullOrEmpty(_currentLoadedScene))
            {
                var unloadOp = SceneManager.UnloadSceneAsync(_currentLoadedScene);
                while (!unloadOp.isDone)
                    await UniTask.Yield();
            }

            var loadOp = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
            while (!loadOp.isDone)
                await UniTask.Yield();

            _currentLoadedScene = newSceneName;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
        }
    }
}
