using System;
using System.IO;
using com.mapcolonies.core.Services.LoggerService.CustomAppenders;
using Cysharp.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using UnityEngine;
using ConsoleAppender = com.mapcolonies.core.Services.LoggerService.CustomAppenders.ConsoleAppender;
using Debug = UnityEngine.Debug;

namespace com.mapcolonies.core.Services.LoggerService
{
    public class LoggerService : IDisposable
    {
        private const string LogFilePath = "LogFilePath";
        private const string HttpEndpointUrl = "HttpEndpointUrl";
        private const string HttpPersistenceDirectory = "HttpPersistenceDirectory";
        private const string Pattern = "%date %-5level %logger - %message%newline";
        private const string ConsoleAppenderName = "Console";
        private const string FileAppenderName = "File";
        private const string HttpAppenderName = "Http";

        private readonly LoggerServiceConfig _config;

        private ILogHandler _originalUnityLogHandler;
        private ILogHandler _log4NetLogHandler;

        public LoggerService(LoggerServiceConfig config)
        {
            _config = config;

            if (!config.Settings.ServiceEnabled) return;

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
                string logConfigFilePath = Path.Combine(Application.streamingAssetsPath, _config.Settings.Log4NetConfigXml);

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
                GlobalContext.Properties[HttpEndpointUrl] = _config.Settings.HttpEndpointUrl;
                GlobalContext.Properties[HttpPersistenceDirectory] = _config.GetHttpPersistenceDirectory();

                FileInfo logConfigFile = new FileInfo(logConfigFilePath);
                XmlConfigurator.Configure(logConfigFile);

                Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
                bool isDev = Application.isEditor || Debug.isDebugBuild;

                if (!isDev)
                {
                    foreach (IAppender appender in hierarchy.GetAppenders())
                    {
                        if (appender is AppenderSkeleton sk)
                        {
                            if (appender.Name.Contains(ConsoleAppenderName, StringComparison.OrdinalIgnoreCase))
                            {
                                sk.Threshold = hierarchy.LevelMap[_config.Settings.MinConsoleLogLevel] ?? Level.Debug;
                            }
                            else
                            {
                                sk.Threshold = Level.Debug;
                            }

                            sk.ActivateOptions();
                        }
                    }

                    ApplyFileAndHttpThresholds(hierarchy, _config.Settings);
                }

                ConsoleAppender consoleAppender = new ConsoleAppender(_originalUnityLogHandler);

                PatternLayout layout = new PatternLayout();
                layout.ConversionPattern = Pattern;
                layout.ActivateOptions();
                consoleAppender.Layout = layout;

                consoleAppender.Threshold = hierarchy.LevelMap[_config.Settings.MinConsoleLogLevel] ?? Level.Debug;
                consoleAppender.ActivateOptions();

                hierarchy.Root.AddAppender(consoleAppender);

                if (!_config.Settings.ConsoleEnabled)
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

                string logConfigFilePath = Path.Combine(Application.streamingAssetsPath, _config.Settings.Log4NetConfigXml);

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

        private void ApplyFileAndHttpThresholds(Hierarchy hierarchy, LoggerSettings settings)
        {
            foreach (IAppender appender in hierarchy.GetAppenders())
            {
                if (appender is AppenderSkeleton sk)
                {
                    if (appender.Name.Contains(FileAppenderName, StringComparison.OrdinalIgnoreCase) ||
                        appender is RollingFileAppender)
                    {
                        sk.Threshold = hierarchy.LevelMap[settings.MinFileLogLevel] ?? Level.Debug;
                    }
                    else if (appender.Name.Contains(HttpAppenderName, StringComparison.OrdinalIgnoreCase) ||
                             appender is HttpAppender)
                    {
                        sk.Threshold = hierarchy.LevelMap[settings.MinHttpLogLevel] ?? Level.Debug;
                    }

                    sk.ActivateOptions();
                }
            }
        }

        public async UniTask<bool> UpdateLoggerSettings(LoggerSettings settings)
        {
            if (!settings.ForceMinLogLevel) return false;

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            bool isDev = Application.isEditor || Debug.isDebugBuild;

            if (isDev) return false;

            ApplyFileAndHttpThresholds(hierarchy, settings);
            return true;
        }
    }
}
