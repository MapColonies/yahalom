using System;
using R3;
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

        public ReactiveProperty<bool> Hidden { get; private set; } = new ReactiveProperty<bool>(false);

        public virtual void ReportProgress(string name, float progress)
        {
            Name = name;
            Progress = progress;

            Debug.Log($"Name: {name} Progress: {progress}");
        }

        public void Hide() => Hidden.Value = true;
        public void Show() => Hidden.Value = false;

        public void Dispose()
        {
            Hidden?.Dispose();
        }
    }
}
