using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Walterlv.CloudTyping
{
    public class TypingAsyncTracker
    {
        private readonly string _token;
        private long _currentVersion;
        private TaskCompletionSource<TypingText> _source;

        private TypingAsyncTracker(string token)
        {
            _token = token;
        }

        public void PushChanges(TypingText typing)
        {
            var source = _source;
            _source = null;
            source?.SetResult(typing);
        }

        public Task<TypingText> WaitForChangesAsync(long currentVersion, TimeSpan timeout)
        {
            if (currentVersion > _currentVersion)
            {
                // 在请求时已经发生了更新，那么直接推送新的修改。
                _currentVersion = currentVersion;
                return Task.FromResult<TypingText>(null);
            }
            else
            {
                _source = _source ?? new TaskCompletionSource<TypingText>();
                return _source.Task;
            }
        }

        private static readonly ConcurrentDictionary<string, TypingAsyncTracker> Trackers
            = new ConcurrentDictionary<string, TypingAsyncTracker>();

        public static TypingAsyncTracker From(string token) => Trackers.GetOrAdd(token, CreateNewTracker);

        private static TypingAsyncTracker CreateNewTracker(string token) => new TypingAsyncTracker(token);
    }
}