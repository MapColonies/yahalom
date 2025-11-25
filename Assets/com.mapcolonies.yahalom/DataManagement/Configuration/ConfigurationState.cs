using System;

namespace com.mapcolonies.yahalom.DataManagement.Configuration
{
    [Serializable]
    public record ConfigurationState()
    {
        public string Url
        {
            get;
            set;
        }
    }
}
