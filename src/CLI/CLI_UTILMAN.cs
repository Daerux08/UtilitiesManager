using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManagerCLI
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
                    var checker = new CheckDependencyCommand();
                    await HandleSystemMonitoringCommand(checker, command);
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
            while (true)
            {
                var checker = new CheckDependencyCommand();
                await checker.CheckDependenciesAsync();
                
                if (!checker.IsNmcliAvailable)
                {
                    MenuHelper.ShowError("WiFi Networks", "nmcli (NetworkManager) is not available on this system.");
                    return;
                }

                var networks = await checker.GetWiFiNetworksAsync();
                
                if (networks.Count == 0)
                {
                    MenuHelper.ShowMessage("WiFi Networks", "No WiFi networks found. Please check your WiFi adapter.", false);
                    Console.WriteLine("\nPress any key to refresh or 'Q' to go back...");
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Q || key.Key == ConsoleKey.Escape)
                        return;
                    continue;
                }

                // Create menu options from WiFi networks
                var menuOptions = new List<string>();
                foreach (var network in networks)
                {
                    var status = network.IsActive ? "[CONNECTED]" : "[AVAILABLE]";
                    var signal = !string.IsNullOrEmpty(network.Signal) ? $" ({network.Signal})" : "";
                    menuOptions.Add($"{status} {network.SSID}{signal}");
                }
                
                // Add control options at the end
                menuOptions.Add("Refresh networks");
                menuOptions.Add(" Back to main menu");

                var choice = MenuHelper.ShowArrowMenu("WiFi Networks", menuOptions);

                if (choice == -1 || choice == menuOptions.Count - 1)
                {
                    // User cancelled or selected "Back"
                    return;
                }
                else if (choice == menuOptions.Count - 2)
                {
                    // User selected "Refresh"
                    continue;
                }
                else if (choice >= 0 && choice < networks.Count)
                {
                    // User selected a WiFi network
                    var selectedNetwork = networks[choice];
                    await HandleWiFiSelection(selectedNetwork);
                }
            }
        }

        private static async Task HandleWiFiSelection(WiFiInfo network)
        {
            if (network.IsActive)
            {
                // Already connected - show disconnect option
                var options = new List<string>
                {
                    $"Connected to {network.SSID}",
                    "Disconnect from this network",
                    "Back to network list"
                };
                
                var choice = MenuHelper.ShowArrowMenu("WiFi Connection", options);
                
                if (choice == 1)
                {
                    await DisconnectFromWiFi(network.SSID);
                }
                // choice 0 or 2 (or -1) just returns to network list
            }
            else
            {
                // Not connected - attempt to connect
                await ConnectToWiFi(network);
            }
        }

        private static async Task ConnectToWiFi(WiFiInfo network)
        {
            try
            {
                Console.Clear();
                Console.WriteLine($"=== Connecting to {network.SSID} ===");
                Console.WriteLine();
                Console.WriteLine("Attempting connection...");
                
                // First try to connect without password
                var command = $"nmcli device wifi connect \"{network.SSID}\"";
                var result = await TerminalCommands.RunCommandWithResultAsync(command);

                if (result.IsSuccess)
                {
                    MenuHelper.ShowMessage("Connection Successful", $"Successfully connected to {network.SSID}");
                    return;
                }

                // Check if password is required
                if (result.CombinedOutput.Contains("Secrets were required", StringComparison.OrdinalIgnoreCase) ||
                    result.CombinedOutput.Contains("no-secrets", StringComparison.OrdinalIgnoreCase) ||
                    result.CombinedOutput.Contains("Secrets", StringComparison.OrdinalIgnoreCase) ||
                    result.ExitCode == 7)
                {
                    // Prompt for password
                    var password = MenuHelper.GetUserInput($"Enter password for {network.SSID}");
                    
                    if (!string.IsNullOrEmpty(password))
                    {
                        Console.WriteLine("Attempting connection with password...");
                        var commandWithPassword = $"nmcli device wifi connect \"{network.SSID}\" password \"{password}\"";
                        var resultWithPassword = await TerminalCommands.RunCommandWithResultAsync(commandWithPassword);
                        
                        if (resultWithPassword.IsSuccess)
                        {
                            MenuHelper.ShowMessage("Connection Successful", $"Successfully connected to {network.SSID}");
                        }
                        else
                        {
                            MenuHelper.ShowError("Connection Failed", $"Failed to connect to {network.SSID}:\n{resultWithPassword.CombinedOutput}");
                        }
                    }
                    else
                    {
                        MenuHelper.ShowMessage("Connection Cancelled", "No password provided.");
                    }
                }
                else
                {
                    MenuHelper.ShowError("Connection Failed", $"Failed to connect to {network.SSID}:\n{result.CombinedOutput}");
                }
            }
            catch (Exception ex)
            {
                MenuHelper.ShowError("Connection Error", $"Error connecting to {network.SSID}: {ex.Message}");
            }
        }

        private static async Task DisconnectFromWiFi(string ssid)
        {
            try
            {
                Console.Clear();
                Console.WriteLine($"=== Disconnecting from {ssid} ===");
                Console.WriteLine();
                Console.WriteLine("Attempting disconnection...");
                
                var command = $"nmcli connection down \"{ssid}\"";
                var result = await TerminalCommands.RunCommandWithResultAsync(command);
                
                if (result.IsSuccess)
                {
                    MenuHelper.ShowMessage("Disconnection Successful", $"Successfully disconnected from {ssid}");
                }
                else
                {
                    MenuHelper.ShowError("Disconnection Failed", $"Failed to disconnect from {ssid}:\n{result.CombinedOutput}");
                }
            }
            catch (Exception ex)
            {
                MenuHelper.ShowError("Disconnection Error", $"Error disconnecting from {ssid}: {ex.Message}");
            }
        }

        private static async Task PowerMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsPowerProfilesCtlAvailable)
            {
                MenuHelper.ShowError("Power Profiles", "powerprofilesctl is not available on this system.");
                return;
            }

            while (true)
            {
                var currentProfile = await checker.GetCurrentPowerProfileAsync();
                
                var menuOptions = new List<string>
                {
                    $"Set performance mode {(currentProfile == "performance" ? "[CURRENT]" : "")}",
                    $"Set balanced mode {(currentProfile == "balanced" ? "[CURRENT]" : "")}",
                    $"Set power-saver mode {(currentProfile == "power-saver" ? "[CURRENT]" : "")}",
                    "Show current profile",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("POWER PROFILES", menuOptions);

                switch (choice)
                {
                    case 0:
                        var changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("performance");
                        MenuHelper.ShowMessage("Success", "Power profile set to performance");
                        break;

                    case 1:
                        changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("balanced");
                        MenuHelper.ShowMessage("Success", "Power profile set to balanced");
                        break;

                    case 2:
                        changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("power-saver");
                        MenuHelper.ShowMessage("Success", "Power profile set to power-saver");
                        break;

                    case 3:
                        currentProfile = await checker.GetCurrentPowerProfileAsync();
                        MenuHelper.ShowMessage("Current Profile", $"Current power profile: {currentProfile}");
                        break;

                    case 4:
                        return;

                    case -1:
                        return;
                }
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

        private static async Task HandleSystemMonitoringCommand(CheckDependencyCommand checker, string command)
        {
            await checker.CheckDependenciesAsync();
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
            await checker.CheckDependenciesAsync();
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
            await checker.CheckDependenciesAsync();

            while (true)
            {
                var menuOptions = new List<string>
                {
                    "CPU Information - Usage, load, temperature",
                    "Memory Usage - RAM and swap usage", 
                    "Disk Usage - Storage space and mount points",
                    "Network Interfaces - IP addresses and connections",
                    "Full System Overview - All information at once",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("SYSTEM MONITORING", menuOptions);

                switch (choice)
                {
                    case 0:
                        await HandleSystemMonitoringCommand(checker, "cpu");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        await HandleSystemMonitoringCommand(checker, "memory");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        await HandleSystemMonitoringCommand(checker, "disk");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 3:
                        await HandleSystemMonitoringCommand(checker, "network");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 4:
                        var systemInfo = await checker.GetSystemInfoAsync();
                        
                        // Display Full System Overview
                        Console.Clear();
                        Console.WriteLine("=== FULL SYSTEM OVERVIEW ===");
                        Console.WriteLine();
                        
                        // CPU Information
                        await HandleSystemMonitoringCommand(checker, "cpu");
                        Console.WriteLine();
                        
                        // Memory Information  
                        await HandleSystemMonitoringCommand(checker, "memory");
                        Console.WriteLine();
                        
                        // Disk Information
                        await HandleSystemMonitoringCommand(checker, "disk");
                        Console.WriteLine();
                        
                        // Network Information
                        await HandleSystemMonitoringCommand(checker, "network");
                        
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 5:
                        return;
                    case -1:
                        return;
                }
            }
        }

        private static async Task ServiceManagementMenu()
        {
            Console.WriteLine("DEBUG: Starting Service Management Menu");
            var checker = new CheckDependencyCommand();
            await checker.CheckDependenciesAsync();
            Console.WriteLine($"DEBUG: IsSystemctlAvailable = {checker.IsSystemctlAvailable}");
            
            if (!checker.IsSystemctlAvailable)
            {
                MenuHelper.ShowError("Service Management", "systemctl is not available on this system.");
                return;
            }

            while (true)
            {
                var menuOptions = new List<string>
                {
                    "List all services",
                    "Check specific service status",
                    "Start a service",
                    "Stop a service", 
                    "Restart a service",
                    "What are services?",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("SERVICE MANAGEMENT", menuOptions);

                switch (choice)
                {
                    case 0:
                        await HandleServicesCommand(new string[] { "services" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        var serviceName = MenuHelper.GetUserInput("Enter service name");
                        if (!string.IsNullOrEmpty(serviceName))
                        {
                            await HandleServicesCommand(new string[] { "services", "status", serviceName });
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        var startService = MenuHelper.GetUserInput("Enter service name to start");
                        if (!string.IsNullOrEmpty(startService))
                        {
                            await HandleServicesCommand(new string[] { "services", "start", startService });
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 3:
                        var stopService = MenuHelper.GetUserInput("Enter service name to stop");
                        if (!string.IsNullOrEmpty(stopService))
                        {
                            await HandleServicesCommand(new string[] { "services", "stop", stopService });
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 4:
                        var restartService = MenuHelper.GetUserInput("Enter service name to restart");
                        if (!string.IsNullOrEmpty(restartService))
                        {
                            await HandleServicesCommand(new string[] { "services", "restart", restartService });
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 5:
                        var helpText = @"=== WHAT ARE SYSTEMD SERVICES? ===

Services (systemd services) are background programs that run on your Linux system.
They manage core system functionality and applications.

COMMON SERVICES:
• sshd - Secure Shell server for remote access
• nginx - Web server
• docker - Container management
• ufw - Firewall management
• NetworkManager - Network connections
• cron - Scheduled tasks

SERVICE STATES:
• active (running) - Service is currently running
• inactive (dead) - Service is stopped
• enabled - Service starts automatically on boot
• disabled - Service must be started manually

WHY MANAGE SERVICES?
• Fix problems by restarting problematic services
• Improve security by stopping unused services  
• Save resources by disabling unnecessary services
• Debug system issues by checking service status

TIPS:
• Be careful when stopping system-critical services
• Use 'status' first before making changes
• Some services require sudo privileges to control";
                        
                        MenuHelper.ShowMessage("About Services", helpText);
                        break;
                    case 6:
                        return;
                    case -1:
                        return;
                }
            }
        }

        private static async Task UserManagementMenu()
        {
            while (true)
            {
                var menuOptions = new List<string>
                {
                    "List users",
                    "Show logged in users",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("USER MANAGEMENT", menuOptions);

                switch (choice)
                {
                    case 0:
                        await HandleUsersCommand(new string[] { "users" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        var checker = new CheckDependencyCommand();
                        if (checker.IsProcpsAvailable)
                        {
                            var whoOutput = await TerminalCommands.RunCommandAsync("who");
                            MenuHelper.ShowMessage("Logged In Users", whoOutput);
                        }
                        else
                        {
                            MenuHelper.ShowError("User Management", "who command is not available on this system.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        return;
                    case -1:
                        return;
                }
            }
        }

        private static async Task LogManagementMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsJournalctlAvailable)
            {
                MenuHelper.ShowError("Log Management", "journalctl is not available on this system.");
                return;
            }

            while (true)
            {
                var menuOptions = new List<string>
                {
                    "System logs",
                    "Kernel logs",
                    "Boot logs",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("LOG MANAGEMENT", menuOptions);

                switch (choice)
                {
                    case 0:
                        await HandleLogsCommand(new string[] { "logs", "system" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        await HandleLogsCommand(new string[] { "logs", "kernel" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        await HandleLogsCommand(new string[] { "logs", "boot" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 3:
                        return;
                    case -1:
                        return;
                }
            }
        }

        private static async Task FirewallManagementMenu()
        {
            while (true)
            {
                var menuOptions = new List<string>
                {
                    "Show firewall status",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("FIREWALL MANAGEMENT", menuOptions);

                switch (choice)
                {
                    case 0:
                        await HandleFirewallCommand(new string[] { "firewall" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        return;
                    case -1:
                        return;
                }
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
                var menuOptions = new List<string>
                {
                    "Install all packages",
                    "Install packages individually",
                    "Show package status",
                    "Setup hardware sensors",
                    "Configure firewall",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("PACKAGE INSTALLATION", menuOptions);

                switch (choice)
                {
                    case 0:
                        await DownloadScript.RunPackageInstallationAsync();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        await DownloadScript.InstallIndividualPackagesAsync();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        await DownloadScript.ShowPackageStatusAsync();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 3:
                        await DownloadScript.SetupSensorsAsync();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 4:
                        await DownloadScript.ConfigureFirewallAsync();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 5:
                        return;
                    case -1:
                        return;
                }
            }
        }
    }
}
