using System;
using System.Globalization;
using com.mapcolonies.core.Services.LoggerService.CustomAppenders;
using com.mapcolonies.core.Services.LoggerService.Extentions;
using log4net;
using log4net.Core;
using log4net.Util;

namespace com.mapcolonies.core.Services.LoggerService
{
    public struct LogMethod
    {
        private readonly ILog _target;
        private readonly LogExt.LogType _logType;

        internal LogMethod(ILog target, LogExt.LogType logType)
        {
            _target = target;
            _logType = logType;
        }

        public void Call(string message)
        {
            if (!_target.IsEnabled(_logType)) return;

            switch (_logType)
            {
                case LogExt.LogType.Debug:
                {
                    _target.Debug(message);
                }
                    break;
                case LogExt.LogType.Info:
                {
                    _target.Info(message);
                }
                    break;
                case LogExt.LogType.Warn:
                {
                    _target.Warn(message);
                }
                    break;
                case LogExt.LogType.Error:
                {
                    _target.Error(message);
                }
                    break;
                case LogExt.LogType.Fatal:
                {
                    _target.Fatal(message);
                }
                    break;
            }
        }

        public void Call(string message, Exception exception)
        {
            if (!_target.IsEnabled(_logType)) return;

            switch (_logType)
            {
                case LogExt.LogType.Debug:
                {
                    _target.Debug(message, exception);
                }
                    break;
                case LogExt.LogType.Info:
                {
                    _target.Info(message, exception);
                }
                    break;
                case LogExt.LogType.Warn:
                {
                    _target.Warn(message, exception);
                }
                    break;
                case LogExt.LogType.Error:
                {
                    _target.Error(message, exception);
                }
                    break;
                case LogExt.LogType.Fatal:
                {
                    _target.Fatal(message, exception);
                }
                    break;
            }
        }

        public void CallFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!_target.IsEnabled(_logType)) return;

            switch (_logType)
            {
                case LogExt.LogType.Debug:
                {
                    _target.DebugFormat(formatProvider, format, args);
                }
                    break;
                case LogExt.LogType.Info:
                {
                    _target.InfoFormat(formatProvider, format, args);
                }
                    break;
                case LogExt.LogType.Warn:
                {
                    _target.WarnFormat(formatProvider, format, args);
                }
                    break;
                case LogExt.LogType.Error:
                {
                    _target.ErrorFormat(formatProvider, format, args);
                }
                    break;
                case LogExt.LogType.Fatal:
                {
                    _target.FatalFormat(formatProvider, format, args);
                }
                    break;
            }
        }

        public void CallFormat(string format, object arg0)
        {
            if (!_target.IsEnabled(_logType)) return;

            switch (_logType)
            {
                case LogExt.LogType.Debug:
                {
                    _target.DebugFormat(format, arg0);
                }
                    break;
                case LogExt.LogType.Info:
                {
                    _target.InfoFormat(format, arg0);
                }
                    break;
                case LogExt.LogType.Warn:
                {
                    _target.WarnFormat(format, arg0);
                }
                    break;
                case LogExt.LogType.Error:
                {
                    _target.ErrorFormat(format, arg0);
                }
                    break;
                case LogExt.LogType.Fatal:
                {
                    _target.FatalFormat(format, arg0);
                }
                    break;
            }
        }

        public void CallFormat(string format, object arg0, object arg1)
        {
            if (!_target.IsEnabled(_logType)) return;

            switch (_logType)
            {
                case LogExt.LogType.Debug:
                {
                    _target.DebugFormat(format, arg0, arg1);
                }
                    break;
                case LogExt.LogType.Info:
                {
                    _target.InfoFormat(format, arg0, arg1);
                }
                    break;
                case LogExt.LogType.Warn:
                {
                    _target.WarnFormat(format, arg0, arg1);
                }
                    break;
                case LogExt.LogType.Error:
                {
                    _target.ErrorFormat(format, arg0, arg1);
                }
                    break;
                case LogExt.LogType.Fatal:
                {
                    _target.FatalFormat(format, arg0, arg1);
                }
                    break;
            }
        }

        public void CallFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!_target.IsEnabled(_logType)) return;

            switch (_logType)
            {
                case LogExt.LogType.Debug:
                {
                    _target.DebugFormat(format, arg0, arg1, arg2);
                }
                    break;
                case LogExt.LogType.Info:
                {
                    _target.InfoFormat(format, arg0, arg1, arg2);
                }
                    break;
                case LogExt.LogType.Warn:
                {
                    _target.WarnFormat(format, arg0, arg1, arg2);
                }
                    break;
                case LogExt.LogType.Error:
                {
                    _target.ErrorFormat(format, arg0, arg1, arg2);
                }
                    break;
                case LogExt.LogType.Fatal:
                {
                    _target.FatalFormat(format, arg0, arg1, arg2);
                }
                    break;
            }
        }

        public void CallFormat(string format, params object[] args)
        {
            if (!_target.IsEnabled(_logType)) return;

            switch (_logType)
            {
                case LogExt.LogType.Debug:
                {
                    _target.DebugFormat(format, args);
                }
                    break;
                case LogExt.LogType.Info:
                {
                    _target.InfoFormat(format, args);
                }
                    break;
                case LogExt.LogType.Warn:
                {
                    _target.WarnFormat(format, args);
                }
                    break;
                case LogExt.LogType.Error:
                {
                    _target.ErrorFormat(format, args);
                }
                    break;
                case LogExt.LogType.Fatal:
                {
                    _target.FatalFormat(format, args);
                }
                    break;
            }
        }

        private Level GetLevel()
        {
            switch (_logType)
            {
                case LogExt.LogType.Debug:
                    return Level.Debug;
                case LogExt.LogType.Info:
                    return Level.Info;
                case LogExt.LogType.Warn:
                    return Level.Warn;
                case LogExt.LogType.Error:
                    return Level.Error;
                case LogExt.LogType.Fatal:
                    return Level.Fatal;
            }

            return Level.Verbose;
        }

        public void CallFormat(UnityEngine.Object ctx, string format, params object[] args)
        {
            if (!_target.IsEnabled(_logType)) return;
            var evt = new LoggingEvent(ThisDeclaringType, _target.Logger.Repository, _target.Logger.Name, GetLevel(), new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            if (ctx != null)
                evt.Properties[ConsoleAppender.UnityContext] = ctx;
            _target.Logger.Log(evt);
        }

        public void Call(UnityEngine.Object ctx, string msg)
        {
            if (!_target.IsEnabled(_logType)) return;
            var evt = new LoggingEvent(ThisDeclaringType, _target.Logger.Repository, _target.Logger.Name, GetLevel(), msg, null);
            if (ctx != null)
                evt.Properties[ConsoleAppender.UnityContext] = ctx;
            _target.Logger.Log(evt);
        }

        public void Call(UnityEngine.Object ctx, string msg, Exception e)
        {
            if (!_target.IsEnabled(_logType)) return;
            var evt = new LoggingEvent(ThisDeclaringType, _target.Logger.Repository, _target.Logger.Name, GetLevel(), msg, e);
            if (ctx != null)
                evt.Properties[ConsoleAppender.UnityContext] = ctx;
            _target.Logger.Log(evt);
        }

        private static readonly Type ThisDeclaringType = typeof(LogImpl);
    }
}
