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
            await checker.CheckDependenciesAsync();
            var systemInfo = await checker.GetSystemInfoAsync();

            switch (command.ToLower())
            {
                case "cpu":
                    Console.WriteLine("=== CPU INFORMATION ===");
                    Console.WriteLine($"Uptime: {systemInfo.Uptime}");
                    Console.WriteLine($"Load Average: {string.Join(", ", systemInfo.LoadAverage)}");
                    
                    if (systemInfo.Temperatures.Any())
                    {
                        Console.WriteLine("\nTemperatures:");
                        foreach (var temp in systemInfo.Temperatures)
                        {
                            Console.WriteLine($"  {temp.Key}: {temp.Value}");
                        }
                    }
                    break;

                case "memory":
                    Console.WriteLine("=== MEMORY INFORMATION ===");
                    if (systemInfo.MemoryInfo.Any())
                    {
                        Console.WriteLine($"Memory Total: {(systemInfo.MemoryInfo.TryGetValue("Total", out var total) ? total : "N/A")}");
                        Console.WriteLine($"Memory Used: {(systemInfo.MemoryInfo.TryGetValue("Used", out var used) ? used : "N/A")}");
                        Console.WriteLine($"Memory Free: {(systemInfo.MemoryInfo.TryGetValue("Free", out var free) ? free : "N/A")}");
                        Console.WriteLine($"Swap Total: {(systemInfo.MemoryInfo.TryGetValue("SwapTotal", out var swapTotal) ? swapTotal : "N/A")}");
                        Console.WriteLine($"Swap Used: {(systemInfo.MemoryInfo.TryGetValue("SwapUsed", out var swapUsed) ? swapUsed : "N/A")}");
                        Console.WriteLine($"Swap Free: {(systemInfo.MemoryInfo.TryGetValue("SwapFree", out var swapFree) ? swapFree : "N/A")}");
                    }
                    else
                    {
                        Console.WriteLine("Memory information not available");
                    }
                    break;

                case "disk":
                    Console.WriteLine("=== DISK USAGE ===");
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
                    Console.WriteLine("=== NETWORK INTERFACES ===");
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
            await checker.CheckDependenciesAsync();

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
