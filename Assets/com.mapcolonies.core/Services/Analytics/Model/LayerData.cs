using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Interfaces;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    public class LayerData : IAnalyticLogParameter
    {
        public string LayerDomain { get; private set; }

        public string UniqueLayerId { get; private set; }

        private LayerData(string layerDomain, string uniqueLayerId)
        {
            LayerDomain = layerDomain;
            UniqueLayerId = uniqueLayerId;
        }

        public static LayerData Create(string layerDomain, string uniqueLayerId)
        {
            return new LayerData(layerDomain, uniqueLayerId);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(LayerDomain), LayerDomain);
            info.AddValue(nameof(UniqueLayerId), UniqueLayerId);
        }
    }
}
