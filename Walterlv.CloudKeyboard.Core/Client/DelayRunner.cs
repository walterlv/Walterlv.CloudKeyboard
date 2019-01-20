using System;
using System.Threading.Tasks;

namespace Walterlv.CloudTyping.Client
{
    public class DelayRunner<TRunningState>
    {
        private readonly Func<TRunningState, Task> _asyncAction;
        private readonly TimeSpan _delay;
        private bool _isRequired;
        private bool _isRunning;
        private bool _isInterrupted;
        private TRunningState _runningState;

        public DelayRunner(TimeSpan delay, Func<TRunningState, Task> asyncAction)
        {
            _delay = delay;
            _asyncAction = asyncAction;
        }

        public async void Run(TRunningState runningState, bool immediately = false)
        {
            _runningState = runningState;

            // 如果这是一个里程碑事件，那么立即执行此任务。
            if (immediately)
            {
                // 并同时标记此前的任务全部丢弃。
                _isInterrupted = true;
                _isRequired = false;
                await _asyncAction(_runningState).ConfigureAwait(false);
                return;
            }

            // 如果正在执行任务，那么只是标记需要执行任务。
            if (_isRunning)
            {
                _isRequired = true;
                return;
            }

            while (_isRequired || !_isRunning)
            {
                _isRequired = false;
                _isRunning = true;
                await Task.Delay(_delay).ConfigureAwait(false);

                // 如果任务被全部放弃，而且放弃之后中途也没有插入新的任务，那么就直接结束。
                if (_isInterrupted && !_isRequired)
                {
                    _isInterrupted = false;
                    break;
                }

                // 如果有新的插入任务，那么就重新等待。
                if (_isRequired)
                {
                    continue;
                }

                // 如果任务保留，而且也没有新插入的任务，那么就执行这个任务。
                await _asyncAction(_runningState).ConfigureAwait(false);
            }

            _isRunning = false;
        }
    }
}
