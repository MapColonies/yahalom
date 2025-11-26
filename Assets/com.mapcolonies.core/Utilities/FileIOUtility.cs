using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.mapcolonies.core.Utilities
{
    public class FileIOUtility
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

        public static async UniTask<string> ReadTextFileAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            using StreamReader reader = new StreamReader(path);
            return await reader.ReadToEndAsync();
        }

        public static async UniTask WriteTextFileAsync(string path, string text)
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await using StreamWriter writer = new StreamWriter(path, false);
            await writer.WriteAsync(text);
        }

        public static async UniTask<bool> FileExistsAsync(string path)
        {
            return await UniTask.RunOnThreadPool(() => File.Exists(path));
        }
    }
}
