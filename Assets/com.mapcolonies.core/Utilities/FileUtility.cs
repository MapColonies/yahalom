using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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

        public static async Task AppendLineToFileAsync(string line, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("File path is not set, skipping file write.");
                return;
            }

            await File.AppendAllTextAsync(filePath, line + Environment.NewLine);
        }
    }
}
