using Avalonia;
using System;
using System.Threading.Tasks;
using UtilitiesManagerCLI;

namespace UtilitiesManager
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Check if we should run CLI mode
            if (args.Length > 0)
            {
                // Run CLI mode asynchronously
                Task.Run(async () => await CliUtilMan.RunCliMode(args)).Wait();
                return;
            }

            // Check if we're in a headless environment or should force CLI
            var forceCli = Environment.GetEnvironmentVariable("UTILITIES_MANAGER_CLI") == "1" ||
                          Environment.GetEnvironmentVariable("DISPLAY") == null;
            
            if (forceCli)
            {
                // Run CLI interactive mode
                Task.Run(async () => await CliUtilMan.RunInteractiveMode()).Wait();
                return;
            }

            // Run GUI mode
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start GUI: {ex.Message}");
                Console.WriteLine("Falling back to CLI mode...");
                Task.Run(async () => await CliUtilMan.RunInteractiveMode()).Wait();
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
