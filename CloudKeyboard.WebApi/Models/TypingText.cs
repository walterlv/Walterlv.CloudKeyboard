using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Walterlv.CloudTyping.Models
{
    public class TypingText
    {
        public TypingText()
        {
        }

        public TypingText(TypingText typing)
        {
            Text = typing.Text;
            CaretStartIndex = typing.CaretStartIndex;
            CaretEndIndex = typing.CaretEndIndex;
            Enter = typing.Enter;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long Timestamp { get; set; }
        public string Text { get; set; }
        public int CaretStartIndex { get; set; }
        public int CaretEndIndex { get; set; }
        public bool Enter { get; set; }

        public CloudTyping.TypingText AsClient()
        {
            return new CloudTyping.TypingText(Text, CaretStartIndex, CaretEndIndex, Enter);
        }

        public static implicit operator TypingText(CloudTyping.TypingText typing)
        {
            return new TypingText
            {
                Text = typing.Text,
                CaretStartIndex = typing.CaretStartIndex,
                CaretEndIndex = typing.CaretEndIndex,
                Enter = typing.Enter,
                Timestamp = DateTimeOffset.UtcNow.UtcTicks,
            };
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