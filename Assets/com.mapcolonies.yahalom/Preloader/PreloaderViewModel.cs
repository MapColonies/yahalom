using System;
using UnityEngine;

namespace com.mapcolonies.yahalom.Preloader
{
    [Serializable]
    public class PreloaderViewModel : IDisposable
    {
        public string Name
        {
            get;
            private set;
        }

        public float Progress
        {
            get;
            private set;
        }

        public void Dispose()
        {
        }

        public void ReportProgress(string name, float progress)
        {
            Name = name;
            Progress = progress;

            Debug.Log($"Name: {name} Progress: {progress}");
        }
    }
}
