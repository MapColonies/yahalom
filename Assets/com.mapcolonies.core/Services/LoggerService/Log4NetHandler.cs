using System;
using System.Collections.Generic;
using com.mapcolonies.core.Services.LoggerService.Extentions;
using log4net;
using UnityEngine;
using Object = UnityEngine.Object;

namespace com.mapcolonies.core.Services.LoggerService
{
    public class Log4NetHandler : ILogHandler
    {
        private const string DebugLogText = "UnityEngine.Debug.Log";
        private const string PrefixText = "  at ";
        private const int NotFound = -1;

        public static Version UnityVersion;
        public static string ApplicationDataPath;

        private static readonly ILog CommonLogger = LogManager.GetLogger("Unity");
        private static readonly Dictionary<Type, ILog> Loggers = new Dictionary<Type, ILog>();

        private static ILog GetLogger(Object context)
        {
            if (!context) return CommonLogger;
            Type type = context.GetType();

            if (Loggers.TryGetValue(type, out ILog typedLog) && typedLog != null)
            {
                return typedLog;
            }

            typedLog = LogManager.GetLogger(type);
            Loggers[type] = typedLog;

            return typedLog;
        }

        private ILog GetLogger(string contextType)
        {
            if (string.IsNullOrEmpty(contextType)) return CommonLogger;
            Type type = Type.GetType(contextType);

            if (type == null)
            {
                return CommonLogger;
            }

            if (Loggers.TryGetValue(type, out ILog typedLog) && typedLog != null)
            {
                return typedLog;
            }

            typedLog = LogManager.GetLogger(type);
            Loggers[type] = typedLog;

            return typedLog;
        }

        private static string[] GetStack()
        {
            string[] stack = Environment.StackTrace.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            int foundAt = NotFound;

            for (int i = 0; i < stack.Length; i++)
            {
                if (stack[i].Substring(PrefixText.Length, DebugLogText.Length) != DebugLogText) continue;

                foundAt = i;
                break;
            }

            if (foundAt <= NotFound || foundAt >= stack.Length)
            {
                return Array.Empty<string>();
            }

            string[] actualResult = new string[stack.Length - foundAt];

            for (int i = foundAt; i < stack.Length; i++)
            {
                actualResult[i - foundAt] = stack[i].Substring(PrefixText.Length);
            }

            return actualResult;
        }

        private string GetFullClassName(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            string[] parts = source.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 1 ? parts[0] : string.Join(".", parts, 0, parts.Length - 1);
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            ILog logger = null;

            if (!context)
            {
                string[] stack = GetStack();

                if (stack.Length == 0)
                {
                    logger = GetLogger(context);
                }
                else
                {
                    string[] parts = stack[0].Split(" ");

                    if (parts.Length > 0)
                    {
                        logger = GetLogger(GetFullClassName(parts[0]));
                    }
                }
            }
            else
            {
                logger = GetLogger(context);
            }

            LogMethod? method = null;

            switch (logType)
            {
                case LogType.Assert:
                case LogType.Exception:
                case LogType.Error:
                    method = logger.Error();
                    break;
                case LogType.Warning:
                    method = logger.Warn();
                    break;
                case LogType.Log:
                    method = logger.Info();
                    break;
            }

            if (args?.Length > 0)
            {
                method?.CallFormat(context, format, args);
            }
            else
            {
                method?.Call(context, format);
            }
        }

        public void LogException(Exception exception, Object context)
        {
            ILog logger = GetLogger(context);

            if (exception == null)
            {
                return;
            }

            logger.Error()?.Call(context, exception.UnityMessageWithStack(), exception);
        }
    }
}
