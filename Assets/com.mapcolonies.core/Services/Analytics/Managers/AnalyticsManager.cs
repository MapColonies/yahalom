using System;
using System.Threading;
using com.mapcolonies.core.Services.Analytics.Model;
using com.mapcolonies.core.Utilities;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace com.mapcolonies.core.Services.Analytics.Managers
{
    public interface IAnalyticsManager
    {
        string SessionId
        {
            get;
        }

        UniTask Publish(LogObject logObject);
    }

    /// <summary>
    /// Analytics Manager responsible for creating and publishing logs to a local file.
    /// </summary>
    public class AnalyticsManager : IDisposable, IAnalyticsManager
    {
        public string SessionId
        {
            get;
            private set;
        }

        public const string AnalyticsFileName = "AnalyticsLogs";

        private delegate UniTask PublishDelegate(LogObject logObject);

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

                _publish = enablePublishing ? PublishAnalytics : (_) => UniTask.CompletedTask;
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
            _logFilePath = FileUtility.GetFullPath(AnalyticsFileName, $"session-{SessionId}.log");
        }

        private async UniTask PublishAnalytics(LogObject logObject)
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

        private async UniTask WriteLogToFileAsync(string logContent)
        {
            if (_fileSemaphore == null)
            {
                Debug.LogError($"Semaphore is null for file {_logFilePath}. Write operation is not thread-safe and was skipped.");
                return;
            }

            await _fileSemaphore.WaitAsync();

            try
            {
                await FileUtility.AppendLineToFileAsync(logContent, _logFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to file {_logFilePath}: {e.Message}");
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }

        public async UniTask Publish(LogObject logObject)
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
