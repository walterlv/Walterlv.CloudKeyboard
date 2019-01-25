using System.ComponentModel.DataAnnotations;

namespace Walterlv.CloudTyping.Models
{
    public class TypingChange
    {
        [Key]
        public string Token { get; set; }
        public long PopVersion { get; set; }
        public long PushVersion { get; set; }
    }
}