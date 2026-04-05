using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class StatusService
    {
        public static async Task HandleStatusCommand()
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
            if (CheckDependencyCommand.IsBrightnessCtlAvailable)
            {
                var brightness = await checker.GetBrightnessPercentAsync();
                Console.WriteLine($"Brightness: {brightness}%");
            }
            else
            {
                Console.WriteLine("Brightness: Not available");
            }
            
            // Volume
            if (CheckDependencyCommand.IsPactlAvailable)
            {
                var volume = await checker.GetVolumeAsync();
                Console.WriteLine($"Volume: {volume}%");
            }
            else
            {
                Console.WriteLine("Volume: Not available");
            }
            
            // Power profile
            if (CheckDependencyCommand.IsPowerProfilesCtlAvailable)
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
            Console.WriteLine($"brightnessctl: {(CheckDependencyCommand.IsBrightnessCtlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"pactl: {(CheckDependencyCommand.IsPactlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"upower: {(CheckDependencyCommand.IsUpowerAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"nmcli: {(CheckDependencyCommand.IsNmcliAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"powerprofilesctl: {(CheckDependencyCommand.IsPowerProfilesCtlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"procps (ps/free): {(CheckDependencyCommand.IsProcpsAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"lm-sensors: {(CheckDependencyCommand.IsLmSensorsAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"sysstat: {(CheckDependencyCommand.IsSysstatAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"systemctl: {(CheckDependencyCommand.IsSystemctlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"journalctl: {(CheckDependencyCommand.IsJournalctlAvailable ? "Available" : "Not available")}");
            Console.WriteLine($"ufw: {(CheckDependencyCommand.IsUfwAvailable ? "Available" : "Not available")}");
        }
    }
}
