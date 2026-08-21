using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class SystemMonitoringService
    {
        public static async Task HandleSystemMonitoringCommand(CheckDependencyCommand checker, string command)
        {
            checker.CheckDependencies();
            var systemInfo = await checker.GetSystemInfoAsync();

            switch (command.ToLower())
            {
                case "cpu":
                    MenuEngine.GeneralMessage("CPU Information");
                     MenuEngine.GeneralMessage($"Uptime: {systemInfo.Uptime}");
                     MenuEngine.GeneralMessage($"Load Average: {string.Join(", ", systemInfo.LoadAverage)}");

                    if (systemInfo.Temperatures.Any())
                    {
                         MenuEngine.GeneralMessage("\nTemperatures:");
                        foreach (var temp in systemInfo.Temperatures)
                        {
                             MenuEngine.GeneralMessage($"  {temp.Key}: {temp.Value}");
                        }
                    }
                    break;

                    case "memory":
                        MenuEngine.GeneralMessage("=== MEMORY INFORMATION ===");
                        if (systemInfo.MemoryInfo.Any())
                        {
                            MenuEngine.GeneralMessage($"Memory Total: {(systemInfo.MemoryInfo.TryGetValue("Total", out var total) ? total : "N/A")}");
                            MenuEngine.GeneralMessage($"Memory Used: {(systemInfo.MemoryInfo.TryGetValue("Used", out var used) ? used : "N/A")}");
                            MenuEngine.GeneralMessage($"Memory Free: {(systemInfo.MemoryInfo.TryGetValue("Free", out var free) ? free : "N/A")}");
                            MenuEngine.GeneralMessage($"Swap Total: {(systemInfo.MemoryInfo.TryGetValue("SwapTotal", out var swapTotal) ? swapTotal : "N/A")}");
                            MenuEngine.GeneralMessage($"Swap Used: {(systemInfo.MemoryInfo.TryGetValue("SwapUsed", out var swapUsed) ? swapUsed : "N/A")}");
                            MenuEngine.GeneralMessage($"Swap Free: {(systemInfo.MemoryInfo.TryGetValue("SwapFree", out var swapFree) ? swapFree : "N/A")}");
                        }
                        else
                        {
                            MenuEngine.ErrorMessage("Memory information not available");
                        }
                        break;

                case "disk":
                     MenuEngine.GeneralMessage("=== DISK USAGE ===");
                    Console.WriteLine("{0,-15} {1,-8} {2,-8} {3,-8} {4,-6} {5,-20}",
                        "Filesystem", "Size", "Used", "Avail", "Use%", "Mount");
                    Console.WriteLine(new string('-', 80));

                    foreach (var disk in systemInfo.DiskUsage)
                    {
                        Console.WriteLine("{0,-15} {1,-8} {2,-8} {3,-8} {4,-6} {5,-20}",
                            disk.Filesystem, disk.Size, disk.Used, disk.Available, disk.UsePercent, disk.MountPoint);
                    }
                    break;

                case "network":
                     MenuEngine.GeneralMessage("=== NETWORK INTERFACES ===");
                    Console.WriteLine("{0,-15} {1,-15}", "Interface", "IP Address");
                    Console.WriteLine(new string('-', 30));

                    foreach (var net in systemInfo.NetworkInterfaces)
                    {
                        Console.WriteLine("{0,-15} {1,-15}", net.Interface, net.IPAddress);
                    }
                    break;
            }
        }

        public static async Task SystemMonitoringMenu()
        {
            var checker = new CheckDependencyCommand();
            checker.CheckDependencies();

            while (true)
            {
                var menuOptions = new List<string>
                {
                    "🖥️ CPU Information - Usage, load, temperature",
                    "💾 Memory Usage - RAM and swap usage",
                    "💿 Disk Usage - Storage space and mount points",
                    "🌐 Network Interfaces - IP addresses and connections",
                    "📊 Full System Overview - All information at once",
                    "⬅ Back to main menu"
                };

                var choice = MenuEngine.ShowArrowMenu("SYSTEM MONITORING", menuOptions);

                switch (choice)
                {
                    case 0:
                        await HandleSystemMonitoringCommand(checker, "cpu");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        await HandleSystemMonitoringCommand(checker, "memory");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        await HandleSystemMonitoringCommand(checker, "disk");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 3:
                        await HandleSystemMonitoringCommand(checker, "network");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 4:
                        var systemInfo = await checker.GetSystemInfoAsync();

                        // Display Full System Overview
                        Console.Clear();
                        Console.WriteLine("=== FULL SYSTEM OVERVIEW ===");
                        Console.WriteLine();

                        // CPU Information
                        await HandleSystemMonitoringCommand(checker, "cpu");
                        Console.WriteLine();

                        // Memory Information
                        await HandleSystemMonitoringCommand(checker, "memory");
                        Console.WriteLine();

                        // Disk Information
                        await HandleSystemMonitoringCommand(checker, "disk");
                        Console.WriteLine();

                        // Network Information
                        await HandleSystemMonitoringCommand(checker, "network");

                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 5:
                        return;
                    case -1:
                        return;
                }
            }
        }
    }
}
