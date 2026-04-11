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
            checker.LoadOriginalValues();
            
            Console.WriteLine("=== SYSTEM STATUS ===");
            
            // Battery
            var battery = checker.BatteryStatus;
            Console.WriteLine($"Battery: {battery.Percentage}% ({battery.State})");
            if (!string.IsNullOrEmpty(battery.TimeToEmpty))
                Console.WriteLine($"  Time to empty: {battery.TimeToEmpty}");
            
            // Brightness
            if (checker.IsBrightnessCtlAvailable)
            {
                var brightness = checker.GetBrightnessPercent();
                Console.WriteLine($"Brightness: {brightness}%");
            }
            else
            {
                Console.WriteLine("Brightness: Not available");
            }
            
            // Volume
            if (checker.IsPactlAvailable)
            {
                var volume = checker.GetVolume();
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
    }
}
