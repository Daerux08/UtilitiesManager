using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class FirewallService
    {
        public static async Task HandleFirewallCommand(string[] args)
        {
            var checker = new CheckDependencyCommand();
            var firewall = await checker.GetFirewallStatusAsync();
            
            Console.WriteLine("=== FIREWALL STATUS ===");
            
            if (!string.IsNullOrEmpty(firewall.UfwStatus))
            {
                Console.WriteLine($"UFW Status: {firewall.UfwStatus}");
            }
            
            if (!string.IsNullOrEmpty(firewall.Fail2banStatus))
            {
                Console.WriteLine($"Fail2ban Status: {firewall.Fail2banStatus}");
            }
            
            if (firewall.IptablesRules.Any())
            {
                Console.WriteLine("\nRecent iptables rules:");
                foreach (var rule in firewall.IptablesRules.Take(5))
                {
                    Console.WriteLine($"  {rule}");
                }
            }
        }

        public static async Task FirewallManagementMenu()
        {
            while (true)
            {
                var menuOptions = new List<string>
                {
                    "🛡️ Show firewall status",
                    "⬅ Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("FIREWALL MANAGEMENT", menuOptions);

                switch (choice)
                {
                    case 0:
                        await HandleFirewallCommand(new string[] { "firewall" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
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
