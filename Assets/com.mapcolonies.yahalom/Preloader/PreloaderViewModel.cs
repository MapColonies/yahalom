using System;
using UnityEngine;

namespace com.mapcolonies.yahalom.Preloader
{
    [Serializable]
    public class PreloaderViewModel
    {
        [SerializeField] public String Name { get; private set; }
        [SerializeField] public float Progress { get; private set; } = 0f;

        public PreloaderViewModel()
        {
        }

        public void ReportProgress(string name, float progress)
        {
            Name = name;
            Progress = progress;
        }
    }
}
