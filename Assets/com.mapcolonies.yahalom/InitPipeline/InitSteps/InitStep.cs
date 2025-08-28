using System.Collections.Generic;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;

namespace com.mapcolonies.yahalom.InitPipeline.InitSteps
{
    public class InitStep
    {
        public string Name { get; set; }
        public StepMode Mode { get; }
        public IReadOnlyList<IInitUnit> InitUnits { get; }

        public InitStep(string name, StepMode mode, IReadOnlyList<IInitUnit> units)
        {
            Name = name;
            Mode = mode;
            InitUnits = units;
        }
    }
}