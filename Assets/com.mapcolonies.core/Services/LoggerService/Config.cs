namespace com.mapcolonies.core.Services.LoggerService
{
    [System.Serializable]
    public class Config
    {
        public string Log4NetConfigXml;
        public int StackTraceRowLimit;
        public bool Enabled;
        public bool ConsoleEnabled;
        public string MinLogLevel;
        public string MinFileLogLevel;
        public string MinHttpLogLevel;
        public bool ForceMinLogLevel;
        public string HttpEndpointUrl;
        public string HttpPersistenceDirectory;
    }
}
