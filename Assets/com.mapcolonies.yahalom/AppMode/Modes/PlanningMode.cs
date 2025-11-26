using System.Collections.Generic;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using com.mapcolonies.yahalom.SceneManagement;
using com.mapcolonies.yahalom.SceneManagement.Enums;
using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.AppMode.Modes
{
    public class PlanningMode : IAppMode
    {
        private const float StartPercentage = 0.8f;
        private bool _firstRun = true;
        private InitializationPipeline _pipeline;
        private readonly List<InitStep> _steps;

        public PlanningMode(InitializationPipeline initializationPipeline, ISceneController sceneController)
        {
            _pipeline = initializationPipeline;

            _steps = new List<InitStep>
            {
                new InitStep("Planning Mode Init Steps", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Dummy 1", 0.1f, InitPolicy.Fail,
                        () => UniTask.Delay(1000)),
                    new ActionUnit("Dummy 2", 0.1f, InitPolicy.Fail,
                        () => UniTask.Delay(1000)),
                    new ActionUnit("Switch Scene", 0.1f, InitPolicy.Fail,
                        () => sceneController.SwitchSceneAsync(Scenes.PlanningScene))
                })
            };
        }

        public async UniTask EnterMode()
        {
            if (_firstRun)
            {
                await _pipeline.RunAsync(_steps, StartPercentage);
                _firstRun = false;
            }
            else
            {
                await _pipeline.RunAsync(_steps);
            }
        }

        public UniTask ExitMode()
        {
            return UniTask.CompletedTask;
        }
    }
}
