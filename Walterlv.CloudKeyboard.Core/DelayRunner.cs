using System;
using System.Threading.Tasks;

namespace Walterlv.CloudTyping
{
    public class DelayRunner
    {
        private readonly Func<Task> _asyncAction;
        private readonly TimeSpan _delay;
        private bool _isRequired;
        private bool _isRunning;

        public DelayRunner(TimeSpan delay, Func<Task> asyncAction)
        {
            _delay = delay;
            _asyncAction = asyncAction;
        }

        public async void Run()
        {
            if (_isRunning)
            {
                _isRequired = true;
                return;
            }

            while (_isRequired || !_isRunning)
            {
                _isRequired = false;
                _isRunning = true;
                await Task.Delay(_delay);
                if (_isRequired)
                {
                    continue;
                }
                await _asyncAction();
            }

            _isRunning = false;
        }
    }
}
