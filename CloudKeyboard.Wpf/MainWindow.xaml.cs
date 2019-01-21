using System;
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

            _sender = new CloudKeyboardSender(HostInfo.BaseUrl, "walterlv", () => new TypingText(
                TypingTextBox.Text, TypingTextBox.SelectionStart,
                TypingTextBox.SelectionStart + TypingTextBox.SelectionLength));
            _sender.TargetUpdated += OnTargetUpdated;
            _sender.ExceptionOccurred += OnExceptionOccurred;
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
                ErrorTipTextBlock.Visibility = Visibility;
                ErrorTipTextBlock.Text = e.Exception.ToString();
            });
        }
    }
}
