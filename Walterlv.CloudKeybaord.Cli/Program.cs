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
            if (true)
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
                await keyboard.PutTextAsync(args.Line, args.Line.Length - 1, args.Line.Length);
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
            var inScreenCount = 0;
            var keyboard = new CloudKeyboard(token);

            while (true)
            {
                Console.CursorTop = inScreenCount + 1;
                Console.CursorLeft = 0;
                for (var i = 0; i < 320; i++)
                {
                    Console.Write(' ');
                }

                Console.CursorTop = inScreenCount + 1;
                Console.CursorLeft = 0;

                var typing = await keyboard.FetchTextAsync();
                if (typing.Enter)
                {
                    inScreenCount++;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("[上屏] ");
                    Console.Write(typing.Text);
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    for (var i = 0; i < typing.Text.Length; i++)
                    {
                        var c = typing.Text[i];
                        if (typing.CaretStartIndex <= i && typing.CaretEndIndex > i)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(c);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write(c);
                        }
                    }
                }

                await Task.Delay(500);
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
