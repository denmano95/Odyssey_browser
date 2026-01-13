using System;
using System.Windows;

namespace Odyssey
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string MutexName = "OdysseySingleInstanceMutex";
        private System.Threading.Mutex _mutex;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new System.Threading.Mutex(true, MutexName, out bool createdNew);

            if (!createdNew)
            {
                // App is already running!
                // Find the existing process
                var current = System.Diagnostics.Process.GetCurrentProcess();
                foreach (var process in System.Diagnostics.Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        IntPtr handle = process.MainWindowHandle;
                        if (handle != IntPtr.Zero)
                        {
                            // Restore if minimized
                            ShowWindow(handle, SW_RESTORE);
                            // Bring to front
                            SetForegroundWindow(handle);
                        }
                        break;
                    }
                }

                // Close this instance
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}
