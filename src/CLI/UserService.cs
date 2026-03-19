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
            while (true)
            {
                var menuOptions = new List<string>
                {
                    "👥 List users",
                    "🔐 Show logged in users",
                    "⬅ Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("USER MANAGEMENT", menuOptions);

                switch (choice)
                {
                    case 0:
                        await HandleUsersCommand(new string[] { "users" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        var checker = new CheckDependencyCommand();
                        if (checker.IsProcpsAvailable)
                        {
                            var whoOutput = await TerminalCommands.RunCommandAsync("who");
                            MenuHelper.ShowMessage("Logged In Users", whoOutput);
                        }
                        else
                        {
                            MenuHelper.ShowError("User Management", "who command is not available on this system.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        return;
                    case -1:
                        return;
                }
            }
        }
    }
}
