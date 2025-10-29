using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace com.mapcolonies.core.Utilities
{
    public class FileUtility : MonoBehaviour
    {
        public static string SetupFilePath(string folderName, string fileName = null)
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

        public static async Task AppendLineToFileSafeAsync(string line, string filePath, SemaphoreSlim fileSemaphore)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("File path is not set, skipping file write.");
                return;
            }

            if (fileSemaphore == null)
            {
                Debug.LogError($"Semaphore is null for file {filePath}. Write operation is not thread-safe and was skipped.");
                return;
            }

            await fileSemaphore.WaitAsync();

            try
            {
                await File.AppendAllTextAsync(filePath, line + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write to file {filePath}: {ex.Message}");
            }
            finally
            {
                fileSemaphore.Release();
            }
        }
    }
}
