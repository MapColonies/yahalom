using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using com.mapcolonies.core.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Utilities
{
    public class FileUtilityPlayModeTests
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
            var path = FileUtility.GetFullPath(dirName, "minimal.log");

            Assert.IsNotNull(path);
            Assert.IsTrue(Directory.Exists(Path.Combine(Application.persistentDataPath, dirName)), "Directory should be created");
            StringAssert.EndsWith("minimal.log", path.Replace('\\', '/'));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Append_Writes_Single_Line()
        {
            var dir = FileUtility.GetFullPath("FileUtility_Append");
            var file = Path.Combine(dir, $"session-{Guid.NewGuid():N}.log");

            yield return Await(FileUtility.AppendLineToFileAsync("hello", file));

            Assert.IsTrue(File.Exists(file), "File should be created");
            StringAssert.Contains("hello", File.ReadAllText(file));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Append_Skips_When_FilePath_Null()
        {
            var dirName = "FileUtility_Skip";
            var dir = Path.Combine(Application.persistentDataPath, dirName);

            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }

            string nullPath = null;
            yield return Await(FileUtility.AppendLineToFileAsync("ignored", nullPath));

            Assert.IsFalse(Directory.Exists(dir), "Directory should not be created when path is null");
            yield return null;
        }
    }
}
