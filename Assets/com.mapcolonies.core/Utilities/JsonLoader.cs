using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace com.mapcolonies.core.Utilities
{
    public static class JsonLoader
    {
        public static async UniTask<T> LoadStreamingAssetsJsonAsync<T>(string relativePath)
        {
            string path = Path.Combine(Application.streamingAssetsPath, relativePath);
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

            using var request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new IOException($"Failed to load remote JSON from {url}: {request.error}");

            return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        }
    }
}
