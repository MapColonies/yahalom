using System;
using UnityEngine;

namespace com.mapcolonies.yahalom.Preloader
{
    [Serializable]
    public class PreloaderViewModel
    {
        [SerializeField] public float Progress { get; set; } = 0f;

        public PreloaderViewModel()
        {
        }
    }
}
