using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Interfaces;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    [System.Serializable]
    public class UserInputDevices : IAnalyticLogParameter
    {
        public string[] InputDevices { get; private set; }

        private UserInputDevices(string[] inputDevices)
        {
            InputDevices = inputDevices;
        }

        public static UserInputDevices Create(string[] inputDevices)
        {
            return new UserInputDevices(inputDevices);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(InputDevices), InputDevices);
        }
    }
}
