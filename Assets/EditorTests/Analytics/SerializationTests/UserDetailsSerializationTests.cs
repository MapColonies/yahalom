using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;

namespace EditorTests.Analytics.SerializationTests
{
    public class UserDetailsSerializationTests
    {
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
    }
}
