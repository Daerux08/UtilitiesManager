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

            MenuEngine.GeneralMessage("=== SYSTEM STATUS ===");

            // Battery
            var battery = checker.BatteryStatus;
            MenuEngine.GeneralMessage($"Battery: {battery.Percentage}% ({battery.State})");
            if (!string.IsNullOrEmpty(battery.TimeToEmpty))
                MenuEngine.GeneralMessage($"  Time to empty: {battery.TimeToEmpty}");

                // Brightness
                if (checker.IsBrightnessCtlAvailable)
                {
                    var brightness = checker.GetBrightnessPercent();
                    MenuEngine.GeneralMessage($"Brightness: {brightness}%");
                }
                else
                {
                    MenuEngine.ErrorMessage("Brightness: Not available");
                }

                // Volume
                if (checker.IsPactlAvailable)
                {
                    var volume = checker.GetVolume();
                    MenuEngine.GeneralMessage($"Volume: {volume}%");
                }
                else
                {
                    MenuEngine.ErrorMessage("Volume: Not available");
                }

                // Power profile
                if (checker.IsPowerProfilesCtlAvailable)
                {
                    var profile = await checker.GetCurrentPowerProfileAsync();
                    MenuEngine.GeneralMessage($"Power Profile: {profile}");
                }
                else
                {
                    MenuEngine.ErrorMessage("Power Profile: Not available");
                }

            
            MenuEngine.GeneralMessage("=== DEPENDENCIES ===");
            MenuEngine.GeneralMessage($"brightnessctl: {(checker.IsBrightnessCtlAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"pactl: {(checker.IsPactlAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"upower: {(checker.IsUpowerAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"nmcli: {(checker.IsNmcliAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"powerprofilesctl: {(checker.IsPowerProfilesCtlAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"procps (ps/free): {(checker.IsProcpsAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"lm-sensors: {(checker.IsLmSensorsAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"sysstat: {(checker.IsSysstatAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"systemctl: {(checker.IsSystemctlAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"journalctl: {(checker.IsJournalctlAvailable ? "Available" : "Not available")}");
            MenuEngine.GeneralMessage($"ufw: {(checker.IsUfwAvailable ? "Available" : "Not available")}");
        }
    }
}
