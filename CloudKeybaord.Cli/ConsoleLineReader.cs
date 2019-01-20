using System;
using System.Text;

namespace Walterlv.CloudTyping
{
    public sealed class ConsoleLineReader
    {
        public event EventHandler<ConsoleTextChangedEventArgs> TextChanged;

        public string ReadLine()
        {
            var builder = new StringBuilder();
            while (true)
            {
                var i = Console.ReadKey(true);

                if (i.Key == ConsoleKey.Enter)
                {
                    var line = builder.ToString();
                    OnTextChanged(line, i.Key);
                    Console.WriteLine();
                    return line;
                }

                if (i.Key == ConsoleKey.Backspace)
                {
                    if (builder.Length > 0)
                    {
                        var lastChar = builder[builder.Length - 1];
                        Console.Write(lastChar > 0xA0 ? "\b\b  \b\b" : "\b \b");
                        builder.Remove(builder.Length - 1, 1);
                    }
                }
                else
                {
                    builder.Append(i.KeyChar);
                    Console.Write(i.KeyChar);
                }

                OnTextChanged(builder.ToString(), i.Key);
            }
        }

        private void OnTextChanged(string line, ConsoleKey key)
        {
            TextChanged?.Invoke(this, new ConsoleTextChangedEventArgs(line, key));
        }
    }

    public class ConsoleTextChangedEventArgs : EventArgs
    {
        public ConsoleTextChangedEventArgs(string line, ConsoleKey consoleKey)
        {
            Line = line;
            ConsoleKey = consoleKey;
        }

        public string Line { get; }
        public ConsoleKey ConsoleKey { get; }
    }
}
