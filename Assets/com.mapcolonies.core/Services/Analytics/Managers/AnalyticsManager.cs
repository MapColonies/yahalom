using System;
using System.IO;
using System.Threading;
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
    public class AnalyticsManager : IInitializable, IDisposable
    {
        public static long SessionId;

        private delegate Task PublishDelegate(LogObject logObject);

        private static PublishDelegate _publish;
        private static string _logFilePath;
        private static readonly SemaphoreSlim _fileSemaphore = new SemaphoreSlim(1, 1);
        private static bool _isInitialized;

        public void Initialize()
        {
            try
            {
                SessionId = UnityEngine.Analytics.AnalyticsSessionInfo.sessionId;

                if (SessionId == 0)
                {
                    SessionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                }

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
                _isInitialized = true;

                Debug.Log($"AnalyticsManager initialized successfully (Session: {SessionId})");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize AnalyticsManager: {ex.Message}");
                _isInitialized = false;
            }
        }

        /// <summary>
        /// Sets up the directory and file path for the current session's analytics log.
        /// </summary>
        private void SetupLogFile()
        {
            try
            {
                string logDirectory = Path.Combine(Application.persistentDataPath, "AnalyticsLogs");

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                    Debug.Log($"Created analytics log directory: {logDirectory}");
                }

                _logFilePath = Path.Combine(logDirectory, $"session-{SessionId}.log");
                Debug.Log($"Analytics log file for this session: {_logFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create analytics log file: {ex.Message}");
                _logFilePath = null;
            }
        }

        private static async Task PublishAnalytics(LogObject logObject)
        {
            try
            {
                string json = JsonConvert.SerializeObject(logObject, Formatting.None);
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
            if (string.IsNullOrEmpty(_logFilePath))
            {
                Debug.LogWarning("Log file path is not set, skipping file write");
                return;
            }

            await _fileSemaphore.WaitAsync();
            try
            {
                await File.AppendAllTextAsync(_logFilePath, logContent + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write analytics to file {_logFilePath}: {ex.Message}");
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }

        public static async Task Publish(LogObject logObject)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("AnalyticsManager not initialized, cannot publish log");
                return;
            }

            if (_publish == null)
            {
                Debug.LogWarning("Publish delegate is null, cannot publish log");
                return;
            }

            await _publish(logObject);
        }

        public void Dispose()
        {
            _fileSemaphore?.Dispose();
        }
    }
}
