using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace com.mapcolonies.core.Utilities
{
    public class FileUtility
    {
        public static string GetFullPath(string folderName, string fileName = null)
        {
            try
            {
                string directoryPath = Path.Combine(Application.persistentDataPath, folderName);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Debug.Log($"Created directory: {directoryPath}");
                }

                if (string.IsNullOrEmpty(fileName))
                {
                    Debug.Log($"Directory ready: {directoryPath}");
                    return directoryPath;
                }

                string filePath = Path.Combine(directoryPath, fileName);
                Debug.Log($"File path ready: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to prepare file path: {ex.Message}");
                return null;
            }
        }

        public static async UniTask AppendLineToFileAsync(string line, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("File path is not set, skipping file write.");
                return;
            }

            await File.AppendAllTextAsync(filePath, line + Environment.NewLine);
        }

        public static async UniTask<T> LoadStreamingAssetsJsonAsync<T>(string relativePath)
        {
            string path = Path.Combine(Application.streamingAssetsPath, relativePath);
            return await LoadLocalJsonAsync<T>(path);
        }

        public static async UniTask<T> LoadPersistentJsonAsync<T>(string relativePath)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);
            return await LoadLocalJsonAsync<T>(path);
        }

        public static async UniTask<T> LoadLocalJsonAsync<T>(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            using StreamReader reader = new StreamReader(path);
            string json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async UniTask<T> LoadRemoteJsonAsync<T>(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty.", nameof(url));

            using UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new IOException($"Failed to load remote JSON from {url}: {request.error}");

            return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        }

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
            await using StreamWriter writer = new StreamWriter(path, false);
            await writer.WriteAsync(json);
        }

        public static async UniTask<bool> DoesPersistentJsonExistAsync(string relativePath)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);
            return await UniTask.RunOnThreadPool(() => File.Exists(path));
        }
    }
}
