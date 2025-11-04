using System;
using UnityEngine;

namespace com.mapcolonies.yahalom.Preloader
{
    [Serializable]
    public class PreloaderViewModel : IDisposable
    {
        [SerializeField]
        public string Name
        {
            get;
            private set;
        }

        [SerializeField]
        public float Progress
        {
            get;
            private set;
        }

        public virtual void ReportProgress(string name, float progress)
        {
            Name = name;
            Progress = progress;

            Debug.Log($"Name: {name} Progress: {progress}");
        }

        public void Dispose()
        {
        }
    }
}
