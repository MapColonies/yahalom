using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.Analytics.SerializationTests
{
    public class UserPlatformDataSerializationTests
    {
        [Test]
        public void UserPlatformData_Serializes_Expected_Fields()
        {
            Resolution res = new Resolution { width = 1920, height = 1080, refreshRate = 60 };
            UserPlatformData data = UserPlatformData.Create("Windows 11", "Intel", "Direct3D11", 32768,
                "RTX 3080", "555.12", 10240, res, "CustomPC");
            SerializationInfo info = new SerializationInfo(typeof(UserPlatformData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual("Windows 11", info.GetString(nameof(UserPlatformData.OperatingSystem)));
            Assert.AreEqual("Intel", info.GetString(nameof(UserPlatformData.ProcessorType)));
            Assert.AreEqual("Direct3D11", info.GetString(nameof(UserPlatformData.GraphicsDeviceType)));
            Assert.AreEqual(32768, info.GetInt32(nameof(UserPlatformData.Ram)));
            Assert.AreEqual("RTX 3080", info.GetString(nameof(UserPlatformData.GraphicsDeviceName)));
            Assert.AreEqual("555.12", info.GetString(nameof(UserPlatformData.GraphicsDeviceVersion)));
            Assert.AreEqual("CustomPC", info.GetString(nameof(UserPlatformData.DeviceModel)));
            Assert.DoesNotThrow(() => info.GetValue(nameof(UserPlatformData.ScreenResolution), typeof(Resolution)));
        }
    }
}
