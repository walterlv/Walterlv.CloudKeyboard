using Microsoft.EntityFrameworkCore;

namespace Walterlv.CloudTyping.Models
{
    public class KeyboardContext : DbContext
    {
        public KeyboardContext(DbContextOptions<KeyboardContext> options) : base(options)
        {
        }

        public DbSet<Keyboard> Keyboards { get; set; }
        public DbSet<TypingText> Typings { get; set; }
        public DbSet<TypingChange> Changes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Keyboard>()
                .HasMany(k => k.Typings);
        }
    }
}