using System;
using System.Threading.Tasks;

namespace Walterlv.CloudTyping.Client
{
    public class CloudKeyboardReceiver
    {
        public CloudKeyboardReceiver(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            Token = token;
        }

        public event EventHandler<TypingTextEventArgs> Typing;
        public event EventHandler<TypingTextEventArgs> Confirmed;
        public event EventHandler<ExceptionEventArgs> ExceptionOccurred;

        public async void Start()
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;

            while (_isRunning)
            {
                try
                {
                    var typing = await _keyboard.FetchTextAsync();

                    if (typing.Enter)
                    {
                        _lastTyping = typing;
                        Confirmed?.Invoke(this, new TypingTextEventArgs(typing));
                    }
                    else
                    {
                        var isEqual = _lastTyping != null && _lastTyping.Text == typing.Text
                                                          && _lastTyping.CaretStartIndex == typing.CaretStartIndex
                                                          && _lastTyping.CaretEndIndex == typing.CaretEndIndex;
                        _lastTyping = typing;
                        if (!isEqual)
                        {
                            Typing?.Invoke(this, new TypingTextEventArgs(typing));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionOccurred?.Invoke(this, new ExceptionEventArgs(ex));
                }

                await Task.Delay(500);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public string Token
        {
            get => _keyboard.Token;
            set => _keyboard = new CloudKeyboard(_baseUrl, value);
        }

        private TypingText _lastTyping;
        private CloudKeyboard _keyboard;
        private bool _isRunning;
        private readonly string _baseUrl;
    }
}
