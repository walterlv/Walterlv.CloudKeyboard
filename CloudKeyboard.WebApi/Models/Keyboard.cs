using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TypingTextModel = Walterlv.CloudTyping.Models.TypingText;

namespace Walterlv.CloudTyping.Models
{
    public class Keyboard
    {
        [Key]
        public string Token { get; set; }

        public virtual List<TypingTextModel> Typings { get; set; }
    }
}