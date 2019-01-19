using System;
using System.Windows;
using System.Windows.Controls;

namespace Walterlv.CloudTyping
{
    public partial class MainWindow : Window
    {
        private CloudKeyboard _keyboard;

        public MainWindow()
        {
            InitializeComponent();
            _keyboard = new CloudKeyboard("0");
        }

        private void TypingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Send();
        }

        private void TypingTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Send();
        }

        private async void Send()
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
        }
    }
}
