using System;
using com.mapcolonies.core.Services.Analytics.Enums;
using com.mapcolonies.core.Services.Analytics.Model;
using com.mapcolonies.core.Utilities;
using UnityEngine;
using VContainer.Unity;

namespace com.mapcolonies.core.Services.Analytics.Managers
{
    public class UsageAnalyticsManager : IInitializable, IDisposable
    {
        private readonly IAnalyticsManager _analyticsManager;
        private TimerController _timerController;
        private DateTime _previousProcessorSamplingTime;
        private TimeSpan _previousTotalProcessorTime;

        private const double DefaultPerformanceSampleIntervalSeconds = 60;
        private const string UsageLogComponent = "General";

        public UsageAnalyticsManager(IAnalyticsManager analyticsManager)
        {
            _analyticsManager = analyticsManager;
        }

        public void Initialize()
        {
            (_previousProcessorSamplingTime, _previousTotalProcessorTime) = PlatformUsageHelper.GetInitialProcessorTimes();

            var (performanceSamplingIntervalInSeconds, enabled) = GetConfig();
            if (!enabled) return;

            TimeSpan interval = TimeSpan.FromSeconds(performanceSamplingIntervalInSeconds);
            _timerController = new TimerController(interval.TotalMilliseconds);
            _timerController.OnTimerElapsed += HandleTimerElapsed;
            _timerController.StartTimer();
        }

        private void HandleTimerElapsed()
        {
            var (fps, allocatedMemory, cpuUsage, newTime, newSpan) = PlatformUsageHelper.GetApplicationPerformanceSnapshot(_previousProcessorSamplingTime, _previousTotalProcessorTime);

            _previousProcessorSamplingTime = newTime;
            _previousTotalProcessorTime = newSpan;

            PublishApplicationPerformance(fps, allocatedMemory, cpuUsage);
        }

        private static (double, bool) GetConfig()
        {
            return (DefaultPerformanceSampleIntervalSeconds, true);
            //TODO: Remove after adding and supporting SharedConfigProvider
            /*if (!SharedConfigProvider.TryGetSharedConfig(out var sharedConfig))
            {
                Debug.LogWarning(
                    $"Failed to get shared config, using default interval: {DefaultPerformanceSampleIntervalSeconds}");
                return (DefaultPerformanceSampleIntervalSeconds, true);
            }

            var systemLogConfiguration = sharedConfig.SystemLogConfiguration;
            if (systemLogConfiguration == null) return (DefaultPerformanceSampleIntervalSeconds, true);

            var performanceUsageConfiguration = systemLogConfiguration.PerformanceUsageConfiguration;
            if (performanceUsageConfiguration == null) return (DefaultPerformanceSampleIntervalSeconds, true);

            return (performanceUsageConfiguration.PerformanceSampleIntervalInSeconds,
                performanceUsageConfiguration.Enable);*/
        }

        public void Dispose()
        {
            if (_timerController == null) return;

            _timerController.OnTimerElapsed -= HandleTimerElapsed;
            _timerController.Stop();
            _timerController.Dispose();
            _timerController = null;
        }

        protected void PublishApplicationPerformance(float fps, double allocatedMemory, double cpuUsage)
        {
            var performanceData = PerformanceData.Create(fps, allocatedMemory, cpuUsage);
            var logObject = LogObject.Create(
                _analyticsManager.SessionId,
                LogType.Log,
                AnalyticsMessageTypes.ConsumptionStatus.ToString(),
                performanceData,
                UsageLogComponent,
                AnalyticsMessageTypes.ConsumptionStatus);

            _ = _analyticsManager.Publish(logObject);
        }
    }
}
