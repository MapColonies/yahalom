using com.mapcolonies.core;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.mapcolonies.yahalom.Preloader
{
    public class PreloaderView : BaseMvvmView<PreloaderViewModel>
    {
        private const int FadeMs = 1000;
        private VisualElement _preloaderRoot;

        protected override void OnViewModelBound()
        {
            _preloaderRoot = RootVisualElement.Q<VisualElement>("PreloaderRoot");
            if (_preloaderRoot == null)
            {
                _preloaderRoot = RootVisualElement;
            }

            if (!_preloaderRoot.ClassListContains("preloader-root"))
            {
                _preloaderRoot.AddToClassList("preloader-root");
            }

            if (ViewModel != null)
                ViewModel.ProgressUpdated += OnProgressUpdated;
        }

        protected override void OnViewModelUnbound()
        {
            if (ViewModel != null)
                ViewModel.ProgressUpdated -= OnProgressUpdated;
        }

        private void OnProgressUpdated(string stepName, float progress)
        {
            if (progress >= 1f)
                FadeOutAndDisable();
        }

        private void FadeOutAndDisable()
        {
            if (_preloaderRoot == null)
                _preloaderRoot = RootVisualElement;

            _preloaderRoot.AddToClassList("preloader-fadeout");

            _preloaderRoot.schedule.Execute(() =>
            {
                gameObject.SetActive(false);
            }).StartingIn(FadeMs);
        }

        private void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
