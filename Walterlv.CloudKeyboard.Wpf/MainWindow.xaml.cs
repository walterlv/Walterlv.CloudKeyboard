using System;
using System.Windows;
using System.Windows.Controls;

namespace Walterlv.CloudTyping
{
    public partial class MainWindow : Window
    {
        private CloudKeyboard _keyboard;
        private readonly DelayRunner _runner;

        public MainWindow()
        {
            InitializeComponent();
            _keyboard = new CloudKeyboard("0");
            _runner = new DelayRunner(TimeSpan.FromSeconds(0.5), Send);
        }

        private void TypingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _runner.Run();
        }

        private void TypingTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _runner.Run();
        }

        private async void Send()
        {
            await await TypingTextBox.Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    await _keyboard.SetTextAsync(TypingTextBox.Text,
                        TypingTextBox.SelectionStart, TypingTextBox.SelectionStart + TypingTextBox.SelectionLength);
                }
                catch (Exception ex)
                {
                    ErrorTipTextBlock.Visibility = Visibility;
                    ErrorTipTextBlock.Text = ex.ToString();
                }
            });
        }
    }
}
