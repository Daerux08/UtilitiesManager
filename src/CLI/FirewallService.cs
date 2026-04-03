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
            var menuOptions = new List<(string, Func<Task>)>
            {
                ("🛡️ Show firewall status", async () => { await HandleFirewallCommand(new string[] { "firewall" }); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("⬅ Back to main menu", async () => { throw new GoBackException(); })
            };

            await MenuEngine.DisplayMenuAsync(menuOptions);
        }
    }
}
