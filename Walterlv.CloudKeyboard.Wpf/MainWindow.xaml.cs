using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Walterlv.CloudTyping
{
    public partial class MainWindow : Window
    {
        private CloudKeyboard _keyboard;
        private readonly DelayRunner _runner;
        private string _typingText;
        private int _selectionStart;
        private int _selectionLength;
        private bool _isEntered;

        public MainWindow()
        {
            InitializeComponent();
            _keyboard = new CloudKeyboard("0");
            _runner = new DelayRunner(TimeSpan.FromSeconds(0.1), SendCore);
        }

        private void TypingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Send();
        }

        private void TypingTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Send();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            _isEntered = true;
            Send();
            TypingTextBox.Text = "";
        }

        private void Send()
        {
            _typingText = TypingTextBox.Text;
            _selectionStart = TypingTextBox.SelectionStart;
            _selectionLength = TypingTextBox.SelectionLength;
            _runner.Run();
        }

        private async Task SendCore()
        {
            try
            {
                await _keyboard.PutTextAsync(_typingText, _selectionStart,
                    _selectionStart + _selectionLength, _isEntered);
                _isEntered = false;
            }
            catch (Exception ex)
            {
                await TypingTextBox.Dispatcher.InvokeAsync(() =>
                {
                    ErrorTipTextBlock.Visibility = Visibility;
                    ErrorTipTextBlock.Text = ex.ToString();
                });
            }
        }

        private async void OnActivated(object sender, EventArgs e)
        {
            var text = await _keyboard.FetchTextAsync();
            if (!text.Enter)
            {
                TypingTextBox.Text = text.Text;
                var selectionStart = Math.Min(text.CaretStartIndex, text.CaretEndIndex);
                var selectionEnd = Math.Max(text.CaretStartIndex, text.CaretEndIndex);
                TypingTextBox.SelectionStart = selectionStart;
                TypingTextBox.SelectionLength = selectionEnd - selectionStart;
            }
        }
    }
}
