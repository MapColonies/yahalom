using System;
using System.IO;
using System.Threading.Tasks;
using com.mapcolonies.core.Services.Analytics.Model;
using Newtonsoft.Json;
using UnityEngine;
using VContainer.Unity;

namespace com.mapcolonies.core.Services.Analytics.Managers
{
    /// <summary>
    /// Analytics Manager responsible for creating and publishing logs to a local file.
    /// </summary>
    public class AnalyticsManager : IInitializable
    {
        public static long SessionId;
        public static bool EnablePublishingAnalytics;

        private delegate Task PublishDelegate(LogObject logObject);

        private static PublishDelegate _publish;
        private static string _logFilePath;
        private static readonly object _fileLock = new object();

        public void Initialize()
        {
            SessionId = UnityEngine.Analytics.AnalyticsSessionInfo.sessionId;

            SetupLogFile();

            bool enablePublishing = true;
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

            _publish = enablePublishing ? PublishAnalytics : (_) => Task.CompletedTask;
        }

        /// <summary>
        /// Sets up the directory and file path for the current session's analytics log.
        /// </summary>
        private void SetupLogFile()
        {
            try
            {
                string logDirectory = Path.Combine(Application.persistentDataPath, "AnalyticsLogs");
                Directory.CreateDirectory(logDirectory);
                _logFilePath = Path.Combine(logDirectory, $"session-{SessionId}.log");
                Debug.Log($"Analytics log file for this session: {_logFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create analytics log file: {ex.Message}");
            }
        }

        private static async Task PublishAnalytics(LogObject logObject)
        {
            try
            {
                string json = JsonConvert.SerializeObject(logObject, Formatting.Indented);
                Debug.Log($"{logObject.MessageType}: {json}");
                await WriteLogToFileAsync(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to publish log: {ex.Message}");
            }
        }

        private static async Task WriteLogToFileAsync(string logContent)
        {
            if (string.IsNullOrEmpty(_logFilePath)) return;

            try
            {
                lock (_fileLock)
                {
                }

                await File.AppendAllTextAsync(_logFilePath, logContent + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write analytics to file {_logFilePath}: {ex.Message}");
            }
        }


        public static async Task Publish(LogObject logObject)
        {
            await _publish(logObject);
        }
    }
}
