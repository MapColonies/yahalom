using System.Runtime.Serialization;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    public class LocationData : IAnalyticLogParameter
    {
        public double Longitude
        {
            get;
            private set;
        }

        public double Latitude
        {
            get;
            private set;
        }

        private LocationData(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public static LocationData Create(double longitude, double latitude)
        {
            return new LocationData(longitude, latitude);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Longitude), Longitude);
            info.AddValue(nameof(Latitude), Latitude);
        }
    }
}
