using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Interfaces;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    public class UserDetails : IAnalyticLogParameter
    {
        public string UserName { get; private set; }
        public string UserDomainName { get; private set; }
        public string MachineName { get; private set; }

        private UserDetails(string userName, string userDomainName, string machineName)
        {
            UserName = userName;
            UserDomainName = userDomainName;
            MachineName = machineName;
        }

        public static UserDetails Create(string userName, string userDomainName, string machineName)
        {
            return new UserDetails(userName, userDomainName, machineName);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(UserName), UserName);
            info.AddValue(nameof(UserDomainName), UserDomainName);
            info.AddValue(nameof(MachineName), MachineName);
        }
    }
}
