using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Walterlv.CloudTyping.Client;

namespace Walterlv.CloudTyping
{
    public partial class MainWindow : Window
    {
        private readonly CloudKeyboardSender _sender;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;

            var token = GetTokenFromConfigs();
            TokenTextBox.Text = token;
            _sender = new CloudKeyboardSender(HostInfo.BaseUrl, token, () => new TypingText(
                TypingTextBox.Text, TypingTextBox.SelectionStart,
                TypingTextBox.SelectionStart + TypingTextBox.SelectionLength));
            _sender.TargetUpdated += OnTargetUpdated;
            _sender.ExceptionOccurred += OnExceptionOccurred;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TypingTextBox.Focus();
        }

        private void TypingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _sender.Send();
        }

        private void TypingTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _sender.Send();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            _sender.Send(true);
            TypingTextBox.Text = "";
        }

        private void OnActivated(object sender, EventArgs e)
        {
            _sender.Reload();
        }

        private void OnTargetUpdated(object sender, TypingTextEventArgs e)
        {
            var typing = e.Typing;
            TypingTextBox.Text = typing.Text;
            TypingTextBox.SelectionStart = typing.CaretStartIndex;
            TypingTextBox.SelectionLength = typing.CaretEndIndex - typing.CaretStartIndex;
        }

        private async void OnExceptionOccurred(object sender, ExceptionEventArgs e)
        {
            await TypingTextBox.Dispatcher.InvokeAsync(() =>
            {
                ErrorTipTextBlock.Text = e.Exception.ToString();
            });
        }

        private void TokenTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var editedToken = TokenTextBox.Text;
            if (string.IsNullOrWhiteSpace(editedToken))
            {
                TokenTextBox.Text = _sender.Token;
            }
            else
            {
                _sender.Token = editedToken;
                SetTokenToConfigs(editedToken);
            }
        }

        private static string GetTokenFromConfigs()
        {
            string token;

            try
            {
                token = FileConfigurationRepo.Deserialize(@"configs.fkv")["Token"];
            }
            catch
            {
                token = "0";
            }

            return token;
        }

        private void SetTokenToConfigs(string token)
        {
            File.WriteAllText(@"configs.fkv", $@"Token
{token}");
        }
    }
}
