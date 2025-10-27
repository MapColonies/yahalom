using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.mapcolonies.core.Utilities
{
    public static class StreamingAssetsLoader
    {
        public static async UniTask<string> LoadTextAsync(string relativePath)
        {
            string path = Path.Combine(Application.streamingAssetsPath, relativePath);

            #if UNITY_ANDROID && !UNITY_EDITOR
                    using var request = UnityWebRequest.Get(path);
                    await request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success)
                        throw new IOException($"Failed to load {relativePath}: {request.error}");
                    return request.downloadHandler.text;
            #else
                    if (!File.Exists(path))
                        throw new FileNotFoundException($"File not found: {path}");

                    using StreamReader reader = new StreamReader(path);
                    return await reader.ReadToEndAsync();
            #endif
        }
    }
}
