using System;
using System.Linq;
using System.Threading.Tasks;
using UtilitiesManager;
using Avalonia;

namespace UtilitiesManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Check if GUI environment is available
            bool hasGui = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")) || 
                         !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY"));

            // Force GUI mode for testing on Windows
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                hasGui = true;

            if (args.Length > 0)
            {
                await CliUtilMan.RunCliMode(args);
            }
            else if (!hasGui)
            {
                // No GUI environment, run CLI interactive mode
                await CliUtilMan.RunInteractiveMode();
            }
            else
            {
                // GUI available, start Avalonia app
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
