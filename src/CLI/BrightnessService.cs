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
                MenuHelper.ShowError("Brightness Control", "brightnessctl is not available on this system.");
                return;
            }

            var currentBrightness = await checker.GetBrightnessPercentAsync();

            while (true)
            {
                var menuOptions = new List<string>
                {
                    $"Set brightness percentage (Current: {currentBrightness}%)",
                    "Quick set (0%, 25%, 50%, 75%, 100%)",
                    "Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("BRIGHTNESS CONTROL", menuOptions);

                switch (choice)
                {
                    case 0:
                        var input = MenuHelper.GetUserInput("Enter brightness percentage (0-100)", currentBrightness.ToString());
                        if (int.TryParse(input, out int brightness) && brightness >= 0 && brightness <= 100)
                        {
                            var changer = new ChangeValueCommand();
                            await changer.SetBrightnessAsync(brightness);
                            currentBrightness = brightness;
                            MenuHelper.ShowMessage("Success", $"Brightness set to {brightness}%");
                        }
                        else
                        {
                            MenuHelper.ShowError("Invalid Input", "Please enter a number between 0 and 100.");
                        }
                        break;

                    case 1:
                        var quickOptions = new Dictionary<string, int>
                        {
                            ["0% (Off)"] = 0,
                            ["25%"] = 25,
                            ["50%"] = 50,
                            ["75%"] = 75,
                            ["100% (Maximum)"] = 100
                        };

                        var quickChoice = MenuHelper.ShowQuickSelectMenu("Quick brightness options", quickOptions);
                        if (quickChoice >= 0)
                        {
                            var selectedOption = quickOptions.ElementAt(quickChoice);
                            var quickBrightness = selectedOption.Value;

                            var changer = new ChangeValueCommand();
                            await changer.SetBrightnessAsync(quickBrightness);
                            currentBrightness = quickBrightness;
                            MenuHelper.ShowMessage("Success", $"Brightness set to {quickBrightness}%");
                        }
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
