using System;
using com.mapcolonies.core.Services.Analytics.Enums;
using com.mapcolonies.core.Services.Analytics.Managers;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.Analytics
{
    public class LogObjectTests
    {
        [Test]
        public void Create_Sets_All_Fields()
        {
            long fakeSession = 123456789L;
            AnalyticsManager.SessionId = fakeSession;
            LogType severity = LogType.Warning;
            string message = "LayerUseStarted";
            LayerData parameters = LayerData.Create("imagery", "layer-001");
            string component = "General";
            AnalyticsMessageTypes type = AnalyticsMessageTypes.LayerUseStarted;
            LogObject log = LogObject.Create(severity, message, parameters, component, type);

            Assert.AreEqual(fakeSession, log.SessionID);
            Assert.AreEqual(severity.ToString(), log.Severity);
            Assert.AreEqual(message, log.Message);
            Assert.AreSame(parameters, log.MessageParameters);
            Assert.AreEqual(component, log.Component);
            Assert.AreEqual(type, log.MessageType);
            Assert.That(log.TimeStamp, Is.InRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1)));
        }
    }
}
