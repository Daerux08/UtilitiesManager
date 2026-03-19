using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class LogService
    {
        public static async Task HandleLogsCommand(string[] args)
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsJournalctlAvailable)
            {
                Console.WriteLine("journalctl is not available on this system.");
                return;
            }

            var logType = args.Length > 1 ? args[1].ToLower() : "system";
            var logs = await checker.GetRecentLogsAsync(logType);
            
            Console.WriteLine($"=== RECENT LOGS ({logType.ToUpper()}) ===");
            Console.WriteLine("{0,-20} {1}", "Timestamp", "Message");
            Console.WriteLine(new string('-', 80));
            
            foreach (var log in logs.Take(15))
            {
                Console.WriteLine("{0,-20} {1}", log.Timestamp, log.Message);
            }
        }

        public static async Task LogManagementMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsJournalctlAvailable)
            {
                MenuHelper.ShowError("Log Management", "journalctl is not available on this system.");
                return;
            }

            while (true)
            {
                var menuOptions = new List<string>
                {
                    "📄 System logs",
                    "🔧 Kernel logs",
                    "🚀 Boot logs",
                    "⬅ Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("LOG MANAGEMENT", menuOptions);

                switch (choice)
                {
                    case 0:
                        await HandleLogsCommand(new string[] { "logs", "system" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        await HandleLogsCommand(new string[] { "logs", "kernel" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        await HandleLogsCommand(new string[] { "logs", "boot" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 3:
                        return;
                    case -1:
                        return;
                }
            }
        }
    }
}
