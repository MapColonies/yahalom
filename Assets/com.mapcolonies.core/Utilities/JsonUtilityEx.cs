using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace com.mapcolonies.core.Utilities
{
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

        public static async UniTask<T> LoadStreamingAssetsJsonAsync<T>(string relativePath)
        {
            string path = Path.Combine(Application.streamingAssetsPath, relativePath);
            return await FromJsonFileAsync<T>(path);
        }

        public static async UniTask<T> LoadPersistentJsonAsync<T>(string relativePath)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);
            return await FromJsonFileAsync<T>(path);
        }

        public static async UniTask SavePersistentJsonAsync<T>(string relativePath, T data)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);
            await SaveJsonToFileAsync(path, data);
        }

        public static async UniTask<bool> DoesPersistentJsonExistAsync(string relativePath)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);
            return await FileIOUtility.FileExistsAsync(path);
        }
    }
}
