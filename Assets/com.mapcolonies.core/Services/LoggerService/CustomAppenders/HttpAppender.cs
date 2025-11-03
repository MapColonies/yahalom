using System;
using System.Net.Http;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace com.mapcolonies.core.Services.LoggerService.CustomAppenders
{
    public class HttpAppender : AppenderSkeleton
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public string EndpointUrl
        {
            get;
            set;
        }

        protected override async void Append(LoggingEvent loggingEvent)
        {
            try
            {
                string logMessage = RenderLoggingEvent(loggingEvent);
                StringContent content = new StringContent(logMessage, Encoding.UTF8, "application/json");

                await _httpClient.PostAsync(EndpointUrl, content);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Error occurred in HttpLogAppender.", ex);
            }
        }

        protected override void OnClose()
        {
            _httpClient.Dispose();
            base.OnClose();
        }
    }
}
