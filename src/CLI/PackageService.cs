using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class PackageService
    {
        public static async Task HandleDownloadCommand(string[] args)
        {
            if (args.Length > 1)
            {
                var action = args[1].ToLower();
                
                switch (action)
                {
                    case "all":
                        await DownloadScript.RunPackageInstallationAsync();
                        break;
                    case "individual":
                        await DownloadScript.InstallIndividualPackagesAsync();
                        break;
                    case "status":
                        await DownloadScript.ShowPackageStatusAsync();
                        break;
                    case "sensors":
                        await DownloadScript.SetupSensorsAsync();
                        break;
                    case "firewall":
                        await DownloadScript.ConfigureFirewallAsync();
                        break;
                    default:
                        Console.WriteLine("Usage: UtilMan install all|individual|status|sensors|firewall");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Usage: UtilMan install all|individual|status|sensors|firewall");
            }
        }

        public static async Task PackageInstallationMenu()
        {
            while (true)
            {
                var menuOptions = new List<string>
                {
                    "📦 Install all packages",
                    "📋 Install packages individually",
                    "📊 Show package status",
                    "🔧 Setup hardware sensors",
                    "🛡️ Configure firewall",
                    "⬅ Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("PACKAGE INSTALLATION", menuOptions);

                switch (choice)
                {
                    case 0:
                        await DownloadScript.RunPackageInstallationAsync();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        await DownloadScript.InstallIndividualPackagesAsync();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        await DownloadScript.ShowPackageStatusAsync();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 3:
                        await DownloadScript.SetupSensorsAsync();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 4:
                        await DownloadScript.ConfigureFirewallAsync();
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
