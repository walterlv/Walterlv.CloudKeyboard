using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TypingRepo = System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Concurrent.ConcurrentQueue<Walterlv.CloudTyping.TypingText>>;

namespace Walterlv.CloudTyping.Models
{
    public class KeyboardContext : DbContext
    {
        public KeyboardContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Keyboard> Keyboards { get; set; }

        public TypingRepo TypingTextRepo = new TypingRepo(new Dictionary<string, string>
        {
            {"0", "Welcome to use walterlv's cloud keyboard."},
        }.ToDictionary(x => x.Key, x => new ConcurrentQueue<TypingText>(new[] {new TypingText(x.Value)})));
    }
}
