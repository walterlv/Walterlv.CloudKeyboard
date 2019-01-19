using Newtonsoft.Json;

namespace Walterlv.CloudTyping
{
    public readonly struct TypingText
    {
        public TypingText(string text)
        {
            Text = text ?? "";
            CaretStartIndex = Text.Length;
            CaretEndIndex = CaretStartIndex;
        }

        public TypingText(string text, int caretIndex)
        {
            Text = text ?? "";
            CaretStartIndex = caretIndex;
            CaretEndIndex = caretIndex;
        }

        [JsonConstructor]
        public TypingText(string text, int caretStartIndex, int caretEndIndex)
        {
            Text = text;
            CaretStartIndex = caretStartIndex;
            CaretEndIndex = caretEndIndex;
        }

        public string Text { get; }

        public int CaretStartIndex { get; }

        public int CaretEndIndex { get; }
    }
}
