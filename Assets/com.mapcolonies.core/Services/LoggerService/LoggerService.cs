using System;
using System.IO;
using log4net;
using log4net.Config;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace com.mapcolonies.core.Services.LoggerService
{
    public class LoggerService
    {
        private const string LogFilePath = "LogFilePath";
        private readonly LoggerServiceConfig _config;
        private ILogHandler _log4NetLogHandler;

        public LoggerService(LoggerServiceConfig config)
        {
            _config = config;

            if (!config.ServiceEnabled)
            {
                return;
            }

            bool success = InitializeLog4Net();

            if (!success)
            {
                return;
            }

            SubscribeToErrorEvents();

            Debug.unityLogger.logEnabled = _config.EnableConsole;
        }

        private bool InitializeLog4Net()
        {
            bool success = true;

            string logConfigFilePath = Path.Combine(Application.streamingAssetsPath, _config.Log4NetConfigXml);

            if (!File.Exists(logConfigFilePath))
            {
                Debug.LogError($"log4net config file not found at: {logConfigFilePath}");
                success = false;
            }

            if (!TryInitializeLogDirectory(out string logDirectory))
            {
                Debug.LogError("Failed to initialize log directory.");
                success = false;
            }
            else
            {
                GlobalContext.Properties[LogFilePath] = logDirectory;
            }

            if (!success) return false;

            FileInfo logConfigFile = new FileInfo(logConfigFilePath);
            XmlConfigurator.Configure(logConfigFile);
            _log4NetLogHandler = new Log4NetHandler();

            return true;
        }

        private bool TryInitializeLogDirectory(out string logDirectory)
        {
            bool success = true;
            logDirectory = _config.GetSystemLogsDirectory();

            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create log directory at {logDirectory}. Error: {ex}");

                logDirectory = Path.Combine(Application.persistentDataPath, "logs");

                try
                {
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                }
                catch (Exception fallbackEx)
                {
                    Debug.LogError($"Failed to create fallback log directory at {logDirectory}. Error: {fallbackEx}");
                    success = false;
                }

                string logConfigFilePath = Path.Combine(Application.streamingAssetsPath, _config.Log4NetConfigXml);

                if (!success || !File.Exists(logConfigFilePath))
                {
                    Debug.LogError($"log4net config file not found at: {logConfigFilePath}");
                    success = false;
                }
            }

            if (success && !logDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                logDirectory += Path.DirectorySeparatorChar;
            }

            return success;
        }

        private void SubscribeToErrorEvents()
        {
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void UnSubscribeFromErrorEvents()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log4NetLogHandler.LogException(e.ExceptionObject as Exception, sender as Object);
        }

        private void OnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            _log4NetLogHandler.LogFormat(type, null, "{0}\n{1}", condition, stacktrace);
        }

        public void Dispose()
        {
            UnSubscribeFromErrorEvents();
        }
    }
}
