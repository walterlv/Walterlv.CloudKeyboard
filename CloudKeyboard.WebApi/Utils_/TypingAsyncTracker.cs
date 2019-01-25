using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Walterlv.CloudTyping.Models;

namespace Walterlv.CloudTyping
{
    public class TypingAsyncTracker
    {
        private readonly string _token;
        private long _currentVersion;
        private TaskCompletionSource<bool> _source;

        private TypingAsyncTracker(string token)
        {
            _token = token;
        }

        public void PushChanges(TypingChange change)
        {
            _source?.SetResult(true);
            _source = null;
        }

        public Task<bool> WaitForChangesAsync(long currentVersion, TimeSpan timeout)
        {
            if (currentVersion > _currentVersion)
            {
                // 在请求时已经发生了更新，那么直接推送新的修改。
                _currentVersion = currentVersion;
                return Task.FromResult(false);
            }
            else
            {
                _source = _source ?? new TaskCompletionSource<bool>();
                return _source.Task;
            }
        }

        private static readonly ConcurrentDictionary<string, TypingAsyncTracker> Trackers
            = new ConcurrentDictionary<string, TypingAsyncTracker>();

        public static TypingAsyncTracker From(string token) => Trackers.GetOrAdd(token, CreateNewTracker);

        private static TypingAsyncTracker CreateNewTracker(string token) => new TypingAsyncTracker(token);
    }
}