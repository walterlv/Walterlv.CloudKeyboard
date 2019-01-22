using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Walterlv.CloudTyping
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // 启动前一个进程实例。
            try
            {
                var current = Process.GetCurrentProcess();
                var process = Process.GetProcessesByName(current.ProcessName).FirstOrDefault(x => x.Id != current.Id);
                if (process != null)
                {
                    var hwnd = process.MainWindowHandle;
                    ShowWindow(hwnd, 9);
                    SetForegroundWindow(hwnd);
                    return;
                }
            }
            catch (Exception)
            {
                // 忽略任何异常
            }

            // 启动自己。
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, uint nCmdShow);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
