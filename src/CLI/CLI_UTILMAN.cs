using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class CliUtilMan
    {
        public static async Task RunCliMode(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("No arguments provided. Run 'UtilMan help' for available commands.");
                return;
            }
            
            var command = args[0].ToLower();

            switch (command)
            {
                case "brightness":
                    await BrightnessService.HandleBrightnessCommand(args);
                    break;

                case "volume":
                    await VolumeService.HandleVolumeCommand(args);
                    break;

                case "battery":
                    BatteryService.HandleBatteryCommand();
                    break;

                case "wifi":
                    await WifiService.HandleWifiCommand(args);
                    break;

                case "bluetooth":
                    await BluetoothService.HandleBluetoothCommand(args);
                    break;

                case "power":
                    await PowerService.HandlePowerCommand(args);
                    break;

                case "status":
                    await StatusService.HandleStatusCommand();
                    break;

                case "help":
                    Help.ShowAllHelp();
                    break;

                case "cpu":
                case "memory":
                case "disk":
                case "network":
                    var checker = new CheckDependencyCommand();
                    await SystemMonitoringService.HandleSystemMonitoringCommand(checker, command);
                    break;

                case "services":
                    await ServicesService.HandleServicesCommand(args);
                    break;

                case "users":
                    await UserService.HandleUsersCommand(args);
                    break;

                case "logs":
                    await LogService.HandleLogsCommand(args);
                    break;

                case "firewall":
                    await FirewallService.HandleFirewallCommand(args);
                    break;

                case "install":
                case "download":
                    await PackageService.HandleDownloadCommand(args);
                    break;

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    Console.WriteLine("Run 'UtilMan help' for available commands or run without arguments for interactive mode.");
                    break;
            }
        }

        public static async Task RunInteractiveMode()
        {
            var checker = new CheckDependencyCommand();
            checker.CheckDependencies();

            while (true)
            {
                var menuOptions = new List<(string, Func<Task>)>
                {
                    ("Brightness Control", async () => await BrightnessService.BrightnessMenu(checker)),
                    ("Volume Control", async () => await VolumeService.MenuService(checker)),
                    ("Battery Status", async () => { BatteryService.MenuService(checker); await Task.Delay(1); }),
                    ("WiFi Networks", async () => await WifiService.MenuService(checker)),
                    ("Bluetooth Devices", async () => await BluetoothService.MenuService(checker)),
                    ("Power Profiles", async () => await PowerService.MenuService(checker)),
                    ("System Monitoring", async () => await SystemMonitoringService.SystemMonitoringMenu()),
                    ("Service Management", async () => await ServicesService.MenuService(checker)),
                    ("User Management", async () => await UserService.MenuService(checker)),
                    ("Log Management", async () => await LogService.MenuService(checker)),
                    ("Firewall Management", async () => await FirewallService.MenuService(checker)),
                    ("Package Installation", async () => await PackageService.MenuService(checker)),
                    ("Help & Documentation", async () => { Help.ShowAllHelp(); await Task.Delay(1); }),
                    ("Refresh Status", async () => { checker.CheckDependencies(); await Task.Delay(1); }),
                    ("Quit", async () => { MenuEngine.GeneralMessage("Goodbye! Thank you for using Utilities Manager!"); Environment.Exit(0); })
                };

                await MenuEngine.DisplayMenu(menuOptions);
            }
        }
    }
}
