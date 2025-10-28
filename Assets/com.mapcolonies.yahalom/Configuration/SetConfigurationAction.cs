using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Configuration
{
    public record SetConfigurationAction : IAction
    {
        public string type => nameof(SetConfigurationAction);
        public ConfigurationState ConfigurationState { get; }

        public SetConfigurationAction(ConfigurationState configurationState)
        {
            ConfigurationState = configurationState;
        }
    }
}
