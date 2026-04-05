using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class PowerService
    {
        public static async Task HandlePowerCommand(string[] args)
        {
            var powerChecker = new CheckDependencyCommand();
            if (args.Length > 1)
            {
                if (args[1].ToLower() == "get")
                {
                    var profile = await powerChecker.GetCurrentPowerProfileAsync();
                    Console.WriteLine($"Current power profile: {profile}");
                }
                else if (args[1].ToLower() == "set" && args.Length > 2)
                {
                    var changer = new ChangeValueCommand();
                    await changer.SetPowerProfileAsync(args[2]);
                    Console.WriteLine($"Power profile set to: {args[2]}");
                }
                else
                {
                    Console.WriteLine("Usage: UtilMan power get|set <profile>");
                }
            }
            else
            {
                Console.WriteLine("Usage: UtilMan power get|set <profile>");
            }
        }

        public static async Task MenuService(CheckDependencyCommand checker)
        {
            var checker = new CheckDependencyCommand();
            if (!CheckDependencyCommand.IsPowerProfilesCtlAvailable)
            {
                MenuEngine.ShowError("Power Profiles", "powerprofilesctl is not available on this system.");
                return;
            }

            while (true)
            {
                var currentProfile = await checker.GetCurrentPowerProfileAsync();

                var menuOptions = new List<string>
                {
                    $"⚡ Set performance mode {(currentProfile == "performance" ? "[CURRENT]" : "")}",
                    $"⚖️ Set balanced mode {(currentProfile == "balanced" ? "[CURRENT]" : "")}",
                    $"🔋 Set power-saver mode {(currentProfile == "power-saver" ? "[CURRENT]" : "")}",
                    $"📊 Show current profile",
                    $"⬅ Back to main menu"
                };

                var choice = MenuEngine.ShowArrowMenu("POWER PROFILES", menuOptions);

                switch (choice)
                {
                    case 0:
                        var changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("performance");
                        MenuEngine.ShowMessage("Success", "Power profile set to performance");
                        break;

                    case 1:
                        changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("balanced");
                        MenuEngine.ShowMessage("Success", "Power profile set to balanced");
                        break;

                    case 2:
                        changer = new ChangeValueCommand();
                        await changer.SetPowerProfileAsync("power-saver");
                        MenuEngine.ShowMessage("Success", "Power profile set to power-saver");
                        break;

                    case 3:
                        currentProfile = await checker.GetCurrentPowerProfileAsync();
                        MenuEngine.ShowMessage("Current Profile", $"Current power profile: {currentProfile}");
                        break;

                    case 4:
                        return;

                    case -1:
                        return;
                }
            }
        }
    }
}
