using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;

namespace EditorTests.SerializationTests.Analytics
{
    public class UserInputDevicesSerializationTests
    {
        [Test]
        public void UserInputDevices_Serializes_Expected_Fields()
        {
            UserInputDevicesData devs = UserInputDevicesData.Create(new[] { "Keyboard", "Mouse" });
            SerializationInfo info = new SerializationInfo(typeof(UserInputDevicesData), new FormatterConverter());
            devs.GetObjectData(info, new StreamingContext());

            string[] stored = (string[])info.GetValue(nameof(UserInputDevicesData.InputDevices), typeof(string[]));
            CollectionAssert.AreEqual(new[] { "Keyboard", "Mouse" }, stored);
        }
    }
}
