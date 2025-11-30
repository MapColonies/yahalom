using System.IO;
using com.mapcolonies.core.Utilities;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace com.mapcolonies.core.Services.LoggerService
{
    public class LoggerServiceConfig
    {
        private const string JsonFileName = "Logger/LoggerConfig.json";

        public LoggerSettings Settings
        {
            get;
            private set;
        }

        public void Init()
        {
            InitAsync().Forget();
        }

        private async UniTask InitAsync()
        {
            try
            {
                Settings = await JsonUtilityEx.LoadJsonAsync<LoggerSettings>(JsonFileName, FileLocation.StreamingAssets);

                if (Settings == null)
                {
                    Debug.LogError($"Failed to deserialize {JsonFileName} JSON content.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error loading {JsonFileName}: {ex.Message}");
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
