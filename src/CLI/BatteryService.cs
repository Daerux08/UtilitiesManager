using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class BatteryService
    {
        public static async Task HandleBatteryCommand()
        {
            var checker = new CheckDependencyCommand();
            await checker.LoadOriginalValuesAsync();
            var battery = checker.BatteryStatus;

            Console.WriteLine("Battery Status:");
            Console.WriteLine($"  State: {battery.State}");
            Console.WriteLine($"  Percentage: {battery.Percentage}%");
            Console.WriteLine($"  Time to Empty: {battery.TimeToEmpty}");
            Console.WriteLine($"  Time to Full: {battery.TimeToFull}");
            Console.WriteLine($"  Energy Rate: {battery.EnergyRate} W");
            Console.WriteLine($"  Present: {battery.IsPresent}");
        }



        public static async Task PowerMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsPowerProfilesCtlAvailable)
            {
                MenuHelper.ShowError("Power Profiles", "powerprofilesctl is not available on this system.");
                return;
            }

            while (true)
            {
                var currentProfile = await checker.GetCurrentPowerProfileAsync();

                var menuOptions = new List<string>
                {
                    $"⚡ Set performance mode {(currentProfile == "performance" ? "[CURRENT]" : "")}",
                    $"⚖️ Set balanced mode {(currentProfile == "balanced" ? "[CURRENT]" : "")}",
                    $"🔋 Set power-saver mode {(currentProfile == "power-saver" ? "[CURRENT]" : "")}",
                    "📊 Show current profile",
                    "⬅ Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("POWER PROFILES", menuOptions);

                switch (choice)
                {
                    case 0:
                        var changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("performance");
                        MenuHelper.ShowMessage("Success", "Power profile set to performance");
                        break;

                    case 1:
                        changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("balanced");
                        MenuHelper.ShowMessage("Success", "Power profile set to balanced");
                        break;

                    case 2:
                        changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("power-saver");
                        MenuHelper.ShowMessage("Success", "Power profile set to power-saver");
                        break;

                    case 3:
                        currentProfile = await checker.GetCurrentPowerProfileAsync();
                        MenuHelper.ShowMessage("Current Profile", $"Current power profile: {currentProfile}");
                        break;

                    case 4:
                        return;

                    case -1:
                        return;
                }
            }
        }


        public static async Task BatteryMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsUpowerAvailable)
            {
                MenuHelper.ShowError("Battery Status", "upower is not available on this system.");
                return;
            }

            await checker.LoadOriginalValuesAsync();
            var battery = checker.BatteryStatus;

            var batteryInfo = $"State: {battery.State}\n" +
                             $"Percentage: {battery.Percentage}%\n" +
                             $"Time to Empty: {battery.TimeToEmpty}\n" +
                             $"Time to Full: {battery.TimeToFull}\n" +
                             $"Energy Rate: {battery.EnergyRate} W\n" +
                             $"Present: {battery.IsPresent}";

            while (true)
            {
                var menuOptions = new List<string>
                {
                    "Refresh battery status",
                    "Back to main menu"
                };

                // Show battery info before menu
                MenuHelper.ShowMessage("Battery Status", batteryInfo, false);
                Console.WriteLine();

                var choice = MenuHelper.ShowArrowMenu("BATTERY STATUS", menuOptions);

                switch (choice)
                {
                    case 0:
                        // Refresh is automatic when menu loads again
                        break;
                    case 1:
                        return;
                    case -1:
                        return;
                }
            }
        }
    }

}
    
