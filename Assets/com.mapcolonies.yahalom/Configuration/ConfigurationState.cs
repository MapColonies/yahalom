using System;

namespace com.mapcolonies.yahalom.Configuration
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
