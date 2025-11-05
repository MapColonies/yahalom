using System;
using com.mapcolonies.yahalom.SceneManagement.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.mapcolonies.yahalom.SceneManagement
{
    public interface ISceneController
    {
        UniTask SwitchSceneAsync(Scenes scene);
    }

    public class SceneController : ISceneController
    {
        private Scenes? _currentLoadedScene;

        public async UniTask SwitchSceneAsync(Scenes newScene)
        {
            if (!Enum.IsDefined(typeof(Scenes), newScene))
            {
                Debug.LogWarning($"Invalid Scene.");
                return;
            }

            if (_currentLoadedScene.HasValue)
            {
                await SceneManager.UnloadSceneAsync(_currentLoadedScene.Value.ToString());
            }

            string newSceneName = newScene.ToString();
            await SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);

            _currentLoadedScene = newScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
        }
    }
}
