using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace com.mapcolonies.core
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class BaseMvvmView<T> : MonoBehaviour where T : class
    {
        protected CompositeDisposable Disposables
        {
            get;
        } = new CompositeDisposable();

        protected T ViewModel
        {
            get;
            private set;
        }

        [SerializeField] private UIDocument _uiDocument;

        protected VisualElement RootVisualElement
        {
            get;
            private set;
        }

        [Inject]
        public void Construct(T viewModel)
        {
            Debug.Log($"Construct view for {typeof(T)}");
            ViewModel = viewModel;
        }

        private void OnEnable()
        {
            if (ViewModel == null) return;

            RootVisualElement = _uiDocument.rootVisualElement;
            RootVisualElement.dataSource = ViewModel;
        }

        private void OnDestroy()
        {
            Disposables.Dispose();
        }
    }
}
