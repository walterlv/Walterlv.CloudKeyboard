using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Walterlv.CloudTyping.Models
{
    public class TypingText
    {
        public TypingText()
        {
            Timestamp = DateTimeOffset.UtcNow.UtcTicks;
        }

        public TypingText(string keyboardToken, CloudTyping.TypingText typing) : this()
        {
            KeyboardToken = keyboardToken;
            Text = typing.Text;
            CaretStartIndex = typing.CaretStartIndex;
            CaretEndIndex = typing.CaretEndIndex;
            Enter = typing.Enter;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Keyboard.Token))]
        public string KeyboardToken { get; set; }

        public long Timestamp { get; set; }

        public string Text { get; set; }
        public int CaretStartIndex { get; set; }
        public int CaretEndIndex { get; set; }
        public bool Enter { get; set; }

        public CloudTyping.TypingText AsClient()
        {
            return new CloudTyping.TypingText(Text, CaretStartIndex, CaretEndIndex, Enter);
        }

        public void UpdateFrom(CloudTyping.TypingText value)
        {
            Text = value.Text;
            CaretStartIndex = value.CaretStartIndex;
            CaretEndIndex = value.CaretEndIndex;
            Enter = value.Enter;
        }
    }
}