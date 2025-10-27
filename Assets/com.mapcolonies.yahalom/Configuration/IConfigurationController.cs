using com.mapcolonies.core.Services.ConfigurationService;

namespace com.mapcolonies.yahalom.Configuration
{
    public interface IConfigurationController
    {
        ConfigurationState Load();
    }
}
