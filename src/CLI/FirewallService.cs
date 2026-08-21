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

            MenuEngine.GeneralMessage("=== FIREWALL STATUS ===");

            if (!string.IsNullOrEmpty(firewall.UfwStatus))
            {
                MenuEngine.GeneralMessage($"UFW Status: {firewall.UfwStatus}");
            }

            if (!string.IsNullOrEmpty(firewall.Fail2banStatus))
            {
                MenuEngine.GeneralMessage($"Fail2ban Status: {firewall.Fail2banStatus}");
            }

            if (firewall.IptablesRules.Any())
            {
                MenuEngine.GeneralMessage("\nRecent iptables rules:");
                foreach (var rule in firewall.IptablesRules.Take(5))
                {
                    MenuEngine.GeneralMessage($"  {rule}");
                }
            }
        }

        public static async Task MenuService(CheckDependencyCommand checker)
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
