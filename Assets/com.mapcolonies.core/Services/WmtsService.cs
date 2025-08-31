using Cysharp.Threading.Tasks;

namespace com.mapcolonies.core.Services
{
    public class WmtsService
    {
        public UniTask Init()
        {
            // TODO: Implement the real initialization of the WMTS service.
            return UniTask.Delay(400);
        }
    }
}