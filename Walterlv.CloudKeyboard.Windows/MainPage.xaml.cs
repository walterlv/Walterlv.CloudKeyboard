using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Walterlv.CloudTyping
{
    public sealed partial class MainPage : Page
    {
        private CloudKeyboard _keyboard;

        public MainPage()
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
