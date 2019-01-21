using System;
using ObjCRuntime;
using UIKit;
using Walterlv.CloudTyping.Client;

namespace Walterlv.CloudTyping
{
    public partial class KeyboardViewController : UIInputViewController
    {
        UIButton nextKeyboardButton;
        UIButton tokenButton;
        UIButton okButton;

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

            // 初始化打字。
            var token = UIDevice.CurrentDevice.Name;
            var receiver = new CloudKeyboardReceiver(HostInfo.BaseUrl, token);
            receiver.Typing += DidReceive;
            receiver.Confirmed += DidConfirm;
            receiver.Start();

            // Perform custom UI setup here
            nextKeyboardButton = new UIButton(UIButtonType.System);

            nextKeyboardButton.SetTitle("Next Keyboard", UIControlState.Normal);
            nextKeyboardButton.SizeToFit();
            nextKeyboardButton.TranslatesAutoresizingMaskIntoConstraints = false;

            nextKeyboardButton.AddTarget(this, new Selector("advanceToNextInputMode"), UIControlEvent.TouchUpInside);

            View.AddSubview(nextKeyboardButton);

            var nextKeyboardButtonLeftSideConstraint = NSLayoutConstraint.Create(nextKeyboardButton,
                NSLayoutAttribute.Left, NSLayoutRelation.Equal, View, NSLayoutAttribute.Left, 1.0f, 0.0f);
            var nextKeyboardButtonBottomConstraint = NSLayoutConstraint.Create(nextKeyboardButton,
                NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1.0f, 0.0f);
            View.AddConstraints(new[] {nextKeyboardButtonLeftSideConstraint, nextKeyboardButtonBottomConstraint});

            // Token 按钮
            tokenButton = new UIButton(UIButtonType.RoundedRect);

            tokenButton.SetTitle($"使用此文本设置你的 PC 键盘：{token}", UIControlState.Normal);
            tokenButton.SizeToFit();
            tokenButton.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(tokenButton);

            var tokenButtonCenterXConstraint = NSLayoutConstraint.Create(tokenButton,
                NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1.0f, 0.0f);
            var tokenButtonCenterYConstraint = NSLayoutConstraint.Create(tokenButton,
                NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1.0f, 0.0f);
            View.AddConstraints(new[] {tokenButtonCenterXConstraint, tokenButtonCenterYConstraint});

            // 确认按钮
            okButton = new UIButton(UIButtonType.System);

            okButton.SetTitle(TextDocumentProxy.ReturnKeyType.ToString(), UIControlState.Normal);
            okButton.SizeToFit();
            okButton.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(okButton);

            var okButtonCenterXConstraint = NSLayoutConstraint.Create(okButton,
                NSLayoutAttribute.Right, NSLayoutRelation.Equal, View, NSLayoutAttribute.Right, 1.0f, 0.0f);
            var okButtonCenterYConstraint = NSLayoutConstraint.Create(okButton,
                NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1.0f, 0.0f);
            View.AddConstraints(new[] {okButtonCenterXConstraint, okButtonCenterYConstraint});
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

            nextKeyboardButton.SetTitleColor(textColor, UIControlState.Normal);
        }

        private void DidReceive(object sender, TypingTextEventArgs e)
        {
            while (TextDocumentProxy.HasText)
            {
                TextDocumentProxy.DeleteBackward();
            }

            TextDocumentProxy.InsertText(e.Typing.Text);
        }

        private void DidConfirm(object sender, TypingTextEventArgs e)
        {
            while (TextDocumentProxy.HasText)
            {
                TextDocumentProxy.DeleteBackward();
            }

            TextDocumentProxy.InsertText(e.Typing.Text);
            base.TextDocumentProxy.InsertText("\n");
        }
    }
}
