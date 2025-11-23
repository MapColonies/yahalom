using com.mapcolonies.core;
using R3;

namespace com.mapcolonies.yahalom.Preloader
{
    public class PreloaderView : BaseMvvmView<PreloaderViewModel>
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);

            ViewModel?.Hidden
                .DistinctUntilChanged()
                .Subscribe(hidden =>
                {
                    gameObject.SetActive(!hidden);
                })
                .AddTo(Disposables);
        }
    }
}
