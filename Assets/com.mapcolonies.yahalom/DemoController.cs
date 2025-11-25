using com.mapcolonies.yahalom.AppMode;
using com.mapcolonies.yahalom.AppMode.Modes;
using com.mapcolonies.yahalom.SceneManagement.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using SimulationMode = com.mapcolonies.yahalom.AppMode.Modes.SimulationMode;

namespace com.mapcolonies.yahalom
{
    public class DemoController : MonoBehaviour
    {
        private AppModeSwitcher _appModeSwitcher;

        [Inject]
        public void Construct(AppModeSwitcher appModeSwitcher)
        {
            _appModeSwitcher = appModeSwitcher;
        }

        public void SwitchScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName.Equals(Scenes.SimulationScene.ToString()))
            {
                _appModeSwitcher.ChangeMode<PlanningMode>().Forget();
            }
            else if (currentSceneName.Equals(Scenes.PlanningScene.ToString()))
            {
                _appModeSwitcher.ChangeMode<SimulationMode>().Forget();
            }
            else
            {
                Debug.LogWarning($"Current scene '{currentSceneName}' is not part of the demo flow.");
            }
        }
    }
}
