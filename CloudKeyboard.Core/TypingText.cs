﻿using System;
using Newtonsoft.Json;

namespace Walterlv.CloudTyping
{
    public sealed class TypingText
    {
        private string _text;
        private int _caretStartIndex;
        private int _caretEndIndex;

        public TypingText()
        {
        }

        [JsonConstructor]
        public TypingText(string text, int caretStartIndex = -1, int caretEndIndex = -1, bool enter = false)
        {
            _text = text ?? "";
            _caretStartIndex = caretStartIndex < 0 || caretStartIndex > _text.Length ? _text.Length : caretStartIndex;
            _caretEndIndex = caretEndIndex < 0 || caretEndIndex > _text.Length ? _text.Length : caretEndIndex;
            Enter = enter;
        }

        public string Text
        {
            get => _text;
            set
            {
                VerifyFreezing();
                _text = value;
            }
        }

        public int CaretStartIndex
        {
            get => _caretStartIndex;
            set
            {
                VerifyFreezing();
                _caretStartIndex = value;
            }
        }

        public int CaretEndIndex
        {
            get => _caretEndIndex;
            set
            {
                VerifyFreezing();
                _caretEndIndex = value;
            }
        }

        public bool Enter { get; set; }

        public void Freeze()
        {
            Enter = true;
        }

        private void VerifyFreezing()
        {
            if (Enter) throw new InvalidOperationException("在消息确认后，不可修改。");
        }
    }
}
