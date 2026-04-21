using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class BluetoothService
    {
        public static async Task HandleBluetoothCommand(string[] args)
        {
            if (args.Length > 1)
            {
                var subCommand = args[1].ToLower();
                var checker = new CheckDependencyCommand();
                checker.CheckDependencies();

                switch (subCommand)
                {
                    case "list":
                        await ListDevices(checker);
                        break;

                    case "scan":
                        await ScanDevices(checker);
                        break;

                    case "connect":
                        if (args.Length > 2)
                            await ConnectDevice(checker, args[2]);
                        else
                            Console.WriteLine("Usage: UtilMan bluetooth connect <device_address>");
                        break;

                    case "disconnect":
                        if (args.Length > 2)
                            await DisconnectDevice(checker, args[2]);
                        else
                            Console.WriteLine("Usage: UtilMan bluetooth disconnect <device_address>");
                        break;

                    case "pair":
                        if (args.Length > 2)
                            await PairDevice(checker, args[2]);
                        else
                            Console.WriteLine("Usage: UtilMan bluetooth pair <device_address>");
                        break;

                    case "power":
                        if (args.Length > 2 && args[2].ToLower() == "on")
                            await PowerOn(checker);
                        else if (args.Length > 2 && args[2].ToLower() == "off")
                            await PowerOff(checker);
                        else
                            Console.WriteLine("Usage: UtilMan bluetooth power <on|off>");
                        break;

                    default:
                        Console.WriteLine("Unknown bluetooth subcommand. Available: list, scan, connect, disconnect, pair, power");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Usage: UtilMan bluetooth <list|scan|connect|disconnect|pair|power>");
                Console.WriteLine("  list              - List paired/available devices");
                Console.WriteLine("  scan              - Scan for nearby devices");
                Console.WriteLine("  connect <addr>    - Connect to device");
                Console.WriteLine("  disconnect <addr> - Disconnect from device");
                Console.WriteLine("  pair <addr>       - Pair with device");
                Console.WriteLine("  power <on|off>    - Turn Bluetooth on/off");
            }
        }

        private static async Task ListDevices(CheckDependencyCommand checker)
        {
            if (!checker.IsBluetoothctlAvailable)
            {
                Console.WriteLine("Error: bluetoothctl is not available.");
                return;
            }

            var devices = await checker.GetBluetoothDevicesAsync();

            if (devices.Count == 0)
            {
                Console.WriteLine("No Bluetooth devices found.");
                return;
            }

            Console.WriteLine("Bluetooth Devices:");
            Console.WriteLine("{0,-20} {1,-20} {2,-12} {3,-10} {4,-10} {5,-10}",
                "Name", "Address", "Type", "Connected", "Paired", "Trusted");
            Console.WriteLine(new string('-', 90));

            foreach (var device in devices)
            {
                var marker = device.Connected ? "* " : "  ";
                Console.WriteLine("{0}{1,-20} {2,-20} {3,-12} {4,-10} {5,-10} {6,-10}",
                    marker, device.Name, device.Address, device.Type,
                    device.Connected ? "Yes" : "No",
                    device.Paired ? "Yes" : "No",
                    device.Trusted ? "Yes" : "No");
            }
        }

        private static async Task ScanDevices(CheckDependencyCommand checker)
        {
            if (!checker.IsBluetoothctlAvailable)
            {
                Console.WriteLine("Error: bluetoothctl is not available.");
                return;
            }

            Console.WriteLine("Scanning for Bluetooth devices (5 seconds)...");
            
            var changer = new ChangeValueCommand();
            await changer.ScanBluetoothDevicesAsync();
            await Task.Delay(5000);
            await changer.StopBluetoothScanAsync();

            Console.WriteLine("Scan complete. Use 'list' to see found devices.");
        }

        private static async Task ConnectDevice(CheckDependencyCommand checker, string address)
        {
            if (!checker.IsBluetoothctlAvailable)
            {
                Console.WriteLine("Error: bluetoothctl is not available.");
                return;
            }

            Console.WriteLine($"Connecting to {address}...");
            var changer = new ChangeValueCommand();
            var success = await changer.ConnectToDeviceAsync(address);

            if (success)
                Console.WriteLine("Connected successfully.");
            else
                Console.WriteLine("Connection failed.");
        }

        private static async Task DisconnectDevice(CheckDependencyCommand checker, string address)
        {
            if (!checker.IsBluetoothctlAvailable)
            {
                Console.WriteLine("Error: bluetoothctl is not available.");
                return;
            }

            Console.WriteLine($"Disconnecting from {address}...");
            var changer = new ChangeValueCommand();
            var success = await changer.DisconnectDeviceAsync(address);

            if (success)
                Console.WriteLine("Disconnected successfully.");
            else
                Console.WriteLine("Disconnection failed.");
        }

        private static async Task PairDevice(CheckDependencyCommand checker, string address)
        {
            if (!checker.IsBluetoothctlAvailable)
            {
                Console.WriteLine("Error: bluetoothctl is not available.");
                return;
            }

            Console.WriteLine($"Pairing with {address}...");
            Console.WriteLine("Note: Device must be in pairing mode.");
            
            var changer = new ChangeValueCommand();
            var success = await changer.PairDeviceAsync(address);

            if (success)
                Console.WriteLine("Paired successfully.");
            else
                Console.WriteLine("Pairing failed. PIN may be required (use interactive mode).");
        }

        private static async Task PowerOn(CheckDependencyCommand checker)
        {
            if (!checker.IsBluetoothctlAvailable)
            {
                Console.WriteLine("Error: bluetoothctl is not available.");
                return;
            }

            var changer = new ChangeValueCommand();
            var success = await changer.ToggleBluetoothAsync(true);

            if (success)
                Console.WriteLine("Bluetooth powered on.");
            else
                Console.WriteLine("Failed to power on Bluetooth.");
        }

        private static async Task PowerOff(CheckDependencyCommand checker)
        {
            if (!checker.IsBluetoothctlAvailable)
            {
                Console.WriteLine("Error: bluetoothctl is not available.");
                return;
            }

            var changer = new ChangeValueCommand();
            var success = await changer.ToggleBluetoothAsync(false);

            if (success)
                Console.WriteLine("Bluetooth powered off.");
            else
                Console.WriteLine("Failed to power off Bluetooth.");
        }

        public static async Task MenuService(CheckDependencyCommand checkerParam)
        {
            checkerParam.CheckDependencies();
            while (true)
            {
                if (!checkerParam.IsBluetoothctlAvailable)
                {
                    MenuEngine.ShowError("Bluetooth", "bluetoothctl (BlueZ) is not available on this system.");
                    return;
                }

                var devices = await checkerParam.GetBluetoothDevicesAsync();

                if (devices.Count == 0)
                {
                    MenuEngine.ShowMessage("Bluetooth", "No Bluetooth devices found. Try scanning first.", false);
                    Console.WriteLine("\n[S] Scan for devices | [R] Refresh | [Q] Go back");
                    var key = Console.ReadKey(true);
                    
                    if (key.Key == ConsoleKey.Q || key.Key == ConsoleKey.Escape)
                        return;
                    else if (key.Key == ConsoleKey.S)
                    {
                        await ScanDevices(checkerParam);
                        continue;
                    }
                    else if (key.Key == ConsoleKey.R)
                        continue;
                    
                    continue;
                }

                // Create menu options from devices
                var menuOptions = new List<string>();
                foreach (var device in devices)
                {
                    var status = device.Connected ? "[CONNECTED]" : (device.Paired ? "[PAIRED]" : "[AVAILABLE]");
                    var rssi = !string.IsNullOrEmpty(device.RSSI) ? $" ({device.RSSI})" : "";
                    menuOptions.Add($"{status} {device.Name}{rssi}");
                }

                // Add control options
                menuOptions.Add("🔍 Scan for new devices");
                menuOptions.Add("🔄 Refresh device list");
                menuOptions.Add("⬅ Back to main menu");

                var choice = MenuEngine.ShowArrowMenu("Bluetooth Devices", menuOptions);

                if (choice == -1 || choice == menuOptions.Count - 1)
                    return;
                else if (choice == menuOptions.Count - 2)
                    continue; // Refresh
                else if (choice == menuOptions.Count - 3)
                {
                    await ScanDevices(checkerParam);
                    continue;
                }
                else if (choice >= 0 && choice < devices.Count)
                {
                    var selectedDevice = devices[choice];
                    await HandleDeviceSelection(selectedDevice, checkerParam);
                }
            }
        }

        public static async Task HandleDeviceSelection(BluetoothInfo device, CheckDependencyCommand checker)
        {
            var changer = new ChangeValueCommand();

            if (device.Connected)
            {
                // Device is connected
                var options = new List<string>
                {
                    $"Connected to {device.Name}",
                    "Disconnect from this device",
                    "Trust this device",
                    "Back to device list"
                };

                var choice = MenuEngine.ShowArrowMenu("Bluetooth Connection", options);

                if (choice == 1)
                {
                    Console.WriteLine($"Disconnecting from {device.Name}...");
                    await changer.DisconnectDeviceAsync(device.Address);
                    MenuEngine.ShowMessage("Disconnected", $"Disconnected from {device.Name}");
                }
                else if (choice == 2)
                {
                    Console.WriteLine($"Trusting {device.Name}...");
                    await changer.TrustDeviceAsync(device.Address);
                    MenuEngine.ShowMessage("Trusted", $"{device.Name} is now trusted");
                }
            }
            else if (device.Paired)
            {
                // Device is paired but not connected
                var options = new List<string>
                {
                    $"Paired with {device.Name} (not connected)",
                    "Connect to this device",
                    "Remove/trust options...",
                    "Back to device list"
                };

                var choice = MenuEngine.ShowArrowMenu("Bluetooth Connection", options);

                if (choice == 1)
                {
                    Console.WriteLine($"Connecting to {device.Name}...");
                    var success = await changer.ConnectToDeviceAsync(device.Address);
                    
                    if (success)
                        MenuEngine.ShowMessage("Connected", $"Connected to {device.Name}");
                    else
                        MenuEngine.ShowError("Connection Failed", $"Failed to connect to {device.Name}");
                }
                else if (choice == 2)
                {
                    await HandleDeviceOptions(device, checker);
                }
            }
            else
            {
                // Device is not paired
                var options = new List<string>
                {
                    $"{device.Name} (not paired)",
                    "Pair and connect to this device",
                    "Back to device list"
                };

                var choice = MenuEngine.ShowArrowMenu("Bluetooth Connection", options);

                if (choice == 1)
                {
                    Console.WriteLine($"Pairing with {device.Name}...");
                    Console.WriteLine("Make sure the device is in pairing mode.");
                    
                    var success = await changer.PairDeviceAsync(device.Address);
                    
                    if (success)
                    {
                        MenuEngine.ShowMessage("Paired", $"Successfully paired with {device.Name}");
                        Console.WriteLine("Attempting to connect...");
                        await changer.ConnectToDeviceAsync(device.Address);
                    }
                    else
                    {
                        // Pairing might need PIN
                        Console.WriteLine("Pairing failed. PIN/passkey may be required.");
                        var pin = MenuEngine.GetUserInput("Enter PIN/passkey (or press Enter to cancel)");
                        
                        if (!string.IsNullOrEmpty(pin))
                        {
                            var result = await TerminalCommands.RunCommandWithResultAsync(
                                $"echo -e \"pair {device.Address}\n{pin}\n\" | bluetoothctl"
                            );
                            
                            if (result.IsSuccess)
                            {
                                MenuEngine.ShowMessage("Paired", $"Successfully paired with {device.Name}");
                                await changer.ConnectToDeviceAsync(device.Address);
                            }
                            else
                            {
                                MenuEngine.ShowError("Pairing Failed", "Could not pair with the device.");
                            }
                        }
                    }
                }
            }
        }

        public static async Task HandleDeviceOptions(BluetoothInfo device, CheckDependencyCommand checker)
        {
            var changer = new ChangeValueCommand();
            
            var options = new List<string>
            {
                $"Options for {device.Name}",
                "Trust this device",
                "Back"
            };

            var choice = MenuEngine.ShowArrowMenu("Device Options", options);

            if (choice == 1)
            {
                await changer.TrustDeviceAsync(device.Address);
                MenuEngine.ShowMessage("Trusted", $"{device.Name} is now trusted");
            }
        }
    }
}
