using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace com.mapcolonies.core
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class BaseMvvmView<T> : MonoBehaviour where T : class
    {
        private T _viewModel;
        
        [SerializeField] private UIDocument _uiDocument;
        protected VisualElement RootVisualElement { get; private set; }

        [Inject]
        public void Construct(T viewModel)
        {
            Debug.Log($"Construct view for {typeof(T)}");
            
            _viewModel = viewModel;
            RootVisualElement = _uiDocument.rootVisualElement;
            RootVisualElement.dataSource = _viewModel;
        }
    }
}