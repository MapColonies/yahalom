using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Interfaces;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    public class GameModeData : IAnalyticLogParameter
    {
        public string Mode { get; private set; }

        public string ViewMode { get; private set; }


        private GameModeData(string mode, string playerViewMode)
        {
            Mode = mode;
            ViewMode = playerViewMode;
        }

        public static GameModeData Create(string mode, string playerViewMode)
        {
            return new GameModeData(mode, playerViewMode);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Mode), Mode);
            info.AddValue(nameof(ViewMode), ViewMode);
        }
    }
}
