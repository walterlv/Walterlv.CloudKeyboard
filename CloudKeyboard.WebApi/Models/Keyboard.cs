using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TypingTextModel = Walterlv.CloudTyping.Models.TypingText;

namespace Walterlv.CloudTyping.Models
{
    public class Keyboard
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Token { get; set; }

        public virtual List<TypingTextModel> Typings { get; set; }
    }
}