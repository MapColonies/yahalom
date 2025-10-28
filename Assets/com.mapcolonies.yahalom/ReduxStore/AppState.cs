using System;
using com.mapcolonies.yahalom.Configuration;

namespace com.mapcolonies.yahalom.Redux
{
    [Serializable]
    public class AppState
    {
        public ConfigurationState ConfigurationState { get; }

        public AppState(ConfigurationState configurationState)
        {
            ConfigurationState = configurationState;
        }

        public AppState With(ConfigurationState configurationState)
        {
            return new AppState(configurationState);
        }
    }
}
