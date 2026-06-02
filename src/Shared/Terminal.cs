using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UtilitiesManager
{
    public class CommandResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; } = "";
        public string Error { get; set; } = "";
        
        public bool IsSuccess => ExitCode == 0;
        public string CombinedOutput => Output + (string.IsNullOrEmpty(Error) ? "" : "\n" + Error);
    }

    public static class TerminalCommands
    {
        /// <summary>
        /// Safely escapes an argument for use in a bash command string.
        /// Uses double quotes and escapes internal double quotes.
        /// </summary>
        private static string EscapeBashArgument(string argument)
        {
            return "\"" + argument.Replace("\"", "\\\"") + "\"";
        }

        public static async Task<string> RunCommandAsync(string command, int timeoutMs = Timeout.Infinite)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c {EscapeBashArgument(command)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            return output.Trim();
        }

        public static string RunCommand(string command, int timeoutMs = Timeout.Infinite)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c {EscapeBashArgument(command)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return output.Trim();
        }

        public static async Task<CommandResult> RunCommandWithResultAsync(string command, int timeoutMs = Timeout.Infinite)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c {EscapeBashArgument(command)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            return new CommandResult
            {
                ExitCode = process.ExitCode,
                Output = output.Trim(),
                Error = error.Trim()
            };
        }
    }

    public class ChangeValueCommand
    {
        public async Task SetBrightnessAsync(int percent)
        {
            percent = Math.Clamp(percent, 0, 100);
            await TerminalCommands.RunCommandAsync($"brightnessctl set {percent}%");
        }

        public async Task SetVolumeAsync(int percentage)
        {
            await TerminalCommands.RunCommandAsync(
                $"pactl set-sink-volume @DEFAULT_SINK@ {percentage}%"
            );
        }

        public async Task SetPowerProfileAsync(string profile)
        {
            await TerminalCommands.RunCommandAsync($"powerprofilesctl set {profile}");
        }

        public async Task<bool> ScanBluetoothDevicesAsync()
        {
            try
            {
                var result = await TerminalCommands.RunCommandWithResultAsync("bluetoothctl scan on");
                return result.IsSuccess;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> StopBluetoothScanAsync()
        {
            try
            {
                var result = await TerminalCommands.RunCommandWithResultAsync("bluetoothctl scan off");
                return result.IsSuccess;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ConnectToDeviceAsync(string deviceAddress)
        {
            try
            {
                var result = await TerminalCommands.RunCommandWithResultAsync($"bluetoothctl connect {deviceAddress}");
                return result.IsSuccess;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DisconnectDeviceAsync(string deviceAddress)
        {
            try
            {
                var result = await TerminalCommands.RunCommandWithResultAsync($"bluetoothctl disconnect {deviceAddress}");
                return result.IsSuccess;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PairDeviceAsync(string deviceAddress)
        {
            try
            {
                var result = await TerminalCommands.RunCommandWithResultAsync($"bluetoothctl pair {deviceAddress}");
                return result.IsSuccess;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TrustDeviceAsync(string deviceAddress)
        {
            try
            {
                var result = await TerminalCommands.RunCommandWithResultAsync($"bluetoothctl trust {deviceAddress}");
                return result.IsSuccess;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleBluetoothAsync(bool enable)
        {
            try
            {
                var command = enable ? "bluetoothctl power on" : "bluetoothctl power off";
                var result = await TerminalCommands.RunCommandWithResultAsync(command);
                return result.IsSuccess;
            }
            catch
            {
                return false;
            }
        }
    }

    public class CheckDependencyCommand
    {
        public int OriginalValueLight { get; private set; }
        public int OriginalValueSound { get; private set; }
        public BatteryInfo BatteryStatus { get; private set; } = new BatteryInfo();

        public bool IsBrightnessCtlAvailable => dependencies["brightnessctl"];
        public bool IsPactlAvailable => dependencies["pactl"];
        public bool IsUpowerAvailable => dependencies["upower"];
        public bool IsNmcliAvailable => dependencies["nmcli"];
        public bool IsPowerProfilesCtlAvailable => dependencies["powerprofilesctl"];
        public bool IsBluetoothctlAvailable => dependencies["bluetoothctl"];

        // Terminal/Server monitoring booleans
        public bool IsProcpsAvailable => dependencies["ps"];
        public bool IsLmSensorsAvailable => dependencies["sensors"];
        public bool IsSysstatAvailable => dependencies["iostat"];
        public bool IsIotopAvailable => dependencies["iotop"];
        public bool IsNethogsAvailable => dependencies["nethogs"];
        public bool IsSystemctlAvailable => dependencies["systemctl"];
        public bool IsUseraddAvailable => dependencies["useradd"];
        public bool IsJournalctlAvailable => dependencies["journalctl"];
        public bool IsUfwAvailable => dependencies["ufw"];
        public bool IsIptablesAvailable => dependencies["iptables"];
        public bool IsFail2banAvailable => dependencies["fail2ban-client"];
        public bool IsBleachbitAvailable => dependencies["bleachbit"];
        public bool IsNcduAvailable => dependencies["ncdu"];

        public void LoadOriginalValues()
        {
            CheckDependencies();
            OriginalValueLight = IsBrightnessCtlAvailable ? GetBrightnessPercent() : 50;
            OriginalValueSound = IsPactlAvailable ? GetVolume() : 50;
            BatteryStatus = IsUpowerAvailable ? GetBattery() : new BatteryInfo();
        }

       public static Dictionary<string, bool> dependencies = new Dictionary<string, bool>
            {
                { "brightnessctl", false },
                { "pactl", false },
                { "upower", false },
                { "nmcli", false },
                { "powerprofilesctl", false },
                { "bluetoothctl", false },
                { "free", false }, // for memory info
                { "ps", false },   // for CPU info
                { "sensors", false }, // for temperature info
                { "iostat", false }, // for CPU info (sysstat)
                { "mpstat", false }, // for CPU info (sysstat)
                { "iotop", false },  // for disk I/O monitoring
                { "nethogs", false }, // for network monitoring
                { "systemctl", false }, // for service management
                { "useradd", false }, // for user management
                { "journalctl", false }, // for log viewing
                { "ufw", false }, // for firewall status
                { "iptables", false }, // for firewall status
                { "fail2ban-client", false }, // for security monitoring
                { "bleachbit", false }, // for system cleanup
                { "ncdu", false } // for disk usage analysis
            };

        public void CheckDependencies()
        {
            foreach (var key in dependencies.Keys.ToList())
            {
                dependencies[key] = Check(key) == 0;
            }
        }

        static int Check(string command) 
        { 
            using var process = new Process 
            { 
                StartInfo = new ProcessStartInfo 
                { 
                    FileName = "/bin/bash", 
                    Arguments = $"-c \"which {command} > /dev/null 2>&1\"", 
                    UseShellExecute = false, 
                    RedirectStandardOutput = false, 
                } 
            }; 
            process.Start(); 
            process.WaitForExit(); 
            return process.ExitCode; 
        }

        private bool CheckUpowerBatteryAvailable()
        {
            try
            {
                // Check if there are actual battery devices
                string deviceList = TerminalCommands.RunCommand("upower -e | grep -i battery");
                if (string.IsNullOrWhiteSpace(deviceList))
                {
                    // No battery devices found
                    return false;
                }

                // Additional check: verify at least one battery is present
                string devicePath = deviceList.Split('\n')[0].Trim();
                if (!string.IsNullOrWhiteSpace(devicePath))
                {
                    string batteryInfo = TerminalCommands.RunCommand($"upower -i \"{devicePath}\"");
                    // Check for "present: yes" in the battery section (handles various spacing)
                    var lines = batteryInfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var trimmed = line.Trim();
                        if (trimmed.Contains("present:") && trimmed.Contains("yes"))
                        {
                            return true;
                        }
                    }
                    return false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // BRIGHTNESS 
        public int GetBrightnessPercent()
        {
            string currentStr = TerminalCommands.RunCommand("brightnessctl get");
            string maxStr = TerminalCommands.RunCommand("brightnessctl max");

            if (int.TryParse(currentStr, out int current) &&
                int.TryParse(maxStr, out int max) &&
                max > 0)
            {
                return (int)Math.Round((current / (double)max) * 100);
            }

            return -1;
        }

        // VOLUME
        public int GetVolume()
        {
            string output = TerminalCommands.RunCommand(
                "pactl get-sink-volume @DEFAULT_SINK@"
            );

            var match = Regex.Match(output, @"(\d+)%");
            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return -1;
        }

        // BATTERY
        public BatteryInfo GetBattery()
        {
            var info = new BatteryInfo();

            string deviceList = TerminalCommands.RunCommand("upower -e | grep -i -m 1 battery");
            string devicePath = deviceList.Trim();

            if (string.IsNullOrWhiteSpace(devicePath))
                return info;

            string output = TerminalCommands.RunCommand($"upower -i \"{devicePath}\"");

            if (string.IsNullOrWhiteSpace(output))
                return info;

            var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith("battery present:", StringComparison.OrdinalIgnoreCase))
                {
                    info.IsPresent = trimmed.Contains("yes", StringComparison.OrdinalIgnoreCase);
                }
                else if (trimmed.StartsWith("state:", StringComparison.OrdinalIgnoreCase))
                {
                    info.State = trimmed.Split(':')[1].Trim();
                }
                else if (trimmed.StartsWith("percentage:", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(trimmed, @"(\d+(?:\.\d+)?)%");
                    if (match.Success && double.TryParse(match.Groups[1].Value, out double pct))
                        info.Percentage = (int)Math.Round(pct);
                }
                else if (trimmed.StartsWith("time to empty:", StringComparison.OrdinalIgnoreCase))
                {
                    info.TimeToEmpty = trimmed.Split(':')[1].Trim();
                }
                else if (trimmed.StartsWith("time to full:", StringComparison.OrdinalIgnoreCase))
                {
                    info.TimeToFull = trimmed.Split(':')[1].Trim();
                }
                else if (trimmed.StartsWith("energy-rate:", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(trimmed, @"([-+]?\d+(?:\.\d+)?)");
                    if (match.Success && double.TryParse(match.Groups[1].Value, out double rate))
                        info.EnergyRate = rate;
                }
            }

            return info;
        }

        // WIFI
        public async Task<ObservableCollection<WiFiInfo>> GetWiFiNetworksAsync()
        {
            var networks = new ObservableCollection<WiFiInfo>();
            
            try
            {
                // Use multiline format to avoid line wrapping issues
                string output = await TerminalCommands.RunCommandAsync(
                    "nmcli --mode multiline --get-values IN-USE,SSID,BSSID,MODE,CHAN,RATE,SIGNAL,SECURITY device wifi list"
                );
                
                if (string.IsNullOrWhiteSpace(output))
                    return networks;

                // Parse multiline format where each field is on its own line
                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var currentNetwork = new WiFiInfo();
                var fieldCount = 0;
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine)) continue;
                    
                    // Split on first colon to get field name and value
                    var colonIndex = trimmedLine.IndexOf(':');
                    if (colonIndex <= 0) continue;
                    
                    var fieldName = trimmedLine.Substring(0, colonIndex);
                    var fieldValue = trimmedLine.Substring(colonIndex + 1).Trim();
                    
                    switch (fieldName)
                    {
                        case "IN-USE":
                            currentNetwork.IsActive = fieldValue == "*";
                            fieldCount++;
                            break;
                        case "SSID":
                            currentNetwork.SSID = fieldValue;
                            fieldCount++;
                            break;
                        case "BSSID":
                            currentNetwork.BSSID = fieldValue;
                            fieldCount++;
                            break;
                        case "MODE":
                            currentNetwork.Mode = fieldValue;
                            fieldCount++;
                            break;
                        case "CHAN":
                            currentNetwork.Chan = fieldValue;
                            fieldCount++;
                            break;
                        case "RATE":
                            currentNetwork.Rate = fieldValue;
                            fieldCount++;
                            break;
                        case "SIGNAL":
                            currentNetwork.Signal = fieldValue;
                            fieldCount++;
                            break;
                        case "SECURITY":
                            currentNetwork.Security = string.IsNullOrEmpty(fieldValue) || fieldValue == "--" ? "Open" : fieldValue;
                            fieldCount++;
                            break;
                    }
                    
                    // When we've collected all 8 fields, add the network and reset
                    if (fieldCount >= 8)
                    {
                        if (!string.IsNullOrEmpty(currentNetwork.SSID) && currentNetwork.SSID != "--")
                        {
                            networks.Add(currentNetwork);
                        }
                        currentNetwork = new WiFiInfo();
                        fieldCount = 0;
                    }
                }
            }
            catch
            {
                // Log error if needed, return empty collection
            }

            return networks;
        }

        // BLUETOOTH
        public async Task<ObservableCollection<BluetoothInfo>> GetBluetoothDevicesAsync()
        {
            var devices = new ObservableCollection<BluetoothInfo>();
            
            try
            {
                // Get devices info using bluetoothctl
                string output = await TerminalCommands.RunCommandAsync("bluetoothctl devices Paired");
                
                if (string.IsNullOrWhiteSpace(output))
                {
                    // Try to get all devices if no paired devices
                    output = await TerminalCommands.RunCommandAsync("bluetoothctl devices");
                }
                
                if (string.IsNullOrWhiteSpace(output))
                {
                    // TEST MODE: Add mock devices for testing when no real devices found
                    Console.WriteLine("No real Bluetooth devices found, adding test devices...");
                    
                    devices.Add(new BluetoothInfo
                    {
                        Address = "AA:BB:CC:DD:EE:01",
                        Name = "Test Smartphone",
                        Alias = "Test Smartphone",
                        Available = true,
                        Paired = false,
                        Connected = false,
                        Trusted = false,
                        RSSI = "-65"
                    });
                    
                    devices.Add(new BluetoothInfo
                    {
                        Address = "AA:BB:CC:DD:EE:02", 
                        Name = "Test Wireless Headphones",
                        Alias = "Test Wireless Headphones",
                        Available = true,
                        Paired = true,
                        Connected = false,
                        Trusted = true,
                        RSSI = "-45"
                    });
                    
                    devices.Add(new BluetoothInfo
                    {
                        Address = "AA:BB:CC:DD:EE:03",
                        Name = "Test Bluetooth Speaker",
                        Alias = "Test Bluetooth Speaker", 
                        Available = true,
                        Paired = false,
                        Connected = false,
                        Trusted = false,
                        RSSI = "-78"
                    });
                    
                    devices.Add(new BluetoothInfo
                    {
                        Address = "AA:BB:CC:DD:EE:04",
                        Name = "Test Smartwatch",
                        Alias = "Test Smartwatch",
                        Available = false,
                        Paired = false,
                        Connected = false,
                        Trusted = false,
                        RSSI = "-92"
                    });
                    
                    return devices;
                }

                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine)) continue;
                    
                    // Parse device line format: "Device XX:XX:XX:XX:XX:XX Device Name"
                    if (trimmedLine.StartsWith("Device "))
                    {
                        var parts = trimmedLine.Substring(7).Split(new[] { ' ' }, 2);
                        if (parts.Length >= 2)
                        {
                            var device = new BluetoothInfo
                            {
                                Address = parts[0].Trim(),
                                Name = parts[1].Trim(),
                                Alias = parts[1].Trim(),
                                Available = true
                            };
                            
                            // Get detailed device info
                            await GetDeviceDetailsAsync(device);
                            
                            if (!string.IsNullOrEmpty(device.Address))
                            {
                                devices.Add(device);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Log error if needed, return empty collection
            }

            return devices;
        }

        private async Task GetDeviceDetailsAsync(BluetoothInfo device)
        {
            try
            {
                string infoOutput = await TerminalCommands.RunCommandAsync($"bluetoothctl info {device.Address}");
                
                if (string.IsNullOrWhiteSpace(infoOutput))
                    return;

                var lines = infoOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    if (trimmedLine.StartsWith("Alias: "))
                    {
                        device.Alias = trimmedLine.Substring(7).Trim();
                    }
                    else if (trimmedLine.StartsWith("Type: "))
                    {
                        device.Type = trimmedLine.Substring(6).Trim();
                    }
                    else if (trimmedLine.StartsWith("RSSI: "))
                    {
                        device.RSSI = trimmedLine.Substring(6).Trim();
                    }
                    else if (trimmedLine.StartsWith("Paired: "))
                    {
                        device.Paired = trimmedLine.Substring(8).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);
                    }
                    else if (trimmedLine.StartsWith("Connected: "))
                    {
                        device.Connected = trimmedLine.Substring(11).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);
                    }
                    else if (trimmedLine.StartsWith("Trusted: "))
                    {
                        device.Trusted = trimmedLine.Substring(9).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch
            {
                // Ignore errors in getting device details
            }
        }

        // POWER PROFILE
        public async Task<string> GetCurrentPowerProfileAsync()
        {
            try
            {
                string output = await TerminalCommands.RunCommandAsync("powerprofilesctl get");
                return output.Trim();
            }
            catch
            {
                return "Unknown";
            }
        }

        // SYSTEM MONITORING METHODS
        public async Task<SystemInfo> GetSystemInfoAsync()
        {
        var info = new SystemInfo();
        
        // CPU Information
        if (IsProcpsAvailable)
        {
            try
            {
                var uptimeResult = await TerminalCommands.RunCommandWithResultAsync("uptime");
                info.Uptime = uptimeResult.Output.Trim();
                
                var loadAvgResult = await TerminalCommands.RunCommandWithResultAsync("cat /proc/loadavg");
                var loadAvgParts = loadAvgResult.Output.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (loadAvgParts.Length >= 3)
                {
                    info.LoadAverage = new string[] { loadAvgParts[0], loadAvgParts[1], loadAvgParts[2] };
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"CPU info error: {ex.Message}");
            }
        }
        else
        {
            // Debug: Show that procps is not available
            System.Diagnostics.Debug.WriteLine("procps not available for CPU info");
        }

        // Memory Information
        if (IsProcpsAvailable)
        {
            try
            {
                var memResult = await TerminalCommands.RunCommandWithResultAsync("free -h");
                info.MemoryInfo = ParseMemoryInfo(memResult.Output);
                
                // Debug: log memory parsing result
                System.Diagnostics.Debug.WriteLine($"Memory info parsed: {info.MemoryInfo.Count} entries");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Memory parsing error: {ex.Message}");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("procps not available for memory info");
        }

        // CPU Temperature
        if (IsLmSensorsAvailable)
        {
            try
            {
                var tempResult = await TerminalCommands.RunCommandWithResultAsync("sensors");
                info.Temperatures = ParseTemperatureInfo(tempResult.Output);
            }
            catch { }
        }

        // Disk Usage
        try
        {
            var diskResult = await TerminalCommands.RunCommandWithResultAsync("df -h");
            info.DiskUsage = ParseDiskUsage(diskResult.Output);
            
            // Debug: log disk parsing result
            System.Diagnostics.Debug.WriteLine($"Disk usage parsed: {info.DiskUsage.Count} entries");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Disk parsing error: {ex.Message}");
        }

        // Network Interfaces
        try
        {
            var netResult = await TerminalCommands.RunCommandWithResultAsync("ip addr show");
            info.NetworkInterfaces = ParseNetworkInterfaces(netResult.Output);
            
            // Debug: log network parsing result
            System.Diagnostics.Debug.WriteLine($"Network interfaces parsed: {info.NetworkInterfaces.Count} entries");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Network parsing error: {ex.Message}");
        }

        return info;
    }

        public async Task<List<ServiceInfo>> GetServicesAsync()
        {
            var services = new List<ServiceInfo>();
            
            if (!IsSystemctlAvailable) return services;

            try
            {
                var result = await TerminalCommands.RunCommandWithResultAsync("systemctl list-units --type=service --state=running,stopped,failed --no-pager --no-legend");
                var lines = result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines.Take(20)) // Limit to first 20 services
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        services.Add(new ServiceInfo
                        {
                            Name = parts[0],
                            Load = parts[1],
                            Active = parts[2],
                            Sub = parts[3],
                            Description = parts.Length > 4 ? string.Join(" ", parts.Skip(4)) : ""
                        });
                    }
                }
            }
            catch { }

            return services;
        }

        public async Task<List<UserInfo>> GetUsersAsync()
        {
            var users = new List<UserInfo>();
            
            try
            {
                // Get user list from /etc/passwd
                var passwdOutput = await TerminalCommands.RunCommandAsync("cat /etc/passwd");
                var lines = passwdOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines.Take(10)) // Limit to first 10 users
                {
                    var parts = line.Split(':');
                    if (parts.Length >= 7)
                    {
                        users.Add(new UserInfo
                        {
                            Username = parts[0],
                            UID = parts[2],
                            GID = parts[3],
                            Home = parts[5],
                            Shell = parts[6]
                        });
                    }
                }

                // Get currently logged in users
                if (IsProcpsAvailable)
                {
                    try
                    {
                        var whoOutput = await TerminalCommands.RunCommandAsync("who");
                        var loggedInUsers = whoOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                        
                        foreach (var loggedIn in loggedInUsers)
                        {
                            var parts = loggedIn.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 0)
                            {
                                var user = users.FirstOrDefault(u => u.Username == parts[0]);
                                if (user != null)
                                {
                                    user.IsLoggedIn = true;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }

            return users;
        }

        public async Task<List<LogEntry>> GetRecentLogsAsync(string logType = "system")
        {
            var logs = new List<LogEntry>();
            
            if (!IsJournalctlAvailable) return logs;

            try
            {
                var command = logType.ToLower() switch
                {
                    "kernel" => "journalctl -k --no-pager -n 20",
                    "boot" => "journalctl -b --no-pager -n 20",
                    _ => "journalctl --no-pager -n 20"
                };

                var output = await TerminalCommands.RunCommandAsync(command);
                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines.Take(20))
                {
                    logs.Add(new LogEntry
                    {
                        Timestamp = ExtractTimestamp(line),
                        Message = line
                    });
                }
            }
            catch { }

            return logs;
        }

        public async Task<FirewallStatus> GetFirewallStatusAsync()
        {
            var status = new FirewallStatus();
            
            if (IsUfwAvailable)
            {
                try
                {
                    var output = await TerminalCommands.RunCommandAsync("ufw status");
                    status.UfwStatus = ParseUfwStatus(output);
                }
                catch { }
            }

            if (IsIptablesAvailable)
            {
                try
                {
                    var output = await TerminalCommands.RunCommandAsync("iptables -L --line-numbers");
                    status.IptablesRules = ParseIptablesRules(output);
                }
                catch { }
            }

            if (IsFail2banAvailable)
            {
                try
                {
                    var output = await TerminalCommands.RunCommandAsync("fail2ban-client status");
                    status.Fail2banStatus = ParseFail2banStatus(output);
                }
                catch { }
            }

            return status;
        }

        // Helper methods for parsing system information
        private Dictionary<string, string> ParseMemoryInfo(string output)
        {
        var info = new Dictionary<string, string>();
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            if (line.StartsWith("Mem:"))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4)
                {
                    info["Total"] = parts[1];
                    info["Used"] = parts[2];
                    info["Free"] = parts[3];
                }
            }
            else if (line.StartsWith("Swap:"))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4)
                {
                    info["SwapTotal"] = parts[1];
                    info["SwapUsed"] = parts[2];
                    info["SwapFree"] = parts[3];
                }
            }
        }
        
        return info;
    }

        private Dictionary<string, string> ParseTemperatureInfo(string output)
        {
            var temps = new Dictionary<string, string>();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                if (line.Contains("°C"))
                {
                    var parts = line.Split(':');
                    if (parts.Length >= 2)
                    {
                        var sensor = parts[0].Trim();
                        var temp = parts[1].Trim();
                        temps[sensor] = temp;
                    }
                }
            }
            
            return temps;
        }

        private List<DiskInfo> ParseDiskUsage(string output)
        {
            var disks = new List<DiskInfo>();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            // Skip header line and parse each disk entry
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 6)
                {
                    disks.Add(new DiskInfo
                    {
                        Filesystem = parts[0],
                        Size = parts[1],
                        Used = parts[2],
                        Available = parts[3],
                        UsePercent = parts[4],
                        MountPoint = parts[5]
                    });
                }
            }
            
            return disks;
        }

        private List<NetworkInterface> ParseNetworkInterfaces(string output)
        {
            var interfaces = new List<NetworkInterface>();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string currentInterface = "";
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                // Check for interface definition (starts with a number and has colon)
                if (Regex.IsMatch(trimmed, @"^\d+:"))
                {
                    var parts = trimmed.Split(':');
                    if (parts.Length > 1)
                    {
                        currentInterface = parts[1].Trim();
                    }
                }
                // Check for IP address
                else if (trimmed.StartsWith("inet "))
                {
                    var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && !string.IsNullOrEmpty(currentInterface))
                    {
                        interfaces.Add(new NetworkInterface
                        {
                            IPAddress = parts[1],
                            Interface = currentInterface
                        });
                    }
                }
            }
            
            return interfaces;
        }

        private string ParseUfwStatus(string output)
        {
            if (output.Contains("Status: active"))
                return "Active";
            else if (output.Contains("Status: inactive"))
                return "Inactive";
            else
                return "Unknown";
        }

        private List<string> ParseIptablesRules(string output)
        {
            return output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                         .Where(line => !line.StartsWith("Chain") && !line.StartsWith("target"))
                         .Take(10)
                         .ToList();
        }

        private string ParseFail2banStatus(string output)
        {
            if (output.Contains("Status"))
                return output.Split('\n').FirstOrDefault(line => line.Contains("Status")) ?? "Unknown";
            return "Unknown";
        }

        private string ExtractTimestamp(string logLine)
        {
            var parts = logLine.Split(' ');
            if (parts.Length >= 3)
            {
                return $"{parts[0]} {parts[1]} {parts[2]}";
            }
            return "";
        }

        private string ExtractInterfaceName(string fullOutput, string currentLine)
        {
            var lines = fullOutput.Split('\n');
            var currentIndex = Array.IndexOf(lines, currentLine);
            
            for (int i = currentIndex - 1; i >= 0; i--)
            {
                var line = lines[i].Trim();
                if (line.Length > 0 && !line.StartsWith(" ") && !line.StartsWith("\t"))
                {
                    var parts = line.Split(':');
                    if (parts.Length > 0)
                    {
                        return parts[0].Trim();
                    }
                }
            }
            
            return "Unknown";
        }
    }
}
