using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using com.mapcolonies.core.Services.Analytics.Model;
using com.mapcolonies.core.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using VContainer.Unity;

namespace com.mapcolonies.core.Services.Analytics.Managers
{
    public interface IAnalyticsManager
    {
        string SessionId { get; }
        Task Publish(LogObject logObject);
    }

    /// <summary>
    /// Analytics Manager responsible for creating and publishing logs to a local file.
    /// </summary>
    public class AnalyticsManager : IInitializable, IDisposable, IAnalyticsManager
    {
        public string SessionId { get; private set; }
        public const string AnalyticsFileName = "AnalyticsLogs";

        private delegate Task PublishDelegate(LogObject logObject);

        private PublishDelegate _publish;
        private string _logFilePath;
        private readonly SemaphoreSlim _fileSemaphore = new SemaphoreSlim(1, 1);
        private bool _isInitialized;

        public void Initialize()
        {
            try
            {
                SessionId = Guid.NewGuid().ToString();

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

        private void SetupLogFile()
        {
            _logFilePath = FileUtility.SetupFilePath(AnalyticsFileName, $"session-{SessionId}.log");
        }

        private async Task PublishAnalytics(LogObject logObject)
        {
            try
            {
                string json = JsonConvert.SerializeObject(logObject, Formatting.None);
                //TODO:Write to file only in "disconnected" mode
                await WriteLogToFileAsync(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to publish log: {ex.Message}");
            }
        }

        private async Task WriteLogToFileAsync(string logContent)
        {
            await FileUtility.AppendLineToFileSafeAsync(logContent, _logFilePath, _fileSemaphore);
        }

        public async Task Publish(LogObject logObject)
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

            logObject.SessionID = SessionId;
            await _publish(logObject);
        }

        public void Dispose()
        {
            _fileSemaphore?.Dispose();
        }
    }
}
