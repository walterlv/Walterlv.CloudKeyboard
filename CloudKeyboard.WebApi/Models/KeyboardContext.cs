﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TypingTextCore = Walterlv.CloudTyping.TypingText;
using TypingTextModel = Walterlv.CloudTyping.Models.TypingText;
using TypingRepo = System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Concurrent.ConcurrentQueue<Walterlv.CloudTyping.TypingText>>;

namespace Walterlv.CloudTyping.Models
{
    public class KeyboardContext : DbContext
    {
        public KeyboardContext(DbContextOptions<KeyboardContext> options) : base(options)
        {
        }

        public DbSet<Keyboard> Keyboards { get; set; }
        public DbSet<TypingTextModel> TypingTexts { get; set; }

        public TypingRepo TypingTextRepo = new TypingRepo(new Dictionary<string, string>
        {
            {"0", "Welcome to use walterlv's cloud keyboard."},
        }.ToDictionary(x => x.Key, x => new ConcurrentQueue<TypingTextCore>(new[] {new TypingTextCore(x.Value)})));
    }
}