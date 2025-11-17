using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace com.mapcolonies.core.Services.LoggerService.CustomAppenders
{
    public class HttpAppender : AppenderSkeleton, IDisposable
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly ConcurrentQueue<LoggingEvent> _queue = new ConcurrentQueue<LoggingEvent>();
        private Timer _flushTimer;
        private volatile bool _isSending;

        private long _currentPayloadSize;

        private const string OfflineFilePrefix = "offline_";
        private const string OfflineFileExtension = ".log";

        public string EndpointUrl
        {
            get;
            set;
        }

        public int MaxPayloadSize
        {
            get;
            set;
        } = 5 * 1024 * 1024;

        public int TimeThresholdMs
        {
            get;
            set;
        } = 30 * 1000;

        public string PersistenceDirectory
        {
            get;
            set;
        }

        public HttpAppender()
        {
            _flushTimer = new Timer(OnTimerFlush, null, TimeThresholdMs, TimeThresholdMs);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            string renderedMessage = RenderLoggingEvent(loggingEvent);
            long messageSize = Encoding.UTF8.GetByteCount(renderedMessage);

            loggingEvent.Fix = FixFlags.All;
            _queue.Enqueue(loggingEvent);
            Interlocked.Add(ref _currentPayloadSize, messageSize);

            if (Interlocked.Read(ref _currentPayloadSize) >= MaxPayloadSize)
            {
                SendBatch();
            }
        }

        private void OnTimerFlush(object state)
        {
            if (!_queue.IsEmpty)
            {
                SendBatch();
            }
        }

        private async void SendBatch()
        {
            if (_isSending) return;

            string batchContent = null;

            try
            {
                _isSending = true;

                _flushTimer?.Change(Timeout.Infinite, Timeout.Infinite);

                if (_queue.IsEmpty) return;

                List<string> logMessages = new List<string>();

                while (_queue.TryDequeue(out LoggingEvent loggingEvent))
                {
                    logMessages.Add(RenderLoggingEvent(loggingEvent));
                }

                Interlocked.Exchange(ref _currentPayloadSize, 0);

                if (logMessages.Count == 0) return;

                batchContent = string.Join("\n", logMessages);
                StringContent content = new StringContent(batchContent, Encoding.UTF8, "application/x-ndjson");

                var response = await _httpClient.PostAsync(EndpointUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    await SendPersistedBatchesAsync();
                }
                else
                {
                    PersistBatchSafely(batchContent);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Error occurred in HttpLogAppender.", ex);

                if (!string.IsNullOrEmpty(batchContent))
                {
                    PersistBatchSafely(batchContent);
                }
            }
            finally
            {
                _isSending = false;
                _flushTimer?.Change(TimeThresholdMs, TimeThresholdMs);
            }
        }

        private void PersistBatchSafely(string batchContent)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PersistenceDirectory))
                {
                    return;
                }

                Directory.CreateDirectory(PersistenceDirectory);

                string fileName = Path.Combine(
                    PersistenceDirectory,
                    $"{OfflineFilePrefix}{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}{OfflineFileExtension}"
                );

                File.WriteAllText(fileName, batchContent, Encoding.UTF8);
            }
            catch (Exception fileEx)
            {
                ErrorHandler.Error("Failed to persist offline log batch.", fileEx);
            }
        }

        private async Task SendPersistedBatchesAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PersistenceDirectory))
                {
                    return;
                }

                if (!Directory.Exists(PersistenceDirectory))
                {
                    return;
                }

                string[] files = Directory.GetFiles(
                    PersistenceDirectory,
                    $"{OfflineFilePrefix}*{OfflineFileExtension}"
                );

                foreach (string file in files)
                {
                    string content = File.ReadAllText(file, Encoding.UTF8);

                    try
                    {
                        var response = await _httpClient.PostAsync(
                            EndpointUrl,
                            new StringContent(content, Encoding.UTF8, "application/x-ndjson")
                        );

                        if (response.IsSuccessStatusCode)
                        {
                            File.Delete(file);
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Error($"Failed to send persisted log batch: {file}", ex);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Error while sending persisted log batches.", ex);
            }
        }

        protected override void OnClose()
        {
            _flushTimer?.Dispose();
            _flushTimer = null;

            SendBatch();

            base.OnClose();
        }

        public void Dispose()
        {
            _flushTimer?.Dispose();
        }
    }
}
