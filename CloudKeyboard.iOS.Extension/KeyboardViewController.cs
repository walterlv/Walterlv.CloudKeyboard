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
        private static int _staticLoadedCount;
        private int _loadedCount;
        private static int _unloadedCount;
        private int _receivedCount;
        private int _totalReceivedCount;
        private int _changedCount;
        private CloudKeyboardReceiver _receiver;
        private bool IsLoaded => _nextKeyboardButton != null;

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

            try
            {
                // 调试按钮
                _debugButton = CreateButtonToView(View, "...",
                    NSLayoutAttribute.CenterX, NSLayoutAttribute.Top);
            }
            catch (Exception ex)
            {
            }

            try
            {
                // Token 按钮
                _tokenButton = CreateButtonToView(View, $"使用此文本设置你的 PC 键盘：{token}",
                    NSLayoutAttribute.CenterX, NSLayoutAttribute.CenterY);
            }
            catch (Exception ex)
            {
                Debug(ex);
            }

            try
            {
                // 切换输入法按钮
                _nextKeyboardButton = CreateButtonToView(View, "Next Keyboard",
                    NSLayoutAttribute.Left, NSLayoutAttribute.Bottom);
                _nextKeyboardButton.AddTarget(this, new Selector("advanceToNextInputMode"), UIControlEvent.TouchUpInside);
            }
            catch (Exception ex)
            {
                Debug(ex);
            }

            try
            {
                // 确认按钮
                _returnButton = CreateButtonToView(View, TextDocumentProxy.ReturnKeyType.ToString(),
                    NSLayoutAttribute.Right, NSLayoutAttribute.Bottom);
            }
            catch (Exception ex)
            {
                Debug(ex);
            }

            // 初始化打字。
            if (_receiver == null)
            {
                _receiver = new CloudKeyboardReceiver(HostInfo.BaseUrl, token);
                _receiver.Typing += DidReceive;
                _receiver.Confirmed += DidConfirm;
                _receiver.ExceptionOccurred += ExceptionDidOccur;
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            _receiver?.Start();
            _staticLoadedCount++;
            _loadedCount++;
        }

        public override void ViewWillDisappear(bool animated)
        {
            _unloadedCount++;
            _receiver?.Stop();
            base.ViewWillDisappear(animated);
        }

        public override void TextWillChange(IUITextInput textInput)
        {
            // The app is about to change the document's contents. Perform any preparation here.
        }

        public override void TextDidChange(IUITextInput textInput)
        {
            // The app has just changed the document's contents, the document context has been updated.
            UIColor textColor = null;

            try
            {
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
            catch (Exception e)
            {
                Debug(e);
            }
        }

        private void DidReceive(object sender, TypingTextEventArgs e)
        {
            BeginInvokeOnMainThread(() =>
            {
                try
                {
                    _receivedCount++;
                    _totalReceivedCount++;

                    Input(e.Typing.Text);

                    // 阻止屏幕黑屏 https://github.com/walterlv/Walterlv.CloudKeyboard/issues/3
                    UIApplication.SharedApplication.IdleTimerDisabled = true;
                    UpdateCounts();
                }
                catch (Exception ex)
                {
                    Debug(ex);
                }
            });
        }

        private void DidConfirm(object sender, TypingTextEventArgs e)
        {
            BeginInvokeOnMainThread(() =>
            {
                try
                {
                    _receivedCount = 0;
                    _totalReceivedCount++;

                    Input(e.Typing.Text);
                    Input("\n");

                    UpdateCounts();
                }
                catch (Exception ex)
                {
                    Debug(ex);
                }
            });
        }

        private void ExceptionDidOccur(object sender, ExceptionEventArgs e)
        {
            Debug(e.Exception);
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
            view.AddConstraints(new[] { okButtonCenterXConstraint, okButtonCenterYConstraint });

            return button;
        }

        private void UpdateCounts()
        {
            Debug($"{_staticLoadedCount}-{_loadedCount}-{_unloadedCount} -- {_totalReceivedCount}-{_receivedCount}-{_changedCount}");
        }

        private string _lastText;
        private readonly Queue<string> _inputingTexts = new Queue<string>();

        private void Input(string text)
        {
            BeginInvokeOnMainThread(()=>
            {
                try
                {
                    InputInner();
                }
                catch (Exception ex)
                {
                    Debug(ex);
                }
            });

            async void InputInner()
            {
                if (!IsLoaded)
                {
                    return;
                }

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
        }

        private void SetText(string text)
        {
            BeginInvokeOnMainThread(SetTextInner);

            void SetTextInner()
            {
                try
                {
                    _changedCount++;
                    while (TextDocumentProxy.HasText)
                    {
                        TextDocumentProxy.DeleteBackward();
                    }

                    TextDocumentProxy.InsertText(text);
                }
                catch (Exception ex)
                {
                    Debug(ex);
                }
            }
        }

        private void Debug(Exception exception)
        {
            if (!IsLoaded)
            {
                return;
            }

            string info;
            if (exception is NullReferenceException nre)
            {
                info = $"null:{nre.StackTrace}";
            }
            else
            {
                info = $"{exception.GetType().Name.Replace("Exception", "")}:{exception.StackTrace}";
            }

            Debug(info);
        }

        private void Debug(string info)
        {
            BeginInvokeOnMainThread(() =>
            {
                _debugButton.SetTitle(info, UIControlState.Normal);
                _debugButton.SizeToFit();
            });
        }
    }
}
