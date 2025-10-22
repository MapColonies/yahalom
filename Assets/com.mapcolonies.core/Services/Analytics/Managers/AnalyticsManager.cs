using System;
using com.mapcolonies.core.Services.Analytics.Model;
using Newtonsoft.Json;
using UnityEngine;
using VContainer.Unity;

namespace com.mapcolonies.core.Services.Analytics.Managers
{
    /// <summary>
    /// Analytics Manager responsible for creating and publishing logs.
    /// </summary>
    public class AnalyticsManager : IInitializable
    {
        public static long SessionId;
        public static bool EnablePublishingAnalytics;

        private delegate void PublishDelegate(LogObject logObject);

        private static PublishDelegate _publish;

        public void Initialize()
        {
            SessionId = UnityEngine.Analytics.AnalyticsSessionInfo.sessionId;

            var enablePublishing = true;
            //TODO: Remove after adding and supporting SharedConfigProvider
            /*if (SharedConfigProvider.TryGetSharedConfig(out var sharedConfig))
            {
                var systemLogConfiguration = sharedConfig.SystemLogConfiguration;
                if (systemLogConfiguration != null)
                {
                    enablePublishing = systemLogConfiguration.EnablePublishingAnalytics;
                }
                else
                {
                    Debug.LogWarning("No System Log configuration found, using default configuration.");
                }
            }*/

            _publish = enablePublishing ? PublishAnalytics : delegate { };
        }

        private static void PublishAnalytics(LogObject logObject)
        {
            try
            {
                var json = JsonConvert.SerializeObject(logObject, Formatting.Indented);
                Debug.Log($"{logObject.MessageType}: {json}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to publish log: {ex.Message}");
            }
        }

        public static void Publish(LogObject logObject)
        {
            _publish(logObject);
        }
    }
}
