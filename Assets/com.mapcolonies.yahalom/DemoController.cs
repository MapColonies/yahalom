using com.mapcolonies.yahalom.SceneManagement;
using com.mapcolonies.yahalom.SceneManagement.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace com.mapcolonies.yahalom
{
    public class DemoController : MonoBehaviour
    {
        private ISceneController _sceneController;

        [Inject]
        public void Construct(ISceneController sceneController)
        {
            _sceneController = sceneController;
        }

        public void SwitchScene()
        {
            Scenes nextScene;
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName.Equals(Scenes.SimulationScene.ToString()))
            {
                nextScene = Scenes.PlanningScene;
            }
            else if (currentSceneName.Equals(Scenes.PlanningScene.ToString()))
            {
                nextScene = Scenes.SimulationScene;
            }
            else
            {
                Debug.LogWarning($"Current scene '{currentSceneName}' is not part of the demo flow.");
                return;
            }

            _ = _sceneController.SwitchSceneAsync(nextScene);
        }
    }
}
