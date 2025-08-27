using UnityEngine;

namespace com.mapcolonies.core
{
    public abstract class BaseMvvmView<T> : MonoBehaviour where T : BaseMvvmViewModel
    {
    }
}