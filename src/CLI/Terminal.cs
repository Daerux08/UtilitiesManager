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
    public class BatteryInfo
    {
        public string State { get; set; } = "Unknown";
        public int Percentage { get; set; } = -1;
        public string TimeToEmpty { get; set; } = "N/A";
        public string TimeToFull { get; set; } = "N/A";
        public double EnergyRate { get; set; } = -1;
        public bool IsPresent { get; set; } = false;
    }

    public class WiFiInfo
    {
        public string SSID { get; set; } = "";
        public string BSSID { get; set; } = "";
        public string Mode { get; set; } = "";
        public string Chan { get; set; } = "";
        public string Rate { get; set; } = "";
        public string Signal { get; set; } = "";
        public string Security { get; set; } = "";
        public bool IsActive { get; set; } = false;
    }

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
        /// Wraps the string in single quotes and handles internal single quotes.
        /// </summary>
        private static string EscapeBashArgument(string argument)
        {
            return "'" + argument.Replace("'", "'\\''") + "'";
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
    }

    public class CheckDependencyCommand
    {
        public int OriginalValueLight { get; private set; }
        public int OriginalValueSound { get; private set; }
        public BatteryInfo BatteryStatus { get; private set; } = new BatteryInfo();

        public bool IsBrightnessCtlAvailable { get; private set; }
        public bool IsPactlAvailable { get; private set; }
        public bool IsUpowerAvailable { get; private set; }
        public bool IsNmcliAvailable { get; private set; }
        public bool IsPowerProfilesCtlAvailable { get; private set; }

        // Terminal/Server monitoring booleans
        public bool IsProcpsAvailable { get; private set; }
        public bool IsLmSensorsAvailable { get; private set; }
        public bool IsSysstatAvailable { get; private set; }
        public bool IsIotopAvailable { get; private set; }
        public bool IsNethogsAvailable { get; private set; }
        public bool IsSystemctlAvailable { get; private set; }
        public bool IsUseraddAvailable { get; private set; }
        public bool IsJournalctlAvailable { get; private set; }
        public bool IsUfwAvailable { get; private set; }
        public bool IsIptablesAvailable { get; private set; }
        public bool IsFail2banAvailable { get; private set; }
        public bool IsBleachbitAvailable { get; private set; }
        public bool IsNcduAvailable { get; private set; }

        public async Task LoadOriginalValuesAsync()
        {
            await CheckDependenciesAsync();
            OriginalValueLight = IsBrightnessCtlAvailable ? await GetBrightnessPercentAsync() : 50;
            OriginalValueSound = IsPactlAvailable ? await GetVolumeAsync() : 50;
            BatteryStatus = IsUpowerAvailable ? await GetBatteryAsync() : new BatteryInfo();
        }

        public async Task CheckDependenciesAsync()
        {
            string[] deps = { "upower", "pactl", "brightnessctl", "nmcli", "powerprofilesctl", "ps", "free", "sensors", "iostat", "mpstat", "iotop", "nethogs", "systemctl", "useradd", "journalctl", "ufw", "iptables", "fail2ban-client", "bleachbit", "ncdu" }; 

            foreach (string dep in deps) 
            { 
                int exitCode = await CheckAsync(dep); 
                Console.WriteLine($"{dep}: {(exitCode == 0 ? "1" : "0")}"); 
                
                // Set the corresponding boolean properties
                switch (dep)
                {
                    case "brightnessctl":
                        IsBrightnessCtlAvailable = exitCode == 0;
                        break;
                    case "pactl":
                        IsPactlAvailable = exitCode == 0;
                        break;
                    case "upower":
                        IsUpowerAvailable = exitCode == 0 && await CheckUpowerBatteryAvailable();
                        break;
                    case "nmcli":
                        IsNmcliAvailable = exitCode == 0;
                        break;
                    case "powerprofilesctl":
                        IsPowerProfilesCtlAvailable = exitCode == 0;
                        break;
                    case "ps":
                        // Will be combined with "free" for IsProcpsAvailable
                        break;
                    case "free":
                        // Set IsProcpsAvailable if both ps and free are available
                        int psExitCode = await CheckAsync("ps");
                        IsProcpsAvailable = exitCode == 0 && psExitCode == 0;
                        break;
                    case "sensors":
                        IsLmSensorsAvailable = exitCode == 0;
                        break;
                    case "iostat":
                        // Will be combined with "mpstat" for IsSysstatAvailable
                        break;
                    case "mpstat":
                        // Set IsSysstatAvailable if both iostat and mpstat are available
                        int iostatExitCode = await CheckAsync("iostat");
                        IsSysstatAvailable = exitCode == 0 && iostatExitCode == 0;
                        break;
                    case "iotop":
                        IsIotopAvailable = exitCode == 0;
                        break;
                    case "nethogs":
                        IsNethogsAvailable = exitCode == 0;
                        break;
                    case "systemctl":
                        IsSystemctlAvailable = exitCode == 0;
                        break;
                    case "useradd":
                        IsUseraddAvailable = exitCode == 0;
                        break;
                    case "journalctl":
                        IsJournalctlAvailable = exitCode == 0;
                        break;
                    case "ufw":
                        IsUfwAvailable = exitCode == 0;
                        break;
                    case "iptables":
                        IsIptablesAvailable = exitCode == 0;
                        break;
                    case "fail2ban-client":
                        IsFail2banAvailable = exitCode == 0;
                        break;
                    case "bleachbit":
                        IsBleachbitAvailable = exitCode == 0;
                        break;
                    case "ncdu":
                        IsNcduAvailable = exitCode == 0;
                        break;
                }
            }
        }

        static async Task<int> CheckAsync(string command) 
        { 
            var process = new Process 
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
            await process.WaitForExitAsync(); 
            return process.ExitCode; 
        }

        private async Task<bool> CheckUpowerBatteryAvailable()
        {
            try
            {
                // Check if there are actual battery devices
                string deviceList = await TerminalCommands.RunCommandAsync("upower -e | grep -i battery");
                if (string.IsNullOrWhiteSpace(deviceList))
                {
                    // No battery devices found
                    return false;
                }

                // Additional check: verify at least one battery is present
                string devicePath = deviceList.Split('\n')[0].Trim();
                if (!string.IsNullOrWhiteSpace(devicePath))
                {
                    string batteryInfo = await TerminalCommands.RunCommandAsync($"upower -i \"{devicePath}\"");
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
        public async Task<int> GetBrightnessPercentAsync()
        {
            string currentStr = await TerminalCommands.RunCommandAsync("brightnessctl get");
            string maxStr = await TerminalCommands.RunCommandAsync("brightnessctl max");

            if (int.TryParse(currentStr, out int current) &&
                int.TryParse(maxStr, out int max) &&
                max > 0)
            {
                return (int)Math.Round((current / (double)max) * 100);
            }

            return -1;
        }

        // VOLUME
        public async Task<int> GetVolumeAsync()
        {
            string output = await TerminalCommands.RunCommandAsync(
                "pactl get-sink-volume @DEFAULT_SINK@"
            );

            var match = Regex.Match(output, @"(\d+)%");
            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return -1;
        }

        // BATTERY
        public async Task<BatteryInfo> GetBatteryAsync()
        {
            var info = new BatteryInfo();

            string deviceList = await TerminalCommands.RunCommandAsync("upower -e | grep -i -m 1 battery");
            string devicePath = deviceList.Trim();

            if (string.IsNullOrWhiteSpace(devicePath))
                return info;

            string output = await TerminalCommands.RunCommandAsync($"upower -i \"{devicePath}\"");

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
                    var fieldValue = trimmedLine.Substring(colonIndex + 1);
                    
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
                var loadAvgParts = loadAvgResult.Output.Split(' ');
                if (loadAvgParts.Length >= 3)
                {
                    info.LoadAverage = loadAvgParts[0..3];
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
            }
            catch { }
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
        }
        catch { }

        // Network Interfaces
        try
        {
            var netResult = await TerminalCommands.RunCommandWithResultAsync("ip addr show");
            info.NetworkInterfaces = ParseNetworkInterfaces(netResult.Output);
        }
        catch { }

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
                if (parts.Length >= 3)
                {
                    info["Total"] = parts[1];
                    info["Used"] = parts[2];
                    info["Free"] = parts[3];
                }
            }
            else if (line.StartsWith("Swap:"))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
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
        
        foreach (var line in lines.Skip(1)) // Skip header
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
        
        foreach (var line in lines)
        {
            if (line.TrimStart().StartsWith("inet "))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    interfaces.Add(new NetworkInterface
                    {
                        IPAddress = parts[1],
                        Interface = ExtractInterfaceName(output, line)
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

// Additional data classes for system monitoring
public class SystemInfo
{
    public string Uptime { get; set; } = "";
    public string[] LoadAverage { get; set; } = new string[0];
    public Dictionary<string, string> MemoryInfo { get; set; } = new();
    public Dictionary<string, string> Temperatures { get; set; } = new();
    public List<DiskInfo> DiskUsage { get; set; } = new();
    public List<NetworkInterface> NetworkInterfaces { get; set; } = new();
}

public class ServiceInfo
{
    public string Name { get; set; } = "";
    public string Load { get; set; } = "";
    public string Active { get; set; } = "";
    public string Sub { get; set; } = "";
    public string Description { get; set; } = "";
}

public class UserInfo
{
    public string Username { get; set; } = "";
    public string UID { get; set; } = "";
    public string GID { get; set; } = "";
    public string Home { get; set; } = "";
    public string Shell { get; set; } = "";
    public bool IsLoggedIn { get; set; } = false;
}

public class LogEntry
{
    public string Timestamp { get; set; } = "";
    public string Message { get; set; } = "";
}

public class FirewallStatus
{
    public string UfwStatus { get; set; } = "";
    public List<string> IptablesRules { get; set; } = new();
    public string Fail2banStatus { get; set; } = "";
}

public class DiskInfo
{
    public string Filesystem { get; set; } = "";
    public string Size { get; set; } = "";
    public string Used { get; set; } = "";
    public string Available { get; set; } = "";
    public string UsePercent { get; set; } = "";
    public string MountPoint { get; set; } = "";
}

public class NetworkInterface
{
    public string Interface { get; set; } = "";
    public string IPAddress { get; set; } = "";
}
}
