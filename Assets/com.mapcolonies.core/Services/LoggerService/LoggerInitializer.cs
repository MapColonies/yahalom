using System;
using System.Threading;
using UnityEngine;

namespace com.mapcolonies.core.Services.LoggerService
{
    public static class LoggerInitializer
    {
        private const string AdditionalRevisionSeparator = "f";
        private static LoggerService Logger;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Init()
        {
            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                Thread.CurrentThread.Name = "main";
            }

            Log4NetHandler.ApplicationDataPath = Application.dataPath;
            Log4NetHandler.UnityVersion = GetVersion();

            LoggerServiceConfig config = new LoggerServiceConfig();
            config.Init();

            Logger = new LoggerService(config);

            Application.quitting -= Dispose;
            Application.quitting += Dispose;
        }

        private static Version GetVersion()
        {
            Version version;

            if (Version.TryParse(Application.unityVersion.Split(AdditionalRevisionSeparator)[0], out Version parsedVersion))
            {
                version = parsedVersion;
            }
            else
            {
                version = new Version();
            }

            return version;
        }

        private static void Dispose()
        {
            Logger?.Dispose();
        }
    }
}
