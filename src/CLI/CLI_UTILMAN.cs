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
                    await BatteryService.HandleBatteryCommand();
                    break;

                case "wifi":
                    await WifiService.HandleWifiCommand(args);
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
            while (true)
            {
                var menuOptions = new List<(string, Func<Task>)>
                {
                    ("Brightness Control", async () => await BrightnessService.BrightnessMenu()),
                    ("Volume Control", async () => await VolumeService.VolumeMenu()),
                    ("Battery Status", async () => await BatteryService.BatteryMenu()),
                    ("WiFi Networks", async () => await WifiService.WiFiMenu()),
                    ("Power Profiles", async () => await PowerService.PowerMenu()),
                    ("System Monitoring", async () => await SystemMonitoringService.SystemMonitoringMenu()),
                    ("Service Management", async () => await ServicesService.ServiceManagementMenu()),
                    ("User Management", async () => await UserService.UserManagementMenu()),
                    ("Log Management", async () => await LogService.LogManagementMenu()),
                    ("Firewall Management", async () => await FirewallService.FirewallManagementMenu()),
                    ("Package Installation", async () => await PackageService.PackageInstallationMenu()),
                    ("Help & Documentation", async () => { Help.ShowAllHelp(); await Task.Delay(1); }),
                    ("Refresh Status", async () => { await Task.Delay(1); }),
                    ("Quit", async () => { MenuEngine.GeneralMessage("Goodbye! Thank you for using Utilities Manager!"); Environment.Exit(0); })
                };

                MenuEngine.DisplayMenu(menuOptions);
            }
        }
    }
}
