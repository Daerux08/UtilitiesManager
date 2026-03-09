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
                    await HandleBrightnessCommand(args);
                    break;

                case "volume":
                    await HandleVolumeCommand(args);
                    break;

                case "battery":
                    await HandleBatteryCommand();
                    break;

                case "wifi":
                    await HandleWifiCommand(args);
                    break;

                case "power":
                    await HandlePowerCommand(args);
                    break;

                case "status":
                    await ShowSystemStatus();
                    break;

                case "help":
                    Help.ShowAllHelp();
                    break;

                case "cpu":
                case "memory":
                case "disk":
                case "network":
                    await HandleSystemMonitoringCommand(command);
                    break;

                case "services":
                    await HandleServicesCommand(args);
                    break;

                case "users":
                    await HandleUsersCommand(args);
                    break;

                case "logs":
                    await HandleLogsCommand(args);
                    break;

                case "firewall":
                    await HandleFirewallCommand(args);
                    break;

                case "install":
                case "download":
                    await HandleDownloadCommand(args);
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
                        await BrightnessMenu();
                        break;
                    case 1:
                        await VolumeMenu();
                        break;
                    case 2:
                        await BatteryMenu();
                        break;
                    case 3:
                        await WiFiMenu();
                        break;
                    case 4:
                        await PowerMenu();
                        break;
                    case 5:
                        await SystemMonitoringMenu();
                        break;
                    case 6:
                        await ServiceManagementMenu();
                        break;
                    case 7:
                        await UserManagementMenu();
                        break;
                    case 8:
                        await LogManagementMenu();
                        break;
                    case 9:
                        await FirewallManagementMenu();
                        break;
                    case 10:
                        await PackageInstallationMenu();
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

        private static void ShowMainMenu()
        {
            Console.WriteLine("=== MAIN MENU ===");
            Console.WriteLine("1. Brightness Control");
            Console.WriteLine("2. Volume Control");
            Console.WriteLine("3. Battery Status");
            Console.WriteLine("4. WiFi Networks");
            Console.WriteLine("5. Power Profiles");
            Console.WriteLine("6. System Monitoring");
            Console.WriteLine("7. Service Management");
            Console.WriteLine("8. User Management");
            Console.WriteLine("9. Log Management");
            Console.WriteLine("10. Firewall Management");
            Console.WriteLine("11. Package Installation");
            Console.WriteLine("12. Help & Documentation");
            Console.WriteLine("r. Refresh Status");
            Console.WriteLine("q. Quit");
            Console.WriteLine();
        }

        private static async Task HandleBrightnessCommand(string[] args)
        {
            if (args.Length > 1 && int.TryParse(args[1], out int brightness) && brightness >= 0 && brightness <= 100)
            {
                var changer = new ChangeValueCommand();
                await changer.SetBrightnessAsync(brightness);
                Console.WriteLine($"Brightness set to {brightness}%");
            }
            else
            {
                Console.WriteLine("Usage: UtilMan brightness <percentage (0-100)>");
            }
        }

        private static async Task HandleVolumeCommand(string[] args)
        {
            if (args.Length > 1 && int.TryParse(args[1], out int volume) && volume >= 0 && volume <= 100)
            {
                var changer = new ChangeValueCommand();
                await changer.SetVolumeAsync(volume);
                Console.WriteLine($"Volume set to {volume}%");
            }
            else
            {
                Console.WriteLine("Usage: UtilMan volume <percentage (0-100)>");
            }
        }

        private static async Task HandleBatteryCommand()
        {
            var checker = new CheckDependencyCommand();
            await checker.LoadOriginalValuesAsync();
            var battery = checker.BatteryStatus;
            
            Console.WriteLine("Battery Status:");
            Console.WriteLine($"  State: {battery.State}");
            Console.WriteLine($"  Percentage: {battery.Percentage}%");
            Console.WriteLine($"  Time to Empty: {battery.TimeToEmpty}");
            Console.WriteLine($"  Time to Full: {battery.TimeToFull}");
            Console.WriteLine($"  Energy Rate: {battery.EnergyRate} W");
            Console.WriteLine($"  Present: {battery.IsPresent}");
        }

        private static async Task HandleWifiCommand(string[] args)
        {
            if (args.Length > 1 && args[1].ToLower() == "list")
            {
                var wifiChecker = new CheckDependencyCommand();
                var networks = await wifiChecker.GetWiFiNetworksAsync();
                
                Console.WriteLine("Available WiFi Networks:");
                Console.WriteLine("{0,-20} {1,-8} {2,-6} {3,-10} {4,-8} {5,-15}", 
                    "SSID", "Mode", "Chan", "Rate", "Signal", "Security");
                Console.WriteLine(new string('-', 80));
                
                foreach (var network in networks)
                {
                    var marker = network.IsActive ? "* " : "  ";
                    Console.WriteLine("{0}{1,-20} {2,-8} {3,-6} {4,-10} {5,-8} {6,-15}",
                        marker, network.SSID, network.Mode, network.Chan, network.Rate, 
                        network.Signal, network.Security);
                }
            }
            else
            {
                Console.WriteLine("Usage: UtilMan wifi list");
            }
        }

        private static async Task HandlePowerCommand(string[] args)
        {
            var powerChecker = new CheckDependencyCommand();
            if (args.Length > 1)
            {
                if (args[1].ToLower() == "get")
                {
                    var profile = await powerChecker.GetCurrentPowerProfileAsync();
                    Console.WriteLine($"Current power profile: {profile}");
                }
                else if (args[1].ToLower() == "set" && args.Length > 2)
                {
                    var changer = new ChangeValueCommand();
                    await changer.SetPowerProfileAsync(args[2]);
                    Console.WriteLine($"Power profile set to: {args[2]}");
                }
                else
                {
                    Console.WriteLine("Usage: UtilMan power get|set <profile>");
                }
            }
            else
            {
                Console.WriteLine("Usage: UtilMan power get|set <profile>");
            }
        }

        private static async Task BrightnessMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsBrightnessCtlAvailable)
            {
                MenuHelper.ShowError("Brightness Control", "brightnessctl is not available on this system.");
                return;
            }

            var currentBrightness = await checker.GetBrightnessPercentAsync();

            while (true)
            {
                var menuOptions = new List<string>
                {
                    $"Set brightness percentage (Current: {currentBrightness}%)",
                    "Quick set (0%, 25%, 50%, 75%, 100%)",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("BRIGHTNESS CONTROL", menuOptions);

                switch (choice)
                {
                    case 0:
                        var input = MenuHelper.GetUserInput("Enter brightness percentage (0-100)", currentBrightness.ToString());
                        if (int.TryParse(input, out int brightness) && brightness >= 0 && brightness <= 100)
                        {
                            var changer = new ChangeValueCommand();
                            await changer.SetBrightnessAsync(brightness);
                            currentBrightness = brightness;
                            MenuHelper.ShowMessage("Success", $"Brightness set to {brightness}%");
                        }
                        else
                        {
                            MenuHelper.ShowError("Invalid Input", "Please enter a number between 0 and 100.");
                        }
                        break;

                    case 1:
                        var quickOptions = new Dictionary<string, int>
                        {
                            ["0% (Off)"] = 0,
                            ["25%"] = 25,
                            ["50%"] = 50,
                            ["75%"] = 75,
                            ["100% (Maximum)"] = 100
                        };

                        var quickChoice = MenuHelper.ShowQuickSelectMenu("Quick brightness options", quickOptions);
                        if (quickChoice >= 0)
                        {
                            var selectedOption = quickOptions.ElementAt(quickChoice);
                            var quickBrightness = selectedOption.Value;
                            
                            var changer = new ChangeValueCommand();
                            await changer.SetBrightnessAsync(quickBrightness);
                            currentBrightness = quickBrightness;
                            MenuHelper.ShowMessage("Success", $"Brightness set to {quickBrightness}%");
                        }
                        break;

                    case 2:
                        return;
                    case -1:
                        return;
                }
            }
        }

        private static async Task VolumeMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsPactlAvailable)
            {
                MenuHelper.ShowError("Volume Control", "pactl (PulseAudio) is not available on this system.");
                return;
            }

            var currentVolume = await checker.GetVolumeAsync();

            while (true)
            {
                var menuOptions = new List<string>
                {
                    $"Set volume percentage (Current: {currentVolume}%)",
                    "Quick set (0%, 25%, 50%, 75%, 100%)",
                    "Mute/Unmute",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("VOLUME CONTROL", menuOptions);

                switch (choice)
                {
                    case 0:
                        var input = MenuHelper.GetUserInput("Enter volume percentage (0-100)", currentVolume.ToString());
                        if (int.TryParse(input, out int volume) && volume >= 0 && volume <= 100)
                        {
                            var changer = new ChangeValueCommand();
                            await changer.SetVolumeAsync(volume);
                            currentVolume = volume;
                            MenuHelper.ShowMessage("Success", $"Volume set to {volume}%");
                        }
                        else
                        {
                            MenuHelper.ShowError("Invalid Input", "Please enter a number between 0 and 100.");
                        }
                        break;

                    case 1:
                        var quickOptions = new Dictionary<string, int>
                        {
                            ["0% (Mute)"] = 0,
                            ["25%"] = 25,
                            ["50%"] = 50,
                            ["75%"] = 75,
                            ["100% (Maximum)"] = 100
                        };

                        var quickChoice = MenuHelper.ShowQuickSelectMenu("Quick volume options", quickOptions);
                        if (quickChoice >= 0)
                        {
                            var selectedOption = quickOptions.ElementAt(quickChoice);
                            var quickVolume = selectedOption.Value;
                            
                            var changer = new ChangeValueCommand();
                            await changer.SetVolumeAsync(quickVolume);
                            currentVolume = quickVolume;
                            MenuHelper.ShowMessage("Success", $"Volume set to {quickVolume}%");
                        }
                        break;

                    case 2:
                        var volumeChanger = new ChangeValueCommand();
                        await volumeChanger.SetVolumeAsync(0);
                        currentVolume = 0;
                        MenuHelper.ShowMessage("Success", "Volume muted (set to 0%)");
                        break;

                    case 3:
                        return;
                    case -1:
                        return;
                }
            }
        }

        private static async Task BatteryMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsUpowerAvailable)
            {
                MenuHelper.ShowError("Battery Status", "upower is not available on this system.");
                return;
            }

            await checker.LoadOriginalValuesAsync();
            var battery = checker.BatteryStatus;
            
            var batteryInfo = $"State: {battery.State}\n" +
                             $"Percentage: {battery.Percentage}%\n" +
                             $"Time to Empty: {battery.TimeToEmpty}\n" +
                             $"Time to Full: {battery.TimeToFull}\n" +
                             $"Energy Rate: {battery.EnergyRate} W\n" +
                             $"Present: {battery.IsPresent}";

            while (true)
            {
                var menuOptions = new List<string>
                {
                    "Refresh battery status",
                    "Back to main menu"
                };

                // Show battery info before menu
                MenuHelper.ShowMessage("Battery Status", batteryInfo, false);
                Console.WriteLine();

                var choice = MenuHelper.ShowArrowMenu("BATTERY STATUS", menuOptions);

                switch (choice)
                {
                    case 0:
                        // Refresh is automatic when menu loads again
                        break;
                    case 1:
                        return;
                    case -1:
                        return;
                }
            }
        }

        private static async Task WiFiMenu()
        {
            Console.WriteLine("=== WIFI NETWORKS ===");
            
            var checker = new CheckDependencyCommand();
            if (!checker.IsNmcliAvailable)
            {
                Console.WriteLine("nmcli (NetworkManager) is not available on this system.");
                return;
            }

            var networks = await checker.GetWiFiNetworksAsync();
            
            Console.WriteLine("Available WiFi Networks:");
            Console.WriteLine("{0,-20} {1,-8} {2,-6} {3,-10} {4,-8} {5,-15}", 
                "SSID", "Mode", "Chan", "Rate", "Signal", "Security");
            Console.WriteLine(new string('-', 80));
            
            foreach (var network in networks)
            {
                var marker = network.IsActive ? "* " : "  ";
                Console.WriteLine("{0}{1,-20} {2,-8} {3,-6} {4,-10} {5,-8} {6,-15}",
                    marker, network.SSID, network.Mode, network.Chan, network.Rate, 
                    network.Signal, network.Security);
            }

            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Refresh network list");
            Console.WriteLine("2. Back to main menu");
            Console.Write("Choose option: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    // Refresh is automatic when menu loads again
                    break;
                case "2":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        private static async Task PowerMenu()
        {
            Console.WriteLine("=== POWER PROFILES ===");
            
            var checker = new CheckDependencyCommand();
            if (!checker.IsPowerProfilesCtlAvailable)
            {
                Console.WriteLine("powerprofilesctl is not available on this system.");
                return;
            }

            var currentProfile = await checker.GetCurrentPowerProfileAsync();
            Console.WriteLine($"Current power profile: {currentProfile}");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Power Profile Options:");
                Console.WriteLine("1. Set performance mode");
                Console.WriteLine("2. Set balanced mode");
                Console.WriteLine("3. Set power-saver mode");
                Console.WriteLine("4. Show current profile");
                Console.WriteLine("5. Back to main menu");
                Console.Write("Choose option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        var changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("performance");
                        Console.WriteLine("Power profile set to performance");
                        break;

                    case "2":
                        changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("balanced");
                        Console.WriteLine("Power profile set to balanced");
                        break;

                    case "3":
                        changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("power-saver");
                        Console.WriteLine("Power profile set to power-saver");
                        break;

                    case "4":
                        currentProfile = await checker.GetCurrentPowerProfileAsync();
                        Console.WriteLine($"Current power profile: {currentProfile}");
                        break;

                    case "5":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine();
            }
        }

        public static async Task ShowSystemStatus()
        {
            var checker = new CheckDependencyCommand();
            await checker.LoadOriginalValuesAsync();
            
            Console.WriteLine("=== SYSTEM STATUS ===");
            
            // Battery
            var battery = checker.BatteryStatus;
            Console.WriteLine($"Battery: {battery.Percentage}% ({battery.State})");
            if (!string.IsNullOrEmpty(battery.TimeToEmpty))
                Console.WriteLine($"  Time to empty: {battery.TimeToEmpty}");
            
            // Brightness
            if (checker.IsBrightnessCtlAvailable)
            {
                var brightness = await checker.GetBrightnessPercentAsync();
                Console.WriteLine($"Brightness: {brightness}%");
            }
            else
            {
                Console.WriteLine("Brightness: Not available");
            }
            
            // Volume
            if (checker.IsPactlAvailable)
            {
                var volume = await checker.GetVolumeAsync();
                Console.WriteLine($"Volume: {volume}%");
            }
            else
            {
                Console.WriteLine("Volume: Not available");
            }
            
            // Power profile
            if (checker.IsPowerProfilesCtlAvailable)
            {
                var profile = await checker.GetCurrentPowerProfileAsync();
                Console.WriteLine($"Power Profile: {profile}");
            }
            else
            {
                Console.WriteLine("Power Profile: Not available");
            }
            
            Console.WriteLine();
            Console.WriteLine("=== DEPENDENCIES ===");
            Console.WriteLine($"brightnessctl: {(checker.IsBrightnessCtlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"pactl: {(checker.IsPactlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"upower: {(checker.IsUpowerAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"nmcli: {(checker.IsNmcliAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"powerprofilesctl: {(checker.IsPowerProfilesCtlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"procps (ps/free): {(checker.IsProcpsAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"lm-sensors: {(checker.IsLmSensorsAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"sysstat: {(checker.IsSysstatAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"systemctl: {(checker.IsSystemctlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"journalctl: {(checker.IsJournalctlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"ufw: {(checker.IsUfwAvailable ? "Available" : "Not available")}");
        }

        private static async Task HandleSystemMonitoringCommand(string command)
        {
            var checker = new CheckDependencyCommand();
            var systemInfo = await checker.GetSystemInfoAsync();

            switch (command.ToLower())
            {
                case "cpu":
                    Console.WriteLine("=== CPU INFORMATION ===");
                    Console.WriteLine($"Uptime: {systemInfo.Uptime}");
                    Console.WriteLine($"Load Average: {string.Join(", ", systemInfo.LoadAverage)}");
                    
                    if (systemInfo.Temperatures.Any())
                    {
                        Console.WriteLine("\nTemperatures:");
                        foreach (var temp in systemInfo.Temperatures)
                        {
                            Console.WriteLine($"  {temp.Key}: {temp.Value}");
                        }
                    }
                    break;

                case "memory":
                    Console.WriteLine("=== MEMORY INFORMATION ===");
                    if (systemInfo.MemoryInfo.Any())
                    {
                        Console.WriteLine($"Memory Total: {(systemInfo.MemoryInfo.TryGetValue("Total", out var total) ? total : "N/A")}");
                        Console.WriteLine($"Memory Used: {(systemInfo.MemoryInfo.TryGetValue("Used", out var used) ? used : "N/A")}");
                        Console.WriteLine($"Memory Free: {(systemInfo.MemoryInfo.TryGetValue("Free", out var free) ? free : "N/A")}");
                        Console.WriteLine($"Swap Total: {(systemInfo.MemoryInfo.TryGetValue("SwapTotal", out var swapTotal) ? swapTotal : "N/A")}");
                        Console.WriteLine($"Swap Used: {(systemInfo.MemoryInfo.TryGetValue("SwapUsed", out var swapUsed) ? swapUsed : "N/A")}");
                        Console.WriteLine($"Swap Free: {(systemInfo.MemoryInfo.TryGetValue("SwapFree", out var swapFree) ? swapFree : "N/A")}");
                    }
                    else
                    {
                        Console.WriteLine("Memory information not available");
                    }
                    break;

                case "disk":
                    Console.WriteLine("=== DISK USAGE ===");
                    Console.WriteLine("{0,-15} {1,-8} {2,-8} {3,-8} {4,-6} {5,-20}", 
                        "Filesystem", "Size", "Used", "Avail", "Use%", "Mount");
                    Console.WriteLine(new string('-', 80));
                    
                    foreach (var disk in systemInfo.DiskUsage)
                    {
                        Console.WriteLine("{0,-15} {1,-8} {2,-8} {3,-8} {4,-6} {5,-20}",
                            disk.Filesystem, disk.Size, disk.Used, disk.Available, disk.UsePercent, disk.MountPoint);
                    }
                    break;

                case "network":
                    Console.WriteLine("=== NETWORK INTERFACES ===");
                    Console.WriteLine("{0,-15} {1,-15}", "Interface", "IP Address");
                    Console.WriteLine(new string('-', 30));
                    
                    foreach (var net in systemInfo.NetworkInterfaces)
                    {
                        Console.WriteLine("{0,-15} {1,-15}", net.Interface, net.IPAddress);
                    }
                    break;
            }
        }

        private static async Task HandleServicesCommand(string[] args)
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsSystemctlAvailable)
            {
                Console.WriteLine("systemctl is not available on this system.");
                return;
            }

            if (args.Length > 1)
            {
                var action = args[1].ToLower();
                var serviceName = args.Length > 2 ? args[2] : "";

                if (!string.IsNullOrEmpty(serviceName))
                {
                    switch (action)
                    {
                        case "start":
                            await TerminalCommands.RunCommandAsync($"sudo systemctl start {serviceName}");
                            Console.WriteLine($"Starting service: {serviceName}");
                            break;
                        case "stop":
                            await TerminalCommands.RunCommandAsync($"sudo systemctl stop {serviceName}");
                            Console.WriteLine($"Stopping service: {serviceName}");
                            break;
                        case "restart":
                            await TerminalCommands.RunCommandAsync($"sudo systemctl restart {serviceName}");
                            Console.WriteLine($"Restarting service: {serviceName}");
                            break;
                        case "status":
                            var status = await TerminalCommands.RunCommandAsync($"systemctl status {serviceName}");
                            Console.WriteLine($"Service status for {serviceName}:");
                            Console.WriteLine(status);
                            break;
                        default:
                            Console.WriteLine("Usage: UtilMan services start|stop|restart|status <service_name>");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Usage: UtilMan services start|stop|restart|status <service_name>");
                }
            }
            else
            {
                var services = await checker.GetServicesAsync();
                Console.WriteLine("=== SERVICES ===");
                Console.WriteLine("{0,-25} {1,-8} {1,-8} {1,-8}", "Service", "Load", "Active", "Sub");
                Console.WriteLine(new string('-', 60));
                
                foreach (var service in services.Take(15))
                {
                    Console.WriteLine("{0,-25} {1,-8} {2,-8} {3,-8}", 
                        service.Name, service.Load, service.Active, service.Sub);
                }
            }
        }

        private static async Task HandleUsersCommand(string[] args)
        {
            var checker = new CheckDependencyCommand();
            var users = await checker.GetUsersAsync();
            
            Console.WriteLine("=== USER INFORMATION ===");
            Console.WriteLine("{0,-15} {1,-6} {1,-6} {1,-15} {1,-10} {1,-5}", 
                "Username", "UID", "GID", "Home", "Shell", "Online");
            Console.WriteLine(new string('-', 70));
            
            foreach (var user in users)
            {
                Console.WriteLine("{0,-15} {1,-6} {2,-6} {3,-15} {4,-10} {5,-5}",
                    user.Username, user.UID, user.GID, user.Home, user.Shell, user.IsLoggedIn ? "Yes" : "No");
            }
        }

        private static async Task HandleLogsCommand(string[] args)
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsJournalctlAvailable)
            {
                Console.WriteLine("journalctl is not available on this system.");
                return;
            }

            var logType = args.Length > 1 ? args[1].ToLower() : "system";
            var logs = await checker.GetRecentLogsAsync(logType);
            
            Console.WriteLine($"=== RECENT LOGS ({logType.ToUpper()}) ===");
            Console.WriteLine("{0,-20} {1}", "Timestamp", "Message");
            Console.WriteLine(new string('-', 80));
            
            foreach (var log in logs.Take(15))
            {
                Console.WriteLine("{0,-20} {1}", log.Timestamp, log.Message);
            }
        }

        private static async Task HandleFirewallCommand(string[] args)
        {
            var checker = new CheckDependencyCommand();
            var firewall = await checker.GetFirewallStatusAsync();
            
            Console.WriteLine("=== FIREWALL STATUS ===");
            
            if (!string.IsNullOrEmpty(firewall.UfwStatus))
            {
                Console.WriteLine($"UFW Status: {firewall.UfwStatus}");
            }
            
            if (!string.IsNullOrEmpty(firewall.Fail2banStatus))
            {
                Console.WriteLine($"Fail2ban Status: {firewall.Fail2banStatus}");
            }
            
            if (firewall.IptablesRules.Any())
            {
                Console.WriteLine("\nRecent iptables rules:");
                foreach (var rule in firewall.IptablesRules.Take(5))
                {
                    Console.WriteLine($"  {rule}");
                }
            }
        }

        private static async Task SystemMonitoringMenu()
        {
            var checker = new CheckDependencyCommand();
            
            while (true)
            {
                Console.WriteLine("=== SYSTEM MONITORING ===");
                Console.WriteLine("1. CPU Information");
                Console.WriteLine("2. Memory Usage");
                Console.WriteLine("3. Disk Usage");
                Console.WriteLine("4. Network Interfaces");
                Console.WriteLine("5. Full System Overview");
                Console.WriteLine("6. Back to main menu");
                Console.Write("Choose option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        await HandleSystemMonitoringCommand("cpu");
                        break;
                    case "2":
                        await HandleSystemMonitoringCommand("memory");
                        break;
                    case "3":
                        await HandleSystemMonitoringCommand("disk");
                        break;
                    case "4":
                        await HandleSystemMonitoringCommand("network");
                        break;
                    case "5":
                        var systemInfo = await checker.GetSystemInfoAsync();
                        await HandleSystemMonitoringCommand("cpu");
                        Console.WriteLine();
                        await HandleSystemMonitoringCommand("memory");
                        Console.WriteLine();
                        await HandleSystemMonitoringCommand("disk");
                        Console.WriteLine();
                        await HandleSystemMonitoringCommand("network");
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private static async Task ServiceManagementMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsSystemctlAvailable)
            {
                Console.WriteLine("systemctl is not available on this system.");
                return;
            }

            while (true)
            {
                Console.WriteLine("=== SERVICE MANAGEMENT ===");
                Console.WriteLine("1. List services");
                Console.WriteLine("2. Service status");
                Console.WriteLine("3. Start service");
                Console.WriteLine("4. Stop service");
                Console.WriteLine("5. Restart service");
                Console.WriteLine("6. Back to main menu");
                Console.Write("Choose option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        await HandleServicesCommand(new string[] { "services" });
                        break;
                    case "2":
                        Console.Write("Enter service name: ");
                        var serviceName = Console.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(serviceName))
                        {
                            await HandleServicesCommand(new string[] { "services", "status", serviceName });
                        }
                        break;
                    case "3":
                        Console.Write("Enter service name to start: ");
                        var startService = Console.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(startService))
                        {
                            await HandleServicesCommand(new string[] { "services", "start", startService });
                        }
                        break;
                    case "4":
                        Console.Write("Enter service name to stop: ");
                        var stopService = Console.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(stopService))
                        {
                            await HandleServicesCommand(new string[] { "services", "stop", stopService });
                        }
                        break;
                    case "5":
                        Console.Write("Enter service name to restart: ");
                        var restartService = Console.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(restartService))
                        {
                            await HandleServicesCommand(new string[] { "services", "restart", restartService });
                        }
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private static async Task UserManagementMenu()
        {
            while (true)
            {
                Console.WriteLine("=== USER MANAGEMENT ===");
                Console.WriteLine("1. List users");
                Console.WriteLine("2. Show logged in users");
                Console.WriteLine("3. Back to main menu");
                Console.Write("Choose option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        await HandleUsersCommand(new string[] { "users" });
                        break;
                    case "2":
                        var checker = new CheckDependencyCommand();
                        if (checker.IsProcpsAvailable)
                        {
                            var whoOutput = await TerminalCommands.RunCommandAsync("who");
                            Console.WriteLine("=== LOGGED IN USERS ===");
                            Console.WriteLine(whoOutput);
                        }
                        else
                        {
                            Console.WriteLine("who command is not available.");
                        }
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private static async Task LogManagementMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsJournalctlAvailable)
            {
                Console.WriteLine("journalctl is not available on this system.");
                return;
            }

            while (true)
            {
                Console.WriteLine("=== LOG MANAGEMENT ===");
                Console.WriteLine("1. System logs");
                Console.WriteLine("2. Kernel logs");
                Console.WriteLine("3. Boot logs");
                Console.WriteLine("4. Back to main menu");
                Console.Write("Choose option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        await HandleLogsCommand(new string[] { "logs", "system" });
                        break;
                    case "2":
                        await HandleLogsCommand(new string[] { "logs", "kernel" });
                        break;
                    case "3":
                        await HandleLogsCommand(new string[] { "logs", "boot" });
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private static async Task FirewallManagementMenu()
        {
            while (true)
            {
                Console.WriteLine("=== FIREWALL MANAGEMENT ===");
                Console.WriteLine("1. Show firewall status");
                Console.WriteLine("2. Back to main menu");
                Console.Write("Choose option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        await HandleFirewallCommand(new string[] { "firewall" });
                        break;
                    case "2":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private static async Task HandleDownloadCommand(string[] args)
        {
            if (args.Length > 1)
            {
                var action = args[1].ToLower();
                
                switch (action)
                {
                    case "all":
                        await DownloadScript.RunPackageInstallationAsync();
                        break;
                    case "individual":
                        await DownloadScript.InstallIndividualPackagesAsync();
                        break;
                    case "status":
                        await DownloadScript.ShowPackageStatusAsync();
                        break;
                    case "sensors":
                        await DownloadScript.SetupSensorsAsync();
                        break;
                    case "firewall":
                        await DownloadScript.ConfigureFirewallAsync();
                        break;
                    default:
                        Console.WriteLine("Usage: UtilMan install all|individual|status|sensors|firewall");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Usage: UtilMan install all|individual|status|sensors|firewall");
            }
        }

        private static async Task PackageInstallationMenu()
        {
            while (true)
            {
                Console.WriteLine("=== PACKAGE INSTALLATION ===");
                Console.WriteLine("1. Install all packages");
                Console.WriteLine("2. Install packages individually");
                Console.WriteLine("3. Show package status");
                Console.WriteLine("4. Setup hardware sensors");
                Console.WriteLine("5. Configure firewall");
                Console.WriteLine("6. Back to main menu");
                Console.Write("Choose option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        await DownloadScript.RunPackageInstallationAsync();
                        break;
                    case "2":
                        await DownloadScript.InstallIndividualPackagesAsync();
                        break;
                    case "3":
                        await DownloadScript.ShowPackageStatusAsync();
                        break;
                    case "4":
                        await DownloadScript.SetupSensorsAsync();
                        break;
                    case "5":
                        await DownloadScript.ConfigureFirewallAsync();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}
