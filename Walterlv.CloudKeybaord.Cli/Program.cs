using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Walterlv.CloudTyping
{
    class Program
    {
        static void Main(string[] args)
        {
            var strings = Process.GetProcesses().Select(x => x.MainWindowTitle)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => x.StartsWith("Walterlv Cloud Keyboard")).ToArray();
            if (strings.Length >= 1)
            {
                RunAsMobileEnd().Wait();
            }
            else
            {
                RunAsPcEnd().Wait();
            }
        }

        private static Task RunAsPcEnd()
        {
            Console.Title = "Walterlv Cloud Keyboard - PC";
            var token = ReadSingleLineText("Input a token: ");

            var keyboard = new CloudKeyboard(token);
            var reader = new ConsoleLineReader();
            reader.TextChanged += async (sender, args) =>
            {
                await keyboard.SetTextAsync(args.Line);
            };

            while (true)
            {
                reader.ReadLine();
            }
        }

        private static async Task RunAsMobileEnd()
        {
            Console.Title = "Walterlv Cloud Keyboard - Mobile";
            var token = ReadSingleLineText("Input a token: ");
            var keyboard = new CloudKeyboard(token);

            while (true)
            {
                var text = (await keyboard.GetTextAsync()).Text;
                Console.CursorTop = 1;
                Console.CursorLeft = 0;
                for (var i = 0; i < 320; i++)
                {
                    Console.Write(' ');
                }

                Console.CursorTop = 1;
                Console.CursorLeft = 0;
                Console.Write(text);
                await Task.Delay(1000);
            }
        }

        private static string ReadSingleLineText(string tip)
        {
            string result;

            do
            {
                Console.Write(tip);
                result = Console.ReadLine()?.Trim().ToLowerInvariant();
            } while (string.IsNullOrWhiteSpace(result));

            return result;
        }
    }
}
