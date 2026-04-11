using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class BrightnessService
    {
        public static async Task HandleBrightnessCommand(string[] args)
        {
            if (args.Length > 1 && int.TryParse(args[1], out int brightness) && brightness >= 0 && brightness <= 100)
            {
                var changer = new ChangeValueCommand();
                await changer.SetBrightnessAsync(brightness);
                Console.WriteLine($"Brightness set to {brightness}%");
            }
            else
            {
                Console.WriteLine("Usage: UtilMan brightness <percentage (0-100)>");
            }
        }
        
        
        public static async Task BrightnessMenu()
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsBrightnessCtlAvailable)
            {
                MenuEngine.ErrorMessage("brightnessctl is not available on this system.");
                return;
            }

            var currentBrightness = checker.GetBrightnessPercent();

            var menuOptions = new List<(string, Func<Task>)>
            {
                ($"Set brightness percentage (Current: {currentBrightness}%)", async () => {
                    var input = MenuEngine.TextInput($"Enter brightness percentage (0-100) [{currentBrightness}]");
                    if (int.TryParse(input, out int brightness) && brightness >= 0 && brightness <= 100)
                    {
                        var changer = new ChangeValueCommand();
                        await changer.SetBrightnessAsync(brightness);
                        currentBrightness = brightness;
                        MenuEngine.GeneralMessage($"Brightness set to {brightness}%");
                    }
                    else
                    {
                        MenuEngine.ErrorMessage("Please enter a number between 0 and 100.");
                    }
                }),
                ("Quick set (0%, 25%, 50%, 75%, 100%)", async () => {
                    var quickOptions = new List<(string, Func<Task>)>
                    {
                        ("0% (Off)", async () => { var changer = new ChangeValueCommand(); await changer.SetBrightnessAsync(0); currentBrightness = 0; MenuEngine.GeneralMessage("Brightness set to 0%"); }),
                        ("25%", async () => { var changer = new ChangeValueCommand(); await changer.SetBrightnessAsync(25); currentBrightness = 25; MenuEngine.GeneralMessage("Brightness set to 25%"); }),
                        ("50%", async () => { var changer = new ChangeValueCommand(); await changer.SetBrightnessAsync(50); currentBrightness = 50; MenuEngine.GeneralMessage("Brightness set to 50%"); }),
                        ("75%", async () => { var changer = new ChangeValueCommand(); await changer.SetBrightnessAsync(75); currentBrightness = 75; MenuEngine.GeneralMessage("Brightness set to 75%"); }),
                        ("100% (Maximum)", async () => { var changer = new ChangeValueCommand(); await changer.SetBrightnessAsync(100); currentBrightness = 100; MenuEngine.GeneralMessage("Brightness set to 100%"); }),
                        ("Back", async () => { throw new GoBackException(); })
                    };
                    var quickMenu = quickOptions.Select(x => (x.Item1, new Action(() => x.Item2().GetAwaiter().GetResult()))).ToList();
                    MenuEngine.DisplayMenu(quickMenu);
                }),
                ("Back to main menu", async () => { throw new GoBackException(); })
            };

            await MenuEngine.DisplayMenuAsync(menuOptions);
        }
    }
}
