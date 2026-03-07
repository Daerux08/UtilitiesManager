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

        public async Task LoadOriginalValuesAsync()
        {
            await CheckDependenciesAsync();
            OriginalValueLight = IsBrightnessCtlAvailable ? await GetBrightnessPercentAsync() : 50;
            OriginalValueSound = IsPactlAvailable ? await GetVolumeAsync() : 50;
            BatteryStatus = IsUpowerAvailable ? await GetBatteryAsync() : new BatteryInfo();
        }

        public async Task CheckDependenciesAsync()
        {
            IsBrightnessCtlAvailable = await CheckCommandAvailable("brightnessctl");
            IsPactlAvailable = await CheckCommandAvailable("pactl");
            IsUpowerAvailable = await CheckCommandAvailable("upower");
            IsNmcliAvailable = await CheckCommandAvailable("nmcli");
            IsPowerProfilesCtlAvailable = await CheckCommandAvailable("powerprofilesctl");
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
                    var parsedNetworks = TryParseWithColumnStrategy(lines) ?? 
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
    }}

