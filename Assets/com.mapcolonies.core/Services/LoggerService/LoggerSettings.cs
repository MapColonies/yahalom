using System;

namespace com.mapcolonies.core.Services.LoggerService
{
    [Serializable]
    public record LoggerSettings
    {
        public string Log4NetConfigXml
        {
            get;
            set;
        }

        public int StackTraceRowLimit
        {
            get;
            set;
        }

        public bool ServiceEnabled
        {
            get;
            set;
        }

        public bool ConsoleEnabled
        {
            get;
            set;
        }

        public string MinConsoleLogLevel
        {
            get;
            set;
        }

        public string MinFileLogLevel
        {
            get;
            set;
        }

        public string MinHttpLogLevel
        {
            get;
            set;
        }

        public bool ForceMinLogLevel
        {
            get;
            set;
        }

        public string HttpEndpointUrl
        {
            get;
            set;
        }

        public string HttpPersistenceDirectory
        {
            get;
            set;
        }
    }
}
