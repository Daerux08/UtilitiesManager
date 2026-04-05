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
            if (!CheckDependencyCommand.IsJournalctlAvailable)
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

        public static async Task MenuService(CheckDependencyCommand checker)
        {
            var checker = new CheckDependencyCommand();
            if (!CheckDependencyCommand.IsJournalctlAvailable)
            {
                MenuEngine.ErrorMessage("journalctl is not available on this system.");
                return;
            }

            var menuOptions = new List<(string, Func<Task>)>
            {
                ("📄 System logs", async () => { await HandleLogsCommand(new string[] { "logs", "system" }); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("🔧 Kernel logs", async () => { await HandleLogsCommand(new string[] { "logs", "kernel" }); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("🚀 Boot logs", async () => { await HandleLogsCommand(new string[] { "logs", "boot" }); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("⬅ Back to main menu", async () => { throw new GoBackException(); })
            };

            await MenuEngine.DisplayMenuAsync(menuOptions);
        }
    }
}
