using Microsoft.EntityFrameworkCore;

namespace Walterlv.CloudTyping.Models
{
    public class KeyboardContext : DbContext
    {
        public KeyboardContext(DbContextOptions<KeyboardContext> options) : base(options)
        {
        }

        public DbSet<Keyboard> Keyboards { get; set; }
    }
}