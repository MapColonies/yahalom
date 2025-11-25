using System.Collections.Generic;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using com.mapcolonies.yahalom.SceneManagement;
using com.mapcolonies.yahalom.SceneManagement.Enums;
using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.AppMode.Modes
{
    public class SimulationMode : IAppMode
    {
        private readonly InitializationPipeline _pipeline;
        private readonly List<InitStep> _steps;

        public SimulationMode(InitializationPipeline initializationPipeline, ISceneController sceneController)
        {
            _pipeline = initializationPipeline;

            _steps = new List<InitStep>
            {
                new InitStep("Planning Mode Init Steps", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Dummy 3", 0.1f, InitPolicy.Fail,
                        () => UniTask.Delay(1000)),
                    new ActionUnit("Dummy 4", 0.1f, InitPolicy.Fail,
                        () => UniTask.Delay(1000)),
                    new ActionUnit("Switch Scene", 0.1f, InitPolicy.Fail,
                        () => sceneController.SwitchSceneAsync(Scenes.SimulationScene))
                })
            };
        }

        public async UniTask EnterMode()
        {
            await _pipeline.RunAsync(_steps);
        }

        public UniTask ExitMode()
        {
            return UniTask.CompletedTask;
        }
    }
}
