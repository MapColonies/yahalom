using com.mapcolonies.core;

namespace com.mapcolonies.yahalom.Preloader
{
    public class PreloaderView : BaseMvvmView<PreloaderViewModel>
    {
        private void OnEnable()
        {
            if (ViewModel != null)
            {
                ViewModel.ProgressUpdated += OnProgressUpdated;
            }
        }

        private void OnDisable()
        {
            if (ViewModel != null)
            {
                ViewModel.ProgressUpdated -= OnProgressUpdated;
            }
        }

        private void OnProgressUpdated(string stepName, float progress)
        {
            if (progress >= 1f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
