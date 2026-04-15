using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class VolumeService
    {
        public static async Task HandleVolumeCommand(string[] args)
        {
            if (args.Length > 1 && int.TryParse(args[1], out int volume) && volume >= 0 && volume <= 100)
            {
                var changer = new ChangeValueCommand();
                await changer.SetVolumeAsync(volume);
                Console.WriteLine($"Volume set to {volume}%");
            }
            else
            {
                Console.WriteLine("Usage: UtilMan volume <percentage (0-100)>");
            }
        }


        public static async Task MenuService(CheckDependencyCommand checkerParam)
        {
            checkerParam.CheckDependencies();
            if (!checkerParam.IsPactlAvailable)
            {
                MenuEngine.ShowError("Volume Control", "pactl (PulseAudio) is not available on this system.");
                return;
            }

            var currentVolume = checkerParam.GetVolume();

            while (true)
            {
                var menuOptions = new List<string>
                {
                    $"Set volume percentage (Current: {currentVolume}%)",
                    "Quick set (0%, 25%, 50%, 75%, 100%)",
                    "Mute/Unmute",
                    "Back to main menu"
                };

                var choice = MenuEngine.ShowArrowMenu("VOLUME CONTROL", menuOptions);

                switch (choice)
                {
                    case 0:
                        var input = MenuEngine.GetUserInput("Enter volume percentage (0-100)", currentVolume.ToString());
                        if (int.TryParse(input, out int volume) && volume >= 0 && volume <= 100)
                        {
                            var changer = new ChangeValueCommand();
                            await changer.SetVolumeAsync(volume);
                            currentVolume = volume;
                            MenuEngine.ShowMessage("Success", $"Volume set to {volume}%");
                        }
                        else
                        {
                            MenuEngine.ShowError("Invalid Input", "Please enter a number between 0 and 100.");
                        }
                        break;

                    case 1:
                        var quickOptions = new Dictionary<string, int>
                        {
                            ["0% (Mute)"] = 0,
                            ["25%"] = 25,
                            ["50%"] = 50,
                            ["75%"] = 75,
                            ["100% (Maximum)"] = 100
                        };

                        var quickChoice = MenuEngine.ShowQuickSelectMenu("Quick volume options", quickOptions);
                        if (quickChoice >= 0)
                        {
                            var selectedOption = quickOptions.ElementAt(quickChoice);
                            var quickVolume = selectedOption.Value;

                            var changer = new ChangeValueCommand();
                            await changer.SetVolumeAsync(quickVolume);
                            currentVolume = quickVolume;
                            MenuEngine.ShowMessage("Success", $"Volume set to {quickVolume}%");
                        }
                        break;

                    case 2:
                        var volumeChanger = new ChangeValueCommand();
                        await volumeChanger.SetVolumeAsync(0);
                        currentVolume = 0;
                        MenuEngine.ShowMessage("Success", "Volume muted (set to 0%)");
                        break;

                    case 3:
                        return;
                    case -1:
                        return;
                }
            }
        }
    }
}

