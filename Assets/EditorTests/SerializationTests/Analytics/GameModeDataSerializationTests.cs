using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;

namespace EditorTests.SerializationTests.Analytics
{
    public class GameModeDataSerializationTests
    {
        [Test]
        public void GameModeData_Serializes_Expected_Fields()
        {
            GameModeData data = GameModeData.Create("MissionPlanning", "TopDown");
            SerializationInfo info = new SerializationInfo(typeof(GameModeData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual("MissionPlanning", info.GetString(nameof(GameModeData.Mode)));
            Assert.AreEqual("TopDown", info.GetString(nameof(GameModeData.ViewMode)));
        }
    }
}
