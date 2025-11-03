using System;
using log4net.Appender;
using log4net.Core;
using UnityEngine;
using IUnityLogger = UnityEngine.ILogger;
using Object = UnityEngine.Object;

namespace com.mapcolonies.core.Services.LoggerService.CustomAppenders
{
    public class ConsoleAppender : AppenderSkeleton
    {
        public const string UnityContext = "unity:context";
        private readonly IUnityLogger _logger;
        private static readonly int ErrorLevel = Level.Error.Value;
        private static readonly int WarningLevel = Level.Warn.Value;

        public ConsoleAppender(IUnityLogger logger)
        {
            _logger = logger;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            Level level = loggingEvent.Level;

            if (level == null) return;

            string message;

            try
            {
                message = RenderLoggingEvent(loggingEvent);
            }
            catch (Exception e)
            {
                _logger?.LogException(e, null);
                return;
            }

            Object ctx = loggingEvent.LookupProperty(UnityContext) as Object;

            LogType logType = level.Value switch
            {
                var value when value < WarningLevel => LogType.Log,
                var value when value >= WarningLevel && value < ErrorLevel => LogType.Warning,
                var value when value >= ErrorLevel => LogType.Error,
                _ => LogType.Log
            };

            _logger?.LogFormat(logType, ctx, "{0}", message);
        }
    }
}
