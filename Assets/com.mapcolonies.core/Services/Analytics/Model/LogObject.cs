using System;
using com.mapcolonies.core.Services.Analytics.Enums;
using com.mapcolonies.core.Services.Analytics.Managers;
using UnityEngine;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    [Serializable]
    public class LogObject
    {
        public long SessionID { get; private set; }
        public string Severity { get; private set; } //TODO: Change to Log4Net enum
        public DateTime TimeStamp { get; private set; }
        public string Message { get; private set; }
        public IAnalyticLogParameter MessageParameters { get; private set; }
        public string Component { get; private set; }
        public AnalyticsMessageTypes MessageType { get; private set; }

        private LogObject(
            long sessionId,
            string severity,
            DateTime timeStamp,
            string message,
            IAnalyticLogParameter messageParameters,
            string component,
            AnalyticsMessageTypes messageType)
        {
            SessionID = sessionId;
            Severity = severity;
            TimeStamp = timeStamp;
            Message = message;
            MessageParameters = messageParameters;
            Component = component;
            MessageType = messageType;
        }

        /// <summary>
        /// Create a LogObject type
        /// </summary>
        /// <param name="severity">Log severity</param>
        /// <param name="message">Description</param>
        /// <param name="messageParameters">Log content</param>
        /// <param name="component">Log component</param>
        /// <param name="messageType">Analytics message type</param>
        /// <returns>LogObject</returns>
        public static LogObject Create(
            LogType severity,
            string message,
            IAnalyticLogParameter messageParameters,
            string component,
            AnalyticsMessageTypes messageType)
        {
            return new LogObject(
                AnalyticsManager.SessionId,
                severity.ToString(),
                DateTime.UtcNow,
                message,
                messageParameters,
                component,
                messageType
            );
        }
    }
}
