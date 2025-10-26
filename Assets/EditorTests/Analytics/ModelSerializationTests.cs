using System;
using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.Analytics
{
    public class ModelSerializationTests
    {
        [Test]
        public void LayerData_Serializes_Expected_Fields()
        {
            LayerData data = LayerData.Create("elevation", "lyr-42");
            SerializationInfo info = new SerializationInfo(typeof(LayerData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual("elevation", info.GetString(nameof(LayerData.LayerDomain)));
            Assert.AreEqual("lyr-42", info.GetString(nameof(LayerData.UniqueLayerId)));
        }

        [Test]
        public void LocationData_Serializes_Expected_Fields()
        {
            LocationData data = LocationData.Create(34.5, 31.7);
            SerializationInfo info = new SerializationInfo(typeof(LocationData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual(34.5, info.GetDouble(nameof(LocationData.Longitude)));
            Assert.AreEqual(31.7, info.GetDouble(nameof(LocationData.Latitude)));
        }

        [Test]
        public void ApplicationData_Serializes_Expected_Fields()
        {
            ApplicationData data = ApplicationData.Create("Yahalom", "1.2.3");
            SerializationInfo info = new SerializationInfo(typeof(ApplicationData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual("Yahalom", info.GetString(nameof(ApplicationData.ApplicationName)));
            Assert.AreEqual("1.2.3", info.GetString(nameof(ApplicationData.ApplicationVersion)));
        }

        [Test]
        public void GameModeData_Serializes_Expected_Fields()
        {
            GameModeData data = GameModeData.Create("MissionPlanning", "TopDown");
            SerializationInfo info = new SerializationInfo(typeof(GameModeData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual("MissionPlanning", info.GetString(nameof(GameModeData.Mode)));
            Assert.AreEqual("TopDown", info.GetString(nameof(GameModeData.ViewMode)));
        }

        [Test]
        public void PerformanceData_Serializes_Expected_Fields()
        {
            PerformanceData data = PerformanceData.Create(58.9f, 1024.5, 23.3);
            SerializationInfo info = new SerializationInfo(typeof(PerformanceData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual(58.9f, info.GetSingle(nameof(PerformanceData.Fps)));
            Assert.AreEqual(1024.5, info.GetDouble(nameof(PerformanceData.AllocatedMemoryInMB)));
            Assert.AreEqual(23.3, info.GetDouble(nameof(PerformanceData.CpuUsagePercentage)));
        }

        [Test]
        public void UserInputDevices_Serializes_Expected_Fields()
        {
            UserInputDevicesData devs = UserInputDevicesData.Create(new[] {"Keyboard", "Mouse"});
            SerializationInfo info = new SerializationInfo(typeof(UserInputDevicesData), new FormatterConverter());
            devs.GetObjectData(info, new StreamingContext());

            string[] stored = (string[])info.GetValue(nameof(UserInputDevicesData.InputDevices), typeof(string[]));
            CollectionAssert.AreEqual(new[] {"Keyboard", "Mouse"}, stored);
        }

        [Test]
        public void UserDetails_Serializes_Expected_Fields()
        {
            UserDetailsData detailsData = UserDetailsData.Create("username", "MAPCO", "DEVPC01");
            SerializationInfo info = new SerializationInfo(typeof(UserDetailsData), new FormatterConverter());
            detailsData.GetObjectData(info, new StreamingContext());

            Assert.AreEqual("username", info.GetString(nameof(UserDetailsData.UserName)));
            Assert.AreEqual("MAPCO", info.GetString(nameof(UserDetailsData.UserDomainName)));
            Assert.AreEqual("DEVPC01", info.GetString(nameof(UserDetailsData.MachineName)));
        }

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
            // Unity's Resolution struct is serialized as a whole; existence is enough.
            Assert.DoesNotThrow(() => info.GetValue(nameof(UserPlatformData.ScreenResolution), typeof(Resolution)));
        }

        [Test]
        public void ApplicationUsageData_Serializes_Expected_Fields()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(123.45);
            ApplicationUsageData data = ApplicationUsageData.Create(timeSpan);
            SerializationInfo info = new SerializationInfo(typeof(ApplicationUsageData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual(123.45, info.GetDouble(nameof(ApplicationUsageData.Time)));
        }
    }
}
