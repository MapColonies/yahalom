using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace com.mapcolonies.core.Utilities
{
    public class TimerController : IDisposable
    {
        private Timer _timer;
        private readonly double _timerInterval;
        private readonly bool _isRepeating;
        private readonly SynchronizationContext _syncContext;

        public Action OnTimerElapsed { get; set; }

        public TimerController(double interval, bool isRepeating = true)
        {
            _timerInterval = interval;
            _isRepeating = isRepeating;
            _syncContext = SynchronizationContext.Current;
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new Timer(_timerInterval)
            {
                AutoReset = _isRepeating
            };
            _timer.Elapsed += HandleTimerElapsed;
        }

        public void StartTimer()
        {
            _timer?.Start();
        }

        public void Stop()
        {
            _timer?.Stop();
        }

        private void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _syncContext.Post(_ => OnTimerElapsed?.Invoke(), null);
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Elapsed -= HandleTimerElapsed;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            OnTimerElapsed = null;
        }
    }
}
