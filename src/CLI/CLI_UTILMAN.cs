using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtilitiesManager;

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
                var menuOptions = new List<string>
                {
                    "Brightness Control",
                    "Volume Control",
                    "Battery Status",
                    "WiFi Networks",
                    "Power Profiles",
                    "System Monitoring",
                    "Service Management",
                    "User Management",
                    "Log Management",
                    "Firewall Management",
                    "Package Installation",
                    "Help & Documentation",
                    "Refresh Status",
                    "Quit"
                };

                var choice = MenuHelper.ShowArrowMenu("UTILITIES MANAGER - INTERACTIVE MODE", menuOptions);

                switch (choice)
                {
                    case 0:
                        await BrightnessService.BrightnessMenu();
                        break;
                    case 1:
                        await VolumeService.VolumeMenu();
                        break;
                    case 2:
                        await BatteryService.BatteryMenu();
                        break;
                    case 3:
                        await WifiService.WiFiMenu();
                        break;
                    case 4:
                        await PowerService.PowerMenu();
                        break;
                    case 5:
                        await SystemMonitoringService.SystemMonitoringMenu();
                        break;
                    case 6:
                        await ServicesService.ServiceManagementMenu();
                        break;
                    case 7:
                        await UserService.UserManagementMenu();
                        break;
                    case 8:
                        await LogService.LogManagementMenu();
                        break;
                    case 9:
                        await FirewallService.FirewallManagementMenu();
                        break;
                    case 10:
                        await PackageService.PackageInstallationMenu();
                        break;
                    case 11:
                        Help.ShowAllHelp();
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 12:
                        continue; // Refresh status
                    case 13:
                        MenuHelper.ShowMessage("Goodbye!", "Thank you for using Utilities Manager!");
                        return;
                    case -1:
                        MenuHelper.ShowMessage("Goodbye!", "Thank you for using Utilities Manager!");
                        return;
                }
            }
        }
    }
}

       

