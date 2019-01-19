using Newtonsoft.Json;

namespace Walterlv.CloudTyping
{
    public readonly struct TypingText
    {
        public TypingText(string text) : this(text, text?.Length ?? 0, text?.Length ?? 0)
        {
        }

        public TypingText(string text, int caretIndex) : this(text, caretIndex, caretIndex)
        {
        }

        [JsonConstructor]
        public TypingText(string text, int caretStartIndex, int caretEndIndex)
        {
            Text = text ?? "";
            CaretStartIndex = caretStartIndex < 0 || caretStartIndex > text.Length ? text.Length : caretStartIndex;
            CaretEndIndex = caretEndIndex < 0 || caretEndIndex > text.Length ? text.Length : caretEndIndex;
        }

        public string Text { get; }

        public int CaretStartIndex { get; }

        public int CaretEndIndex { get; }
    }
}
