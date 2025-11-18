using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace com.mapcolonies.core.Utilities
{
    public enum FileLocation
    {
        StreamingAssets = 0,
        PersistentData = 1
    }

    public static class JsonUtilityEx
    {
        public static async UniTask<T> FromJsonFileAsync<T>(string path)
        {
            string json = await FileIOUtility.ReadTextFileAsync(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async UniTask SaveJsonToFileAsync<T>(string path, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await FileIOUtility.WriteTextFileAsync(path, json);
        }

        public static async UniTask<T> LoadRemoteJsonAsync<T>(string url)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new IOException($"Failed to load remote JSON: {request.error}");

            return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        }


        public static async UniTask<T> LoadJsonAsync<T>(string relativePath, FileLocation location = FileLocation.StreamingAssets)
        {
            string path = location switch
            {
                FileLocation.PersistentData => Path.Combine(Application.streamingAssetsPath, relativePath),
                FileLocation.StreamingAssets => Path.Combine(Application.streamingAssetsPath, relativePath),
                _ => Path.Combine(Application.streamingAssetsPath, relativePath)
            };

            return await FromJsonFileAsync<T>(path);
        }

        public static async UniTask SaveJsonAsync<T>(string relativePath, T data)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);
            await SaveJsonToFileAsync(path, data);
        }

        public static async UniTask<bool> DoesJsonExistAsync(string relativePath, FileLocation location = FileLocation.PersistentData)
        {
            string path = location switch
            {
                FileLocation.PersistentData => Path.Combine(Application.streamingAssetsPath, relativePath),
                FileLocation.StreamingAssets => Path.Combine(Application.streamingAssetsPath, relativePath),
                _ => Path.Combine(Application.streamingAssetsPath, relativePath)
            };

            return await FileIOUtility.FileExistsAsync(path);
        }
    }
}
