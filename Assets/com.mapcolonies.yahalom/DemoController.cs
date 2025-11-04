using com.mapcolonies.yahalom.SceneManagement;
using com.mapcolonies.yahalom.SceneManagement.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

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
        string nextSceneName = string.Empty;
        var currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName.Equals(Scenes.SimulationScene.ToString()))
            nextSceneName = Scenes.PlanningScene.ToString();
        else if (currentSceneName.Equals(Scenes.PlanningScene.ToString()))
            nextSceneName = Scenes.SimulationScene.ToString();
        else
            return;

        _ = _sceneController.SwitchSceneAsync(nextSceneName);
    }
}
