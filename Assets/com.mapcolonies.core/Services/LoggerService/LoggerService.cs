using System;
using System.IO;
using com.mapcolonies.core.Services.LoggerService.CustomAppenders;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace com.mapcolonies.core.Services.LoggerService
{
    public class LoggerService : IDisposable
    {
        private const string LogFilePath = "LogFilePath";
        private const string Pattern = "%date %-5level %logger - %message%newline";
        private readonly LoggerServiceConfig _config;

        private ILogHandler _originalUnityLogHandler;
        private ILogHandler _log4NetLogHandler;

        public LoggerService(LoggerServiceConfig config)
        {
            _config = config;

            if (!config.ServiceEnabled) return;

            bool success = InitializeLog4Net();

            if (!success)
            {
                Dispose();
                return;
            }

            _log4NetLogHandler = new Log4NetHandler();
            Debug.unityLogger.logHandler = _log4NetLogHandler;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Debug.unityLogger.logEnabled = true;
        }

        private bool InitializeLog4Net()
        {
            try
            {
                _originalUnityLogHandler = Debug.unityLogger.logHandler;
                string logConfigFilePath = Path.Combine(Application.streamingAssetsPath, _config.Log4NetConfigXml);

                if (!File.Exists(logConfigFilePath))
                {
                    Debug.LogError($"log4net config file not found at: {logConfigFilePath}");
                    return false;
                }

                if (!TryInitializeLogDirectory(out string logDirectory))
                {
                    Debug.LogError("Failed to initialize log directory.");
                    return false;
                }

                GlobalContext.Properties[LogFilePath] = logDirectory;

                FileInfo logConfigFile = new FileInfo(logConfigFilePath);
                XmlConfigurator.Configure(logConfigFile);

                Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

                ConsoleAppender consoleAppender = new ConsoleAppender(_originalUnityLogHandler);

                PatternLayout layout = new PatternLayout();
                layout.ConversionPattern = Pattern;
                layout.ActivateOptions();
                consoleAppender.Layout = layout;

                Level consoleThreshold = hierarchy.LevelMap[_config.MinLogLevel] ?? Level.Debug;
                consoleAppender.Threshold = consoleThreshold;
                consoleAppender.ActivateOptions();

                hierarchy.Root.AddAppender(consoleAppender);

                if (!_config.EnableConsole)
                {
                    hierarchy.Root.RemoveAppender(consoleAppender);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error initializing log4net: {ex.Message}");
                return false;
            }
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

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log4NetLogHandler?.LogException(e.ExceptionObject as Exception, null);
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;

            if (_originalUnityLogHandler != null)
            {
                Debug.unityLogger.logHandler = _originalUnityLogHandler;
                _originalUnityLogHandler = null;
            }
        }
    }
}
