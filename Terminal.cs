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
        public static async Task<string> RunCommandAsync(string command, int timeoutMs = Timeout.Infinite)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"",
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
                Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"",
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
            // Original dependencies
            IsBrightnessCtlAvailable = await CheckCommandAvailable("brightnessctl");
            IsPactlAvailable = await CheckCommandAvailable("pactl");
            IsUpowerAvailable = await CheckCommandAvailable("upower");
            IsNmcliAvailable = await CheckCommandAvailable("nmcli");
            IsPowerProfilesCtlAvailable = await CheckCommandAvailable("powerprofilesctl");

            // Terminal/Server monitoring dependencies
            IsProcpsAvailable = await CheckCommandAvailable("ps") && await CheckCommandAvailable("free");
            IsLmSensorsAvailable = await CheckCommandAvailable("sensors");
            IsSysstatAvailable = await CheckCommandAvailable("iostat") && await CheckCommandAvailable("mpstat");
            IsIotopAvailable = await CheckCommandAvailable("iotop");
            IsNethogsAvailable = await CheckCommandAvailable("nethogs");
            IsSystemctlAvailable = await CheckCommandAvailable("systemctl");
            IsUseraddAvailable = await CheckCommandAvailable("useradd");
            IsJournalctlAvailable = await CheckCommandAvailable("journalctl");
            IsUfwAvailable = await CheckCommandAvailable("ufw");
            IsIptablesAvailable = await CheckCommandAvailable("iptables");
            IsFail2banAvailable = await CheckCommandAvailable("fail2ban-client");
            IsBleachbitAvailable = await CheckCommandAvailable("bleachbit");
            IsNcduAvailable = await CheckCommandAvailable("ncdu");
        }

        private async Task<bool> CheckCommandAvailable(string command)
        {
            try
            {
                string output = await TerminalCommands.RunCommandAsync($"which {command}");
                return !string.IsNullOrWhiteSpace(output);
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
            int maxRetries = 3;
            int retryDelayMs = 1000;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    string output = await TerminalCommands.RunCommandAsync("nmcli device wifi list");
                    if (string.IsNullOrWhiteSpace(output))
                    {
                        if (attempt < maxRetries)
                        {
                            await Task.Delay(retryDelayMs);
                            continue;
                        }
                        return networks;
                    }

                    var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    // Try multiple parsing strategies
                    var parsedNetworks = TryParseWithMultiLineStrategy(lines) ??
                                       TryParseWithColumnStrategy(lines) ?? 
                                       TryParseWithRegexStrategy(lines) ??
                                       TryParseWithFlexibleStrategy(lines);

                    if (parsedNetworks != null)
                    {
                        foreach (var network in parsedNetworks)
                        {
                            if (ValidateWiFiInfo(network))
                            {
                                networks.Add(network);
                            }
                        }
                        
                        return networks;
                    }
                    else
                    {
                        // All parsing strategies failed
                    }
                }
                catch
                {
                    // Parsing failed, will retry
                }

                if (attempt < maxRetries)
                {
                    await Task.Delay(retryDelayMs);
                }
            }

            return networks;
        }

        private List<WiFiInfo>? TryParseWithMultiLineStrategy(string[] lines)
        {
            try
            {
                var networks = new List<WiFiInfo>();
                
                // Skip header line
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Check if this line contains a BSSID (MAC address format) anywhere in the line
                    if (Regex.IsMatch(line, @"([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}"))
                    {
                        // Use a more robust parsing approach that handles SSIDs with spaces
                        var network = ParseNetworkLine(line);
                        if (network != null && !string.IsNullOrEmpty(network.SSID) && network.SSID != "--")
                        {
                            networks.Add(network);
                        }
                    }
                }

                return networks.Count > 0 ? networks : null;
            }
            catch
            {
                return null;
            }
        }

        private WiFiInfo? ParseNetworkLine(string line)
        {
            try
            {
                var network = new WiFiInfo();
                
                // Find the IN-USE field (starts with * or is empty)
                var inUseMatch = Regex.Match(line, @"^(\*|\s+)\s+");
                if (inUseMatch.Success)
                {
                    network.IsActive = inUseMatch.Groups[1].Value.Contains("*");
                }
                
                // Find BSSID (MAC address)
                var bssidMatch = Regex.Match(line, @"([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}");
                if (!bssidMatch.Success) return null;
                
                // Find the parts after BSSID
                var afterBssid = line.Substring(bssidMatch.Index + bssidMatch.Length).Trim();
                
                // Use regex to capture the remaining fields
                // SSID, MODE, CHAN, RATE, SIGNAL, BARS, SECURITY
                var pattern = @"^(.+?)\s+(\w+)\s+(\d+)\s+([\d.]+\s*[MG]?)\s+(\d+)\s+([▂▄▆█_]+)\s+(.+)$";
                var match = Regex.Match(afterBssid, pattern);
                
                if (match.Success)
                {
                    network.SSID = match.Groups[1].Value.Trim();
                    network.Mode = match.Groups[2].Value.Trim();
                    network.Chan = match.Groups[3].Value.Trim();
                    network.Rate = match.Groups[4].Value.Trim();
                    network.Signal = match.Groups[5].Value.Trim();
                    // Skip BARS (group 6)
                    network.Security = match.Groups[7].Value.Trim();
                    
                    if (network.Security == "--" || string.IsNullOrEmpty(network.Security))
                        network.Security = "Open";
                    
                    return network;
                }
                
                // Fallback: try to parse manually
                var parts = afterBssid.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 6)
                {
                    // Try to identify the pattern by looking for known field values
                    // Look for mode (Infra, Ad-Hoc)
                    int modeIndex = -1;
                    for (int j = 0; j < parts.Length - 1; j++)
                    {
                        if (parts[j] == "Infra" || parts[j] == "Ad-Hoc")
                        {
                            modeIndex = j;
                            break;
                        }
                    }
                    
                    if (modeIndex > 0)
                    {
                        // SSID is everything before the mode
                        network.SSID = string.Join(" ", parts.Take(modeIndex)).Trim();
                        
                        // Mode
                        network.Mode = parts[modeIndex];
                        
                        // Channel (should be a number)
                        if (modeIndex + 1 < parts.Length && int.TryParse(parts[modeIndex + 1], out _))
                        {
                            network.Chan = parts[modeIndex + 1];
                            modeIndex++;
                        }
                        
                        // Rate (should contain Mbit/s or Gbit/s)
                        if (modeIndex + 1 < parts.Length && parts[modeIndex + 1].Contains("Mbit/s"))
                        {
                            network.Rate = parts[modeIndex + 1];
                            modeIndex++;
                        }
                        else if (modeIndex + 2 < parts.Length && parts[modeIndex + 2].Contains("Mbit/s"))
                        {
                            // Rate might be two parts (e.g., "270 Mbit/s")
                            network.Rate = parts[modeIndex + 1] + " " + parts[modeIndex + 2];
                            modeIndex += 2;
                        }
                        
                        // Signal (should be a number)
                        if (modeIndex + 1 < parts.Length && int.TryParse(parts[modeIndex + 1], out _))
                        {
                            network.Signal = parts[modeIndex + 1];
                            modeIndex++;
                        }
                        
                        // Skip BARS
                        
                        // Security is everything else
                        if (modeIndex + 2 < parts.Length)
                        {
                            network.Security = string.Join(" ", parts.Skip(modeIndex + 2)).Trim();
                            if (network.Security == "--" || string.IsNullOrEmpty(network.Security))
                                network.Security = "Open";
                        }
                        
                        return network;
                    }
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        private List<WiFiInfo>? TryParseWithColumnStrategy(string[] lines)
        {
            try
            {
                var networks = new List<WiFiInfo>();
                
                // Skip header line
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].TrimStart();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = Regex.Split(line, @"\s{2,}");
                    if (parts.Length < 8)
                    {
                        continue;
                    }

                    var network = ParseWiFiFromParts(parts);
                    if (network != null)
                        networks.Add(network);
                }

                return networks.Count > 0 ? networks : null;
            }
            catch
            {
                return null;
            }
        }

        private List<WiFiInfo>? TryParseWithRegexStrategy(string[] lines)
        {
            try
            {
                var networks = new List<WiFiInfo>();
                
                // Regex pattern to match nmcli output format
                var pattern = @"^(\*?\s*)([0-9a-fA-F:]+|\s+)\s+([^:]+?)\s+(\w+)\s+(\d+)\s+([\d.]+[MG]?)\s+(\d+)\s+(\W+)\s*(.*)$";
                
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var match = Regex.Match(line, pattern);
                    if (!match.Success)
                    {
                        continue;
                    }

                    var network = new WiFiInfo
                    {
                        IsActive = match.Groups[1].Value.Contains("*"),
                        SSID = match.Groups[3].Value.Trim(),
                        Mode = match.Groups[4].Value.Trim(),
                        Chan = match.Groups[5].Value.Trim(),
                        Rate = match.Groups[6].Value.Trim(),
                        Signal = match.Groups[7].Value.Trim(),
                        Security = string.IsNullOrEmpty(match.Groups[9].Value.Trim()) ? "Open" : match.Groups[9].Value.Trim()
                    };

                    if (network.Security == "--")
                        network.Security = "Open";

                    networks.Add(network);
                }

                return networks.Count > 0 ? networks : null;
            }
            catch
            {
                return null;
            }
        }

        private List<WiFiInfo>? TryParseWithFlexibleStrategy(string[] lines)
        {
            try
            {
                var networks = new List<WiFiInfo>();
                
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Split on multiple spaces but be more flexible about column count
                    var parts = Regex.Split(line, @"\s{2,}").Select(p => p.Trim()).ToArray();
                    
                    if (parts.Length < 3) // Minimum: SSID, Mode, something else
                    {
                        continue;
                    }

                    var network = new WiFiInfo();
                    
                    // Try to identify columns heuristically
                    int activeIdx = 0;
                    network.IsActive = parts[activeIdx].Contains("*");
                    
                    // Find SSID - usually the first non-empty, non-star field after active indicator
                    int ssidIdx = -1;
                    for (int j = activeIdx + 1; j < parts.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(parts[j]) && !parts[j].Contains(":"))
                        {
                            ssidIdx = j;
                            break;
                        }
                    }
                    
                    if (ssidIdx == -1) continue;
                    
                    network.SSID = parts[ssidIdx];
                    
                    // Assign remaining fields based on position and content
                    if (ssidIdx + 1 < parts.Length) network.Mode = parts[ssidIdx + 1];
                    if (ssidIdx + 2 < parts.Length) network.Chan = parts[ssidIdx + 2];
                    if (ssidIdx + 3 < parts.Length) network.Rate = parts[ssidIdx + 3];
                    if (ssidIdx + 4 < parts.Length) network.Signal = parts[ssidIdx + 4];
                    
                    // Security is usually the last field
                    var securityParts = parts.Skip(ssidIdx + 5).Where(p => !string.IsNullOrEmpty(p));
                    network.Security = securityParts.Any() ? string.Join(" ", securityParts) : "Open";
                    if (network.Security == "--") network.Security = "Open";

                    networks.Add(network);
                }

                return networks.Count > 0 ? networks : null;
            }
            catch
            {
                return null;
            }
        }

        private WiFiInfo? ParseWiFiFromParts(string[] parts)
        {
            try
            {
                var network = new WiFiInfo();
                int idx = 0;

                // IN-USE
                network.IsActive = parts[idx++].Contains("*");

                // BSSID (skip)
                idx++;

                // SSID
                network.SSID = parts[idx++].Trim();

                // MODE
                network.Mode = parts[idx++].Trim();

                // CHAN
                network.Chan = parts[idx++].Trim();

                // RATE
                network.Rate = parts[idx++].Trim();

                // SIGNAL
                network.Signal = parts[idx++].Trim();

                // BARS (skip)
                idx++;

                // SECURITY (remaining parts)
                if (idx < parts.Length)
                {
                    network.Security = string.Join(" ", parts.Skip(idx)).Trim();
                    if (network.Security == "--" || string.IsNullOrEmpty(network.Security))
                        network.Security = "Open";
                }
                else
                {
                    network.Security = "Open";
                }

                return network;
            }
            catch
            {
                return null;
            }
        }

        private bool ValidateWiFiInfo(WiFiInfo network)
        {
            // SSID validation
            if (string.IsNullOrWhiteSpace(network.SSID))
            {
                return false;
            }

            // Skip if SSID is just a mode name (parsing artifact)
            if (network.SSID == "Infra" || network.SSID == "Adhoc" || network.SSID == "AP")
            {
                return false;
            }

            // Signal strength validation - handle both numeric and bar representations
            if (!string.IsNullOrEmpty(network.Signal))
            {
                // Check for bar representation (like '▂___')
                if (network.Signal.Contains("▂") || network.Signal.Contains("▄") || 
                    network.Signal.Contains("█") || network.Signal.Contains("_"))
                {
                    // Bar representation is valid
                    return true;
                }
                
                // Check for numeric representation
                if (int.TryParse(network.Signal, out int signal))
                {
                    if (signal < 0 || signal > 100)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            // Channel validation
            if (!string.IsNullOrEmpty(network.Chan))
            {
                if (int.TryParse(network.Chan, out int channel) && (channel < 1 || channel > 200))
                {
                    return false;
                }
            }

            return true;
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
                var uptimeOutput = await TerminalCommands.RunCommandAsync("uptime");
                info.Uptime = uptimeOutput.Trim();
                
                var loadAvgOutput = await TerminalCommands.RunCommandAsync("cat /proc/loadavg");
                info.LoadAverage = loadAvgOutput.Split(' ')[0..2];
            }
            catch { }
        }

        // Memory Information
        if (IsProcpsAvailable)
        {
            try
            {
                var memOutput = await TerminalCommands.RunCommandAsync("free -h");
                info.MemoryInfo = ParseMemoryInfo(memOutput);
            }
            catch { }
        }

        // CPU Temperature
        if (IsLmSensorsAvailable)
        {
            try
            {
                var tempOutput = await TerminalCommands.RunCommandAsync("sensors");
                info.Temperatures = ParseTemperatureInfo(tempOutput);
            }
            catch { }
        }

        // Disk Usage
        try
        {
            var diskOutput = await TerminalCommands.RunCommandAsync("df -h");
            info.DiskUsage = ParseDiskUsage(diskOutput);
        }
        catch { }

        // Network Interfaces
        try
        {
            var netOutput = await TerminalCommands.RunCommandAsync("ip addr show");
            info.NetworkInterfaces = ParseNetworkInterfaces(netOutput);
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
            var output = await TerminalCommands.RunCommandAsync("systemctl list-units --type=service --state=running,stopped,failed --no-pager --no-legend");
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
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
