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

        private async void TypingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await _keyboard.SetTextAsync(TypingTextBox.Text,
                TypingTextBox.SelectionStart, TypingTextBox.SelectionStart + TypingTextBox.SelectionLength);
        }

        private async void TypingTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            await _keyboard.SetTextAsync(TypingTextBox.Text,
                TypingTextBox.SelectionStart, TypingTextBox.SelectionStart + TypingTextBox.SelectionLength);
        }
    }
}
