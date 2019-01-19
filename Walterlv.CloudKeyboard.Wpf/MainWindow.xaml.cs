using System;
using System.Windows;
using System.Windows.Controls;

namespace Walterlv.CloudTyping
{
    public partial class MainWindow : Window
    {
        private CloudKeyboard _keyboard;
        private readonly DelayRunner _runner;
        private bool _isEntered;

        public MainWindow()
        {
            InitializeComponent();
            _keyboard = new CloudKeyboard("0");
            _runner = new DelayRunner(TimeSpan.FromSeconds(0.1), Send);
        }

        private void TypingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _runner.Run();
        }

        private void TypingTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _runner.Run();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            _isEntered = true;
            _runner.Run();
        }

        private async void Send()
        {
            await await TypingTextBox.Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    await _keyboard.SetTextAsync(TypingTextBox.Text, TypingTextBox.SelectionStart,
                        TypingTextBox.SelectionStart + TypingTextBox.SelectionLength, _isEntered);
                    _isEntered = false;
                }
                catch (Exception ex)
                {
                    ErrorTipTextBlock.Visibility = Visibility;
                    ErrorTipTextBlock.Text = ex.ToString();
                }
            });
        }

        private async void OnActivated(object sender, EventArgs e)
        {
            var text = await _keyboard.GetTextAsync();
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
