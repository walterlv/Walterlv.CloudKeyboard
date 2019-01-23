using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ObjCRuntime;
using UIKit;
using Walterlv.CloudTyping.Client;

namespace Walterlv.CloudTyping
{
    public partial class KeyboardViewController : UIInputViewController
    {
        private UIButton _nextKeyboardButton;
        private UIButton _debugButton;
        private UIButton _tokenButton;
        private UIButton _returnButton;
        private int _receivedCount;
        private int _totalReceivedCount;
        private int _changedCount;

        protected KeyboardViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void UpdateViewConstraints()
        {
            base.UpdateViewConstraints();

            // Add custom view sizing constraints here
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var token = UIDevice.CurrentDevice.Name;

            // 调试按钮
            _debugButton = CreateButtonToView(View, "...",
                NSLayoutAttribute.CenterX, NSLayoutAttribute.Top);

            // Token 按钮
            _tokenButton = CreateButtonToView(View, $"使用此文本设置你的 PC 键盘：{token}",
                NSLayoutAttribute.CenterX, NSLayoutAttribute.CenterY);
            
            // 切换输入法按钮
            _nextKeyboardButton = CreateButtonToView(View, "Next Keyboard",
                NSLayoutAttribute.Left, NSLayoutAttribute.Bottom);
            _nextKeyboardButton.AddTarget(this, new Selector("advanceToNextInputMode"), UIControlEvent.TouchUpInside);

            // 确认按钮
            _returnButton = CreateButtonToView(View, TextDocumentProxy.ReturnKeyType.ToString(),
                NSLayoutAttribute.Right, NSLayoutAttribute.Bottom);

            // 初始化打字。
            var receiver = new CloudKeyboardReceiver(HostInfo.BaseUrl, token);
            receiver.Typing += DidReceive;
            receiver.Confirmed += DidConfirm;
            receiver.ExceptionOccurred += ExceptionDidOccur;
            receiver.Start();
        }

        public override void TextWillChange(IUITextInput textInput)
        {
            // The app is about to change the document's contents. Perform any preparation here.
        }

        public override void TextDidChange(IUITextInput textInput)
        {
            // The app has just changed the document's contents, the document context has been updated.
            UIColor textColor = null;

            if (TextDocumentProxy.KeyboardAppearance == UIKeyboardAppearance.Dark)
            {
                textColor = UIColor.White;
            }
            else
            {
                textColor = UIColor.Black;
            }

            _nextKeyboardButton.SetTitleColor(textColor, UIControlState.Normal);
            _tokenButton.SetTitleColor(textColor, UIControlState.Normal);
            _debugButton.SetTitleColor(textColor, UIControlState.Normal);

            _returnButton.SetTitle(TextDocumentProxy.ReturnKeyType.ToString(), UIControlState.Normal);
            _returnButton.SizeToFit();
            _returnButton.TranslatesAutoresizingMaskIntoConstraints = false;
        }

        private void DidReceive(object sender, TypingTextEventArgs e)
        {
            _receivedCount++;
            _totalReceivedCount++;

            Input(e.Typing.Text);

            UpdateCounts();
        }

        private void DidConfirm(object sender, TypingTextEventArgs e)
        {
            _receivedCount = 0;
            _totalReceivedCount++;

            Input(e.Typing.Text);
            Input("\n");

            UpdateCounts();
        }

        private void ExceptionDidOccur(object sender, ExceptionEventArgs e)
        {
            _debugButton.SetTitle(e.Exception.ToString(), UIControlState.Normal);
            _debugButton.SizeToFit();
            _debugButton.TranslatesAutoresizingMaskIntoConstraints = false;
        }

        private UIButton CreateButtonToView(UIView view, string title,
            NSLayoutAttribute horizontalAlignment, NSLayoutAttribute verticalAlignment)
        {
            var button = new UIButton(UIButtonType.System);

            button.SetTitle(title, UIControlState.Normal);
            button.SizeToFit();
            button.TranslatesAutoresizingMaskIntoConstraints = false;

            view.AddSubview(button);

            var okButtonCenterXConstraint = NSLayoutConstraint.Create(button,
                horizontalAlignment, NSLayoutRelation.Equal, view, horizontalAlignment, 1.0f, 0.0f);
            var okButtonCenterYConstraint = NSLayoutConstraint.Create(button,
                verticalAlignment, NSLayoutRelation.Equal, view, verticalAlignment, 1.0f, 0.0f);
            view.AddConstraints(new[] {okButtonCenterXConstraint, okButtonCenterYConstraint});

            return button;
        }

        private void UpdateCounts()
        {
            _debugButton.SetTitle($"{_totalReceivedCount}-{_receivedCount}-{_changedCount}", UIControlState.Normal);
            _debugButton.SizeToFit();
            _debugButton.TranslatesAutoresizingMaskIntoConstraints = false;
        }

        private string _lastText;
        private readonly Queue<string> _inputingTexts = new Queue<string>();

        private async void Input(string text)
        {
            if (text == _lastText)
            {
                return;
            }
                
            _lastText = text;

            if (_inputingTexts.Any())
            {
                _inputingTexts.Enqueue(text);
                return;
            }

            _inputingTexts.Enqueue(text);

            while (_inputingTexts.Any())
            {
                var current = _inputingTexts.Dequeue();
                if (_inputingTexts.Any())
                {
                    var next = _inputingTexts.Peek();
                    if (current == "\n")
                    {
                        TextDocumentProxy.InsertText("\n");
                        await Task.Delay(100);
                    }
                    else if (next == "\n")
                    {
                        SetText(current);
                        await Task.Delay(100);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    SetText(current);
                    await Task.Delay(100);
                }
            }

            UpdateCounts();
        }

        private void SetText(string text)
        {
            _changedCount++;
            while (TextDocumentProxy.HasText)
            {
                TextDocumentProxy.DeleteBackward();
            }

            TextDocumentProxy.InsertText(text);
        }
    }
}
