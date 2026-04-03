using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class UserService
    {
        public static async Task HandleUsersCommand(string[] args)
        {
            var checker = new CheckDependencyCommand();
            var users = await checker.GetUsersAsync();
            
            Console.WriteLine("=== USER INFORMATION ===");
            Console.WriteLine("{0,-15} {1,-6} {2,-6} {3,-15} {4,-10} {5,-5}", 
                "Username", "UID", "GID", "Home", "Shell", "Online");
            Console.WriteLine(new string('-', 70));
            
            foreach (var user in users)
            {
                Console.WriteLine("{0,-15} {1,-6} {2,-6} {3,-15} {4,-10} {5,-5}",
                    user.Username, user.UID, user.GID, user.Home, user.Shell, user.IsLoggedIn ? "Yes" : "No");
            }
        }

        public static async Task UserManagementMenu()
        {
            var menuOptions = new List<(string, Func<Task>)>
            {
                ("👥 List users", async () => { await HandleUsersCommand(new string[] { "users" }); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("🔐 Show logged in users", async () => { var checker = new CheckDependencyCommand(); if (checker.IsProcpsAvailable) { var whoOutput = await TerminalCommands.RunCommandAsync("who"); MenuEngine.GeneralMessage(whoOutput); } else { MenuEngine.ErrorMessage("who command is not available on this system."); } MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("⬅ Back to main menu", async () => { throw new GoBackException(); })
            };

            await MenuEngine.DisplayMenuAsync(menuOptions);
        }
    }
}
