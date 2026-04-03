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
            var menuOptions = new List<(string, Func<Task>)>
            {
                ("📦 Install all packages", async () => { await DownloadScript.RunPackageInstallationAsync(); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("📋 Install packages individually", async () => { await DownloadScript.InstallIndividualPackagesAsync(); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("📊 Show package status", async () => { await DownloadScript.ShowPackageStatusAsync(); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("🔧 Setup hardware sensors", async () => { await DownloadScript.SetupSensorsAsync(); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("🛡️ Configure firewall", async () => { await DownloadScript.ConfigureFirewallAsync(); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("⬅ Back to main menu", async () => { throw new GoBackException(); })
            };

            await MenuEngine.DisplayMenuAsync(menuOptions);
        }
    }
}
