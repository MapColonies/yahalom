using System.IO;
using UnityEngine;

namespace com.mapcolonies.core.Services.LoggerService
{
    public class LoggerServiceConfig
    {
        private const string JsonFileName = "Logger/LoggerConfig.json";

        private Config _config;

        public string Log4NetConfigXml
        {
            get;
            private set;
        }

        public bool ServiceEnabled
        {
            get;
            private set;
        }

        public bool EnableConsole
        {
            get;
            private set;
        }

        public string MinLogLevel
        {
            get;
            private set;
        }

        public string HttpEndpointUrl
        {
            get;
            private set;
        }

        public string HttpPersistenceDirectory
        {
            get;
            private set;
        }

        public void Init()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, JsonFileName);

            if (!File.Exists(filePath))
            {
                Debug.LogError($"File {JsonFileName} not found at: " + filePath);
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(filePath);
                _config = JsonUtility.FromJson<Config>(jsonContent);

                if (_config == null)
                {
                    Debug.LogError($"Failed to deserialize {JsonFileName} JSON content.");
                    return;
                }

                Log4NetConfigXml = _config.Log4NetConfigXml;
                ServiceEnabled = _config.Enabled;
                EnableConsole = _config.ConsoleEnabled;
                MinLogLevel = _config.MinLogLevel;
                HttpEndpointUrl = _config.HttpEndpointUrl;
                HttpPersistenceDirectory = _config.HttpPersistenceDirectory;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error parsing file: {ex.Message}");
            }
        }

        public string GetSystemLogsDirectory()
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), Application.companyName, Application.productName, "logs");
            }

            return Application.persistentDataPath;
        }

        public string GetHttpPersistenceDirectory()
        {
            string baseLogsDirectory = GetSystemLogsDirectory();
            return Path.Combine(baseLogsDirectory, "offline");
        }
    }
}
