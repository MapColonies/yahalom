using com.mapcolonies.core.Services.Analytics.Enums;
using com.mapcolonies.core.Services.Analytics.Managers;
using com.mapcolonies.core.Services.Analytics.Model;
using UnityEngine;

namespace Domains.Analytics.Utilities
{
    public static class AnalyticsHelper
    {
        public static void PublishLayerChange(AnalyticsMessageTypes messageType, string layerDomain, string layerId)
        {
            var messageParameters = LayerData.Create(layerDomain, layerId);
            var severity = LogType.Log;
            var message = messageType.ToString();
            var logObject = LogObject.Create(severity, message, messageParameters, LogComponent.General, messageType);

            AnalyticsManager.Publish(logObject);
        }
    }
}
