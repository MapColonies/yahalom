using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace com.mapcolonies.core.Services.LoggerService
{
    public class LoggerServiceConfig
    {
        private const string JsonFileName = "Logger/LoggerConfig.json";

        public LoggerSettings Settings { get; private set; }

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
                Settings = JsonConvert.DeserializeObject<LoggerSettings>(jsonContent);
                if (Settings == null)
                {
                    Debug.LogError($"Failed to deserialize {JsonFileName} JSON content.");
                }
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
