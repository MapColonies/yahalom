using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Redux
{
    public record InitConfigurationAction() : IAction
    {
        public string type
        {
            get;
        }

    }
}
