using System.Runtime.Serialization;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    [System.Serializable]
    public class UserInputDevicesData : IAnalyticLogParameter
    {
        public string[] InputDevices
        {
            get;
            private set;
        }

        private UserInputDevicesData(string[] inputDevices)
        {
            InputDevices = inputDevices;
        }

        public static UserInputDevicesData Create(string[] inputDevices)
        {
            return new UserInputDevicesData(inputDevices);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(InputDevices), InputDevices);
        }
    }
}
