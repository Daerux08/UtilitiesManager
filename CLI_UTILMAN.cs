using System;
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

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    Console.WriteLine("Run 'UtilMan help' for available commands or run without arguments for interactive mode.");
                    break;
            }
        }

        public static async Task RunInteractiveMode()
        {
            Console.WriteLine("=== UTILITIES MANAGER - INTERACTIVE MODE ===");
            Console.WriteLine("Linux System Utility Manager");
            Console.WriteLine();

            while (true)
            {
                await ShowSystemStatus();
                Console.WriteLine();
                ShowMainMenu();
                
                Console.Write("Select an option (or 'q' to quit): ");
                var choice = Console.ReadLine()?.ToLower().Trim();

                switch (choice)
                {
                    case "1":
                        await BrightnessMenu();
                        break;
                    case "2":
                        await VolumeMenu();
                        break;
                    case "3":
                        await BatteryMenu();
                        break;
                    case "4":
                        await WiFiMenu();
                        break;
                    case "5":
                        await PowerMenu();
                        break;
                    case "6":
                        Help.ShowAllHelp();
                        break;
                    case "r":
                    case "refresh":
                        continue;
                    case "q":
                    case "quit":
                    case "exit":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                if (choice != "q" && choice != "quit" && choice != "exit")
                {
                    Console.WriteLine("\nPress Enter to continue...");
                    Console.ReadLine();
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
            Console.WriteLine("6. Help & Documentation");
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
            Console.WriteLine("=== BRIGHTNESS CONTROL ===");
            
            var checker = new CheckDependencyCommand();
            if (!checker.IsBrightnessCtlAvailable)
            {
                Console.WriteLine("brightnessctl is not available on this system.");
                return;
            }

            var currentBrightness = await checker.GetBrightnessPercentAsync();
            Console.WriteLine($"Current brightness: {currentBrightness}%");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Brightness Options:");
                Console.WriteLine("1. Set brightness percentage");
                Console.WriteLine("2. Quick set (0%, 25%, 50%, 75%, 100%)");
                Console.WriteLine("3. Back to main menu");
                Console.Write("Choose option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter brightness percentage (0-100): ");
                        if (int.TryParse(Console.ReadLine(), out int brightness) && brightness >= 0 && brightness <= 100)
                        {
                            var changer = new ChangeValueCommand();
                            await changer.SetBrightnessAsync(brightness);
                            Console.WriteLine($"Brightness set to {brightness}%");
                        }
                        else
                        {
                            Console.WriteLine("Invalid percentage. Please enter a number between 0 and 100.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("Quick brightness options:");
                        Console.WriteLine("a. 0% (Off)");
                        Console.WriteLine("b. 25%");
                        Console.WriteLine("c. 50%");
                        Console.WriteLine("d. 75%");
                        Console.WriteLine("e. 100% (Maximum)");
                        Console.Write("Choose option: ");

                        var quickChoice = Console.ReadLine()?.ToLower().Trim();
                        var quickBrightness = quickChoice switch
                        {
                            "a" => 0,
                            "b" => 25,
                            "c" => 50,
                            "d" => 75,
                            "e" => 100,
                            _ => -1
                        };

                        if (quickBrightness >= 0)
                        {
                            var changer = new ChangeValueCommand();
                            await changer.SetBrightnessAsync(quickBrightness);
                            Console.WriteLine($"Brightness set to {quickBrightness}%");
                        }
                        else
                        {
                            Console.WriteLine("Invalid option.");
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

        private static async Task VolumeMenu()
        {
            Console.WriteLine("=== VOLUME CONTROL ===");
            
            var checker = new CheckDependencyCommand();
            if (!checker.IsPactlAvailable)
            {
                Console.WriteLine("pactl (PulseAudio) is not available on this system.");
                return;
            }

            var currentVolume = await checker.GetVolumeAsync();
            Console.WriteLine($"Current volume: {currentVolume}%");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Volume Options:");
                Console.WriteLine("1. Set volume percentage");
                Console.WriteLine("2. Quick set (0%, 25%, 50%, 75%, 100%)");
                Console.WriteLine("3. Mute/Unmute");
                Console.WriteLine("4. Back to main menu");
                Console.Write("Choose option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter volume percentage (0-100): ");
                        if (int.TryParse(Console.ReadLine(), out int volume) && volume >= 0 && volume <= 100)
                        {
                            var changer = new ChangeValueCommand();
                            await changer.SetVolumeAsync(volume);
                            Console.WriteLine($"Volume set to {volume}%");
                        }
                        else
                        {
                            Console.WriteLine("Invalid percentage. Please enter a number between 0 and 100.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("Quick volume options:");
                        Console.WriteLine("a. 0% (Mute)");
                        Console.WriteLine("b. 25%");
                        Console.WriteLine("c. 50%");
                        Console.WriteLine("d. 75%");
                        Console.WriteLine("e. 100% (Maximum)");
                        Console.Write("Choose option: ");

                        var quickChoice = Console.ReadLine()?.ToLower().Trim();
                        var quickVolume = quickChoice switch
                        {
                            "a" => 0,
                            "b" => 25,
                            "c" => 50,
                            "d" => 75,
                            "e" => 100,
                            _ => -1
                        };

                        if (quickVolume >= 0)
                        {
                            var changer = new ChangeValueCommand();
                            await changer.SetVolumeAsync(quickVolume);
                            Console.WriteLine($"Volume set to {quickVolume}%");
                        }
                        else
                        {
                            Console.WriteLine("Invalid option.");
                        }
                        break;

                    case "3":
                        var volumeChanger = new ChangeValueCommand();
                        await volumeChanger.SetVolumeAsync(0);
                        Console.WriteLine("Volume muted (set to 0%)");
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

        private static async Task BatteryMenu()
        {
            Console.WriteLine("=== BATTERY STATUS ===");
            
            var checker = new CheckDependencyCommand();
            if (!checker.IsUpowerAvailable)
            {
                Console.WriteLine("upower is not available on this system.");
                return;
            }

            await checker.LoadOriginalValuesAsync();
            var battery = checker.BatteryStatus;
            
            Console.WriteLine($"Battery Status:");
            Console.WriteLine($"  State: {battery.State}");
            Console.WriteLine($"  Percentage: {battery.Percentage}%");
            Console.WriteLine($"  Time to Empty: {battery.TimeToEmpty}");
            Console.WriteLine($"  Time to Full: {battery.TimeToFull}");
            Console.WriteLine($"  Energy Rate: {battery.EnergyRate} W");
            Console.WriteLine($"  Present: {battery.IsPresent}");
            
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Refresh battery status");
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
        }
    }
}
