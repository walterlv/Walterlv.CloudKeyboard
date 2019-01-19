using Newtonsoft.Json;

namespace Walterlv.CloudTyping
{
    public readonly struct TypingText
    {
        [JsonConstructor]
        public TypingText(string text, int caretStartIndex = -1, int caretEndIndex = -1, bool enter = false)
        {
            Text = text ?? "";
            CaretStartIndex = caretStartIndex < 0 || caretStartIndex > text.Length ? text.Length : caretStartIndex;
            CaretEndIndex = caretEndIndex < 0 || caretEndIndex > text.Length ? text.Length : caretEndIndex;
            Enter = enter;
        }

        public string Text { get; }

        public int CaretStartIndex { get; }

        public int CaretEndIndex { get; }

        public bool Enter { get; }
    }
}
