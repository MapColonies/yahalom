using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using com.mapcolonies.core.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Utilities
{
    public class FileUtilityPlayModeTests : MonoBehaviour
    {
        private static IEnumerator Await(Task t)
        {
            while (!t.IsCompleted) yield return null;
            if (t.IsFaulted) throw t.Exception;
        }

        [UnityTest]
        public IEnumerator SetupFilePath_Creates_Directory_And_FilePath()
        {
            var dirName = "FileUtility_Setup";
            var path = FileUtility.SetupFilePath(dirName, "minimal.log");

            Assert.IsNotNull(path);
            Assert.IsTrue(Directory.Exists(Path.Combine(Application.persistentDataPath, dirName)), "Directory should be created");
            StringAssert.EndsWith("minimal.log", path.Replace('\\', '/'));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Append_Writes_Single_Line()
        {
            var dir = FileUtility.SetupFilePath("FileUtility_Append");
            var file = Path.Combine(dir, $"session-{Guid.NewGuid():N}.log");

            using (var sem = new SemaphoreSlim(1, 1))
            {
                yield return Await(FileUtility.AppendLineToFileSafeAsync("hello", file, sem));
            }

            Assert.IsTrue(File.Exists(file), "File should be created");
            StringAssert.Contains("hello", File.ReadAllText(file));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Append_Skips_When_FilePath_Null()
        {
            var dir = FileUtility.SetupFilePath("FileUtility_Skip");
            var file = Path.Combine(dir, $"session-{Guid.NewGuid():N}.log");

            using (var sem = new SemaphoreSlim(1, 1))
            {
                string nullPath = null;
                yield return Await(FileUtility.AppendLineToFileSafeAsync("ignored", nullPath, sem));
            }

            Assert.IsFalse(Directory.Exists(dir) && File.Exists(file), "No file should be created when filePath is null");
            yield return null;
        }
    }
}
