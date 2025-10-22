using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Interfaces;
using UnityEngine;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    [System.Serializable]
    public class UserPlatformData : IAnalyticLogParameter
    {
        public string OperatingSystem { get; private set; }
        public string ProcessorType { get; private set; }
        public string GraphicsDeviceType { get; private set; }
        public int Ram { get; private set; }
        public string GraphicsDeviceName { get; private set; }
        public string GraphicsDeviceVersion { get; private set; }
        public int GraphicsMemorySize { get; private set; }
        public Resolution ScreenResolution { get; private set; }
        public string DeviceModel { get; private set; }

        private UserPlatformData(string operatingSystem, string processorType, string graphicsDeviceType, int ram,
            string graphicsDeviceName, string graphicsDeviceVersion, int graphicsMemorySize, Resolution screenResolution,
            string deviceModel)
        {
            OperatingSystem = operatingSystem;
            ProcessorType = processorType;
            GraphicsDeviceType = graphicsDeviceType;
            Ram = ram;
            GraphicsDeviceName = graphicsDeviceName;
            GraphicsDeviceVersion = graphicsDeviceVersion;
            GraphicsMemorySize = graphicsMemorySize;
            ScreenResolution = screenResolution;
            DeviceModel = deviceModel;
        }

        public static UserPlatformData Create(string operatingSystem, string processorType, string graphicsDeviceType,
            int ram,
            string graphicsDeviceName, string graphicsDeviceVersion, int graphicsMemorySize, Resolution screenResolution,
            string deviceModel)
        {
            return new UserPlatformData(operatingSystem,
                processorType,
                graphicsDeviceType,
                ram,
                graphicsDeviceName,
                graphicsDeviceVersion,
                graphicsMemorySize, screenResolution, deviceModel
            );
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(OperatingSystem), OperatingSystem);
            info.AddValue(nameof(ProcessorType), ProcessorType);
            info.AddValue(nameof(GraphicsDeviceType), GraphicsDeviceType);
            info.AddValue(nameof(Ram), Ram);
            info.AddValue(nameof(GraphicsDeviceName), GraphicsDeviceName);
            info.AddValue(nameof(GraphicsDeviceVersion), GraphicsDeviceVersion);
            info.AddValue(nameof(ScreenResolution), ScreenResolution);
            info.AddValue(nameof(DeviceModel), DeviceModel);
        }
    }
}
