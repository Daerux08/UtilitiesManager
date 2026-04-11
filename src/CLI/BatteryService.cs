using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class BatteryService
    {
        public static void HandleBatteryCommand()
        {
            var checker = new CheckDependencyCommand();
            checker.LoadOriginalValues();
            var battery = checker.BatteryStatus;

            Console.WriteLine("Battery Status:");
            Console.WriteLine($"  State: {battery.State}");
            Console.WriteLine($"  Percentage: {battery.Percentage}%");
            Console.WriteLine($"  Time to Empty: {battery.TimeToEmpty}");
            Console.WriteLine($"  Time to Full: {battery.TimeToFull}");
            Console.WriteLine($"  Energy Rate: {battery.EnergyRate} W");
            Console.WriteLine($"  Present: {battery.IsPresent}");
        }



        public static void MenuService(CheckDependencyCommand checkerParam)
        {
            if (!checkerParam.IsUpowerAvailable)
            {
                MenuEngine.ShowError("Battery Status", "upower is not available on this system.");
                return;
            }

            checkerParam.LoadOriginalValues();
            var battery = checkerParam.BatteryStatus;

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
                MenuEngine.ShowMessage("Battery Status", batteryInfo, false);
                Console.WriteLine();

                var choice = MenuEngine.ShowArrowMenu("BATTERY STATUS", menuOptions);

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
    
