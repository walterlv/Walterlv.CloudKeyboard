using System;

namespace Walterlv.CloudTyping.Client
{
    public class TypingTextEventArgs : EventArgs
    {
        public TypingTextEventArgs(TypingText typing)
        {
            Typing = typing;
        }

        public TypingText Typing { get; }
    }
}
