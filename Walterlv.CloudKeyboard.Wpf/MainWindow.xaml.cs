using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Walterlv.CloudTyping
{
    public partial class MainWindow : Window
    {
        private CloudKeyboard _keyboard;
        private readonly DelayRunner<TypingText> _runner;

        public MainWindow()
        {
            InitializeComponent();
            _keyboard = new CloudKeyboard("0");
            _runner = new DelayRunner<TypingText>(TimeSpan.FromSeconds(0.2), SendCore);
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
            Send(true);
            TypingTextBox.Text = "";
        }

        private void Send(bool enter = false)
        {
            _runner.Run(new TypingText(TypingTextBox.Text, TypingTextBox.SelectionStart,
                TypingTextBox.SelectionStart + TypingTextBox.SelectionLength, enter), enter);
        }

        private async Task SendCore(TypingText state)
        {
            try
            {
                await _keyboard.PutTextAsync(state.Text, state.CaretStartIndex, state.CaretEndIndex, state.Enter);
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
