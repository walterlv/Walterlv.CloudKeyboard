using System;
using System.Threading.Tasks;
using System.Timers;

namespace Walterlv.CloudTyping
{
    public class DelayRunner
    {
        private readonly Func<Task> _asyncAction;
        private readonly Timer _timer;
        private bool _isWaiting;

        public DelayRunner(TimeSpan delay, Func<Task> asyncAction)
        {
            _asyncAction = asyncAction;
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
            _asyncAction?.Invoke();
        }
    }
}
