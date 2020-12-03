using System;
using Walterlv.CloudTyping.Client;

namespace Walterlv.CloudTyping
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1 && args[1] == "pc")
            {
                RunAsPcEnd();
            }
            else
            {
                RunAsMobileEnd();
            }
        }

        private static void RunAsPcEnd()
        {
            Console.Title = "Walterlv Cloud Keyboard - PC";
            var token = ReadSingleLineText("Input a token: ");

            var keyboard = new CloudKeyboard(HostInfo.BaseUrl, token);
            var reader = new ConsoleLineReader();
            reader.TextChanged += async (sender, args) =>
            {
                await keyboard.PutTextAsync(args.Line, args.Line.Length - 1, args.Line.Length);
            };

            while (true)
            {
                reader.ReadLine();
            }

            string ReadSingleLineText(string tip)
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

        private static void RunAsMobileEnd()
        {
            Console.Title = "Walterlv Cloud Keyboard - Mobile";
            var inScreenCount = 0;

            // 模拟构造函数。
            var receiver = new CloudKeyboardReceiver(HostInfo.BaseUrl, "0");
            receiver.Typing += OnReceived;
            receiver.Confirmed += OnConfirmed;
            receiver.Start();

            // 模拟消息循环。
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
            } while (key.Key != ConsoleKey.Escape);

            receiver.Stop();

            void OnReceived(object sender, TypingTextEventArgs e)
            {
                ClearCurrentDocument();
                SetDocument(e.Typing);
            }

            void OnConfirmed(object sender, TypingTextEventArgs e)
            {
                ClearCurrentDocument();

                inScreenCount++;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[上屏] ");
                Console.Write(e.Typing.Text);
                Console.ResetColor();
                Console.WriteLine();
            }

            void SetDocument(TypingText typing)
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

            void ClearCurrentDocument()
            {
                Console.CursorTop = inScreenCount;
                Console.CursorLeft = 0;
                for (var i = 0; i < 320; i++)
                {
                    Console.Write(' ');
                }

                Console.CursorTop = inScreenCount;
                Console.CursorLeft = 0;
            }
        }
    }
}
