using System;
using System.Threading.Tasks;

namespace Walterlv.CloudTyping.Client
{
    public class CloudKeyboardSender
    {
        public CloudKeyboardSender(Func<TypingText> typingGetter)
        {
            _typingGetter = typingGetter;
            _runner = new DelayRunner<TypingText>(TimeSpan.FromSeconds(0.2), SendCore);
            Token = "0";
        }

        public string Token
        {
            get => _keyboard.Token;
            set => _keyboard = new CloudKeyboard(value);
        }

        public event EventHandler<TypingTextEventArgs> TargetUpdated;
        public event EventHandler<ExceptionEventArgs> ExceptionOccurred;

        public async void Reload()
        {
            TypingText text;

            try
            {
                text = await _keyboard.FetchTextAsync();
            }
            catch
            {
                text = new TypingText("");
            }

            if (!text.Enter)
            {
                TargetUpdated?.Invoke(this, new TypingTextEventArgs(text));
            }
        }

        public void Send(bool enter = false)
        {
            var typing = _typingGetter();

            if (enter)
            {
                typing.Freeze();
            }

            _runner.Run(typing, enter);
        }

        private async Task SendCore(TypingText state)
        {
            try
            {
                await _keyboard.PutTextAsync(state.Text, state.CaretStartIndex, state.CaretEndIndex, state.Enter);
            }
            catch (Exception ex)
            {
                ExceptionOccurred?.Invoke(this, new ExceptionEventArgs(ex));
            }
        }

        private CloudKeyboard _keyboard;
        private readonly Func<TypingText> _typingGetter;
        private readonly DelayRunner<TypingText> _runner;
    }
}
