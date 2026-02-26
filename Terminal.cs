using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

            try
            {
                string output = await TerminalCommands.RunCommandAsync("nmcli device wifi list");

                if (string.IsNullOrWhiteSpace(output))
                    return networks;

                var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                // Skip the header line and process each network
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Split by 2 or more spaces to separate columns
                    var parts = Regex.Split(line, @"\s{2,}");

                    if (parts.Length >= 8)
                    {
                        var network = new WiFiInfo();
                        
                        // IN-USE is the first part, check for '*'
                        network.IsActive = parts[0].Contains("*");
                        
                        // BSSID is second
                        // SSID is third
                        network.SSID = parts[2];
                        
                        // MODE is fourth
                        network.Mode = parts[3];
                        
                        // CHAN is fifth
                        network.Chan = parts[4];
                        
                        // RATE is sixth
                        network.Rate = parts[5];
                        
                        // SIGNAL is seventh
                        network.Signal = parts[6];
                        
                        // SECURITY is eighth and beyond
                        network.Security = parts.Length > 7 ? string.Join(" ", parts[7..]) : "";

                        networks.Add(network);
                        Console.WriteLine($"Parsed network: {network.SSID}, Active: {network.IsActive}");
                    }
                }
                Console.WriteLine($"Total networks parsed: {networks.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing WiFi: {ex.Message}");
                // Return empty list if parsing fails
            }

            return networks;
        }
    }
}
