using System;
using System.Timers;

namespace Walterlv.CloudTyping
{
    public class DelayRunner
    {
        private readonly Action _action;
        private readonly Timer _timer;
        private bool _isWaiting;

        public DelayRunner(TimeSpan delay, Action action)
        {
            _action = action;
            _timer = new Timer(delay.TotalMilliseconds)
            {
                AutoReset = false,
            };
            _timer.Elapsed += OnElapsed;
        }

        public void Run()
        {
            if (_isWaiting)
            {
                return;
            }
            _timer.Stop();
            _timer.Start();
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            _isWaiting = false;
            _action?.Invoke();
        }
    }
}
