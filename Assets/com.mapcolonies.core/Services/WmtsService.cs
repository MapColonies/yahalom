using Cysharp.Threading.Tasks;

namespace com.mapcolonies.core.Services
{
    public class WmtsService
    {
        public UniTask Init()
        {
            return UniTask.Delay(400);
        }
    }
}