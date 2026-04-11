using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace UtilitiesManager
{
    public static class WifiService
    {
        public static async Task HandleWifiCommand(string[] args)
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
        public static async Task MenuService(CheckDependencyCommand checkerParam)
        {
            CheckDependencyCommand.CheckDependencies();
            while (true)
            {
                if (!checkerParam.IsNmcliAvailable)
                {
                    MenuEngine.ShowError("WiFi Networks", "nmcli (NetworkManager) is not available on this system.");
                    return;
                }

                var networks = await checkerParam.GetWiFiNetworksAsync();

                if (networks.Count == 0)
                {
                    MenuEngine.ShowMessage("WiFi Networks", "No WiFi networks found. Please check your WiFi adapter.", false);
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
                menuOptions.Add("🔄 Refresh networks");
                menuOptions.Add("⬅ Back to main menu");

                var choice = MenuEngine.ShowArrowMenu("WiFi Networks", menuOptions);

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

        public static async Task HandleWiFiSelection(WiFiInfo network)
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

                var choice = MenuEngine.ShowArrowMenu("WiFi Connection", options);

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

        public static async Task ConnectToWiFi(WiFiInfo network)
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
                    MenuEngine.ShowMessage("Connection Successful", $"Successfully connected to {network.SSID}");
                    return;
                }

                // Check if password is required
                if (result.CombinedOutput.Contains("Secrets were required", StringComparison.OrdinalIgnoreCase) ||
                    result.CombinedOutput.Contains("no-secrets", StringComparison.OrdinalIgnoreCase) ||
                    result.CombinedOutput.Contains("Secrets", StringComparison.OrdinalIgnoreCase) ||
                    result.ExitCode == 7)
                {
                    // Prompt for password
                    var password = MenuEngine.GetUserInput($"Enter password for {network.SSID}");

                    if (!string.IsNullOrEmpty(password))
                    {
                        Console.WriteLine("Attempting connection with password...");
                        var commandWithPassword = $"nmcli device wifi connect \"{network.SSID}\" password \"{password}\"";
                        var resultWithPassword = await TerminalCommands.RunCommandWithResultAsync(commandWithPassword);

                        if (resultWithPassword.IsSuccess)
                        {
                            MenuEngine.ShowMessage("Connection Successful", $"Successfully connected to {network.SSID}");
                        }
                        else
                        {
                            MenuEngine.ShowError("Connection Failed", $"Failed to connect to {network.SSID}:\n{resultWithPassword.CombinedOutput}");
                        }
                    }
                    else
                    {
                        MenuEngine.ShowMessage("Connection Cancelled", "No password provided.");
                    }
                }
                else
                {
                    MenuEngine.ShowError("Connection Failed", $"Failed to connect to {network.SSID}:\n{result.CombinedOutput}");
                }
            }
            catch (Exception ex)
            {
                MenuEngine.ShowError("Connection Error", $"Error connecting to {network.SSID}: {ex.Message}");
            }
        }

        public static async Task DisconnectFromWiFi(string ssid)
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
                    MenuEngine.ShowMessage("Disconnection Successful", $"Successfully disconnected from {ssid}");
                }
                else
                {
                    MenuEngine.ShowError("Disconnection Failed", $"Failed to disconnect from {ssid}:\n{result.CombinedOutput}");
                }
            }
            catch (Exception ex)
            {
                MenuEngine.ShowError("Disconnection Error", $"Error disconnecting from {ssid}: {ex.Message}");
            }
        }
    }
}
