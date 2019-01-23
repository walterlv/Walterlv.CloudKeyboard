using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            UpdateCharacterInputLeftInfo();
        }

        private void TypingTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !e.IsRepeat)
            {
                e.Handled = true;
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    InsertNewLine();
                }
                else
                {
                    _sender.Send(true);
                    TypingTextBox.Text = "";
                }
            }

            void InsertNewLine()
            {
                var characterInputLeft = TypingTextBox.MaxLength - TypingTextBox.Text.Length;
                if (characterInputLeft < 2)
                {
                    return;
                }

                var oldText = TypingTextBox.Text;
                var selectionStart = TypingTextBox.SelectionStart;

                var builder = new StringBuilder();
                builder.Append(oldText.Substring(0, selectionStart));
                builder.AppendLine();
                builder.Append(oldText.Substring(selectionStart + TypingTextBox.SelectionLength));
                TypingTextBox.Text = builder.ToString();

                TypingTextBox.SelectionStart = selectionStart + Environment.NewLine.Length;
                TypingTextBox.SelectionLength = 0;
            }
        }

        private void TypingTextBox_SelectionChanged(object sender, RoutedEventArgs e) => _sender.Send();

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            if (TypingTextBox.IsFocused)
            {
                _sender.Send(true);
                TypingTextBox.Text = "";
            }
            else if (TokenTextBox.IsFocused)
            {
                TypingTextBox.Focus();
            }
        }

        private void OnActivated(object sender, EventArgs e) => _sender.Reload();

        private void OnTargetUpdated(object sender, TypingTextEventArgs e)
        {
            var typing = e.Typing;
            TypingTextBox.Text = typing.Text;
            TypingTextBox.SelectionStart = typing.CaretStartIndex;
            TypingTextBox.SelectionLength = typing.CaretEndIndex - typing.CaretStartIndex;
        }

        private void OnExceptionOccurred(object sender, ExceptionEventArgs e)
            => TypingTextBox.Dispatcher.InvokeAsync(() => ErrorTipTextBlock.Text = e.Exception.ToString());

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

        private void UpdateCharacterInputLeftInfo()
        {
            var characterInputLeft = TypingTextBox.MaxLength - TypingTextBox.Text.Length;
            if (characterInputLeft <= 200)
            {
                WarningTextBlock.Text = $"你还可以再输入 {characterInputLeft.ToString()} 个字……";
            }
            else if (characterInputLeft <= 0)
            {
                WarningTextBlock.Text = "你输入的字已经够多了……";
            }
            else
            {
                WarningTextBlock.Text = "";
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
