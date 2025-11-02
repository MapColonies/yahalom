#nullable enable
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace com.mapcolonies.core.Utilities
{
    public static class JsonSaver
    {
        public static async UniTask SavePersistentJsonAsync<T>(string relativePath, T data)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);
            await SaveLocalJsonAsync(path, data);
        }

        public static async UniTask SaveLocalJsonAsync<T>(string path, T data)
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            using StreamWriter writer = new StreamWriter(path, false);
            await writer.WriteAsync(json);
        }
    }
}
