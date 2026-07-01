using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilitiesManager
{
    public static class Help
    {
        public static void ShowAllHelp()
        {
            FirstHelpMenu();
        }

        private static void FirstHelpMenu()
        {
            var helpTopics = new List<(string, Action)>
            {
                ("Brightness", ShowBrightnessHelp),
                ("Volume", ShowVolumeHelp),
                ("Battery", ShowBatteryHelp),
                ("WiFi", ShowWiFiHelp),
                ("Bluetooth", ShowBluetoothHelp),
                ("Power", ShowPowerHelp),
                ("Status", ShowStatusHelp),
                ("General", ShowGeneralHelp)
            };

            var selectedIndex = MenuEngine.ShowArrowMenu("Select a help topic:", helpTopics.Select(topic => topic.Item1).ToList());

            if (selectedIndex >= 0 && selectedIndex < helpTopics.Count)
            {
                helpTopics[selectedIndex].Item2();
            }
        }

        private static void ShowBrightnessHelp()
        {
            MenuEngine.ShowMessage("BRIGHTNESS", "[bold cyan]brightness <percentage>[/]     Set screen brightness (0-100)", false);
            MenuEngine.GeneralMessage("Examples:");
            MenuEngine.GeneralMessage("[cyan]brightness 75[/] → Set brightness to 75%");
            MenuEngine.GeneralMessage("[cyan]brightness 0[/] → Turn off screen");
            MenuEngine.GeneralMessage("[cyan]brightness 100[/] → Maximum brightness");

            MenuEngine.ShowError("System Requirements", "[red]•[/] brightnessctl package must be installed\n[red]•[/] User must have permission to control backlight");

            MenuEngine.GeneralMessage("Linux Commands Used:");
            MenuEngine.GeneralMessage("[cyan]brightnessctl get[/] → Get current brightness value");
            MenuEngine.GeneralMessage("[cyan]brightnessctl max[/] → Get maximum brightness value");
            MenuEngine.GeneralMessage("[cyan]brightnessctl set X%[/] → Set brightness to X percent");

            MenuEngine.ShowMessage("Dependencies", "[yellow]•[/] brightnessctl (usually in brightnessctl package)\n[yellow]•[/] Linux kernel backlight support", false);

            MenuEngine.ShowMessage("Notes", "[italic]• Only works on systems with controllable backlight\n• May require udev rules for user permissions\n• Some laptops have multiple backlight devices[/]", false);
        }

        private static void ShowVolumeHelp()
        {
            MenuEngine.ShowMessage("VOLUME", "[bold cyan]volume <percentage>[/]         Set audio volume (0-100)", false);
            MenuEngine.GeneralMessage("Examples:");
            MenuEngine.GeneralMessage("[cyan]volume 50[/] → Set volume to 50%");
            MenuEngine.GeneralMessage("[cyan]volume 0[/] → Mute audio");
            MenuEngine.GeneralMessage("[cyan]volume 100[/] → Maximum volume");

            MenuEngine.ShowError("System Requirements", "[red]•[/] PulseAudio must be running\n[red]•[/] pactl package must be installed\n[red]•[/] User must be in audio group");

            MenuEngine.GeneralMessage("Linux Commands Used:");
            MenuEngine.GeneralMessage("[cyan]pactl get-sink-volume @DEFAULT_SINK@[/] → Get current volume");
            MenuEngine.GeneralMessage("[cyan]pactl set-sink-volume @DEFAULT_SINK@ X%[/] → Set volume to X percent");

            MenuEngine.ShowMessage("Dependencies", "[yellow]•[/] pulseaudio (PulseAudio sound server)\n[yellow]•[/] pactl (PulseAudio command-line tool)\n[yellow]•[/] libpulse (PulseAudio libraries)", false);

            MenuEngine.ShowMessage("Notes", "[italic]• Controls the default audio sink (output device)\n• Works with most modern Linux desktop environments\n• Volume is applied system-wide, not per-application[/]", false);
        }

        private static void ShowBatteryHelp()
        {
            MenuEngine.ShowMessage("BATTERY", "[bold cyan]battery[/]                      Show detailed battery status", false);
            MenuEngine.GeneralMessage("Examples:");
            MenuEngine.GeneralMessage("[cyan]battery[/] → Show current battery information");

            MenuEngine.ShowMessage("Information Displayed", "[blue]•[/] State: charging/discharging/fully-charged/unknown\n[blue]•[/] Percentage: Current charge level\n[blue]•[/] Time to Empty: Estimated remaining time\n[blue]•[/] Time to Full: Time until fully charged\n[blue]•[/] Energy Rate: Current power draw in watts\n[blue]•[/] Present: Whether battery is detected", false);

            MenuEngine.ShowError("System Requirements", "[red]•[/] upower package must be installed\n[red]•[/] UPower service must be running");

            MenuEngine.GeneralMessage("Linux Commands Used:");
            MenuEngine.GeneralMessage("[cyan]upower -e | grep battery[/] → Find battery device path");
            MenuEngine.GeneralMessage("[cyan]upower -i {device}[/] → Get battery information");

            MenuEngine.ShowMessage("Dependencies", "[yellow]•[/] upower (power management service)\n[yellow]•[/] libupower (UPower libraries)\n[yellow]•[/] Linux kernel power management", false);

            MenuEngine.ShowMessage("Notes", "[italic]• Works with most laptop batteries\n• UPS devices may also be detected\n• Some systems report battery information differently[/]", false);
        }

        private static void ShowWiFiHelp()
        {
            MenuEngine.ShowMessage("WIFI", "[bold cyan]wifi list[/]                    List available WiFi networks", false);
            MenuEngine.GeneralMessage("Examples:");
            MenuEngine.GeneralMessage("[cyan]wifi list[/] → Show all available networks");

            MenuEngine.ShowMessage("Information Displayed", "[blue]•[/] SSID: Network name\n[blue]•[/] Mode: Infrastructure/Ad-Hoc/AP\n[blue]•[/] Chan: WiFi channel number\n[blue]•[/] Rate: Connection speed (Mbit/s or Gbit/s)\n[blue]•[/] Signal: Signal strength (dBm or percentage)\n[blue]•[/] Security: Encryption type (WPA2, WEP, Open, etc.)\n[blue]•[/] * marks currently connected network", false);

            MenuEngine.ShowError("System Requirements", "[red]•[/] NetworkManager must be running\n[red]•[/] nmcli package must be installed\n[red]•[/] WiFi adapter must be enabled");

            MenuEngine.GeneralMessage("Linux Commands Used:");
            MenuEngine.GeneralMessage("[cyan]nmcli device wifi rescan[/] → Trigger fresh network scan");
            MenuEngine.GeneralMessage("[cyan]nmcli device wifi list[/] → List available networks");
            MenuEngine.GeneralMessage("[cyan]nmcli device wifi connect[/] → Connect to network");

            MenuEngine.ShowMessage("Dependencies", "[yellow]•[/] NetworkManager (network connection manager)\n[yellow]•[/] nmcli (NetworkManager CLI tool)\n[yellow]•[/] wpa_supplicant (WiFi authentication)\n[yellow]•[/] Linux kernel WiFi drivers", false);

            MenuEngine.ShowMessage("Notes", "[italic]• Only lists networks, doesn't connect (GUI feature)\n• Uses multiple parsing strategies for compatibility\n• Works across different Linux distributions\n• May need sudo for some network operations[/]", false);
        }

        private static void ShowBluetoothHelp()
        {
            MenuEngine.ShowMessage("BLUETOOTH", "[bold cyan]bluetooth list[/]                List available Bluetooth devices\n[bold cyan]bluetooth connect <device>[/]    Connect to Bluetooth device", false);
            MenuEngine.GeneralMessage("Examples:");
            MenuEngine.GeneralMessage("[cyan]bluetooth list[/] → Show all available devices");
            MenuEngine.GeneralMessage("[cyan]bluetooth connect 00:11:22:33:44:55[/] → Connect to device with specified MAC address");

            MenuEngine.ShowMessage("Information Displayed", "[blue]•[/] Device: Device name\n[blue]•[/] Address: Device MAC address\n[blue]•[/] RSSI: Signal strength (dBm)\n[blue]•[/] Paired: Whether device is paired\n[blue]•[/] Connected: Whether device is connected", false);

            MenuEngine.ShowError("System Requirements", "[red]•[/] bluez package must be installed\n[red]•[/] bluetooth service must be running\n[red]•[/] Bluetooth adapter must be enabled");

            MenuEngine.GeneralMessage("Linux Commands Used:");
            MenuEngine.GeneralMessage("[cyan]bluetoothctl devices[/] → List available devices");
            MenuEngine.GeneralMessage("[cyan]bluetoothctl connect <device>[/] → Connect to device");

            MenuEngine.ShowMessage("Dependencies", "[yellow]•[/] bluez (Bluetooth daemon)\n[yellow]•[/] bluetoothctl (Bluetooth CLI tool)\n[yellow]•[/] Linux kernel Bluetooth drivers", false);

            MenuEngine.ShowMessage("Notes", "[italic]• Only lists devices, doesn't connect (GUI feature)\n• Uses multiple parsing strategies for compatibility\n• Works across different Linux distributions\n• May need sudo for some Bluetooth operations[/]", false);
        }

        private static void ShowPowerHelp()
        {
            MenuEngine.ShowMessage("POWER", "[bold cyan]power get[/]                    Get current power profile\n[bold cyan]power set <profile>[/]          Set power profile", false);
            MenuEngine.GeneralMessage("Examples:");
            MenuEngine.GeneralMessage("[cyan]power get[/] → Show current power profile");
            MenuEngine.GeneralMessage("[cyan]power set performance[/] → Set performance mode");
            MenuEngine.GeneralMessage("[cyan]power set balanced[/] → Set balanced mode");
            MenuEngine.GeneralMessage("[cyan]power set power-saver[/] → Set power-saver mode");

            MenuEngine.ShowMessage("Available Profiles", "[green]•[/] [bold]performance[/]: Maximum performance, higher power usage\n[green]•[/] [bold]balanced[/]: Balance between performance and power\n[green]•[/] [bold]power-saver[/]: Minimum power usage, reduced performance", false);

            MenuEngine.ShowError("System Requirements", "[red]•[/] powerprofilesctl package must be installed\n[red]•[/] power-profiles-daemon service must be running\n[red]•[/] System must support power profiling");

            MenuEngine.GeneralMessage("Linux Commands Used:");
            MenuEngine.GeneralMessage("[cyan]powerprofilesctl get[/] → Get current power profile");
            MenuEngine.GeneralMessage("[cyan]powerprofilesctl set X[/] → Set power profile to X");

            MenuEngine.ShowMessage("Dependencies", "[yellow]•[/] power-profiles-daemon (power management daemon)\n[yellow]•[/] powerprofilesctl (CLI tool)\n[yellow]•[/] Linux kernel power management features", false);

            MenuEngine.ShowMessage("Notes", "[italic]• Only available on systems with power profile support\n• Modern laptops with Intel/AMD processors typically support this\n• Profiles affect CPU frequency, GPU performance, etc.[/]", false);
        }

        private static void ShowStatusHelp()
        {
            MenuEngine.ShowMessage("STATUS", "[bold cyan]status[/]                       Show all system status", false);
            MenuEngine.GeneralMessage("Examples:");
            MenuEngine.GeneralMessage("[cyan]status[/] → Show complete system overview");

            MenuEngine.ShowMessage("Information Displayed", "[blue]•[/] Battery: Current charge and state\n[blue]•[/] Brightness: Current brightness level\n[blue]•[/] Volume: Current audio volume\n[blue]•[/] Power Profile: Current power management mode\n[blue]•[/] Dependencies: Availability of required tools", false);

            MenuEngine.ShowError("System Requirements", "[red]•[/] Any combination of supported dependencies\n[red]•[/] Gracefully handles missing tools");

            MenuEngine.ShowMessage("Dependencies Checked", "[yellow]•[/] brightnessctl: For brightness control\n[yellow]•[/] pactl: For audio volume control\n[yellow]•[/] upower: For battery monitoring\n[yellow]•[/] nmcli: For WiFi management\n[yellow]•[/] bluetoothctl: For Bluetooth management\n[yellow]•[/] powerprofilesctl: For power profile management", false);

            MenuEngine.ShowMessage("Notes", "[italic]• Shows \"Not available\" for missing dependencies\n• Useful for troubleshooting and system validation\n• Works even when some features are unavailable[/]", false);
        }

        private static void ShowGeneralHelp()
        {
            MenuEngine.GeneralMessage("GENERAL INFORMATION");

            MenuEngine.ShowMessage("ABOUT", "[italic]Utilities Manager is a Linux system utility manager for controlling brightness, volume, battery, WiFi, Bluetooth, and power profiles via command line.[/]", false);

            MenuEngine.ShowMessage("COMPATIBILITY", "[green]•[/] Tested on: Linux Mint 22.2, Debian 13, Ubuntu 24.04\n[green]•[/] Requires: NetworkManager and standard Linux tools\n[green]•[/] Architecture: Supports x86_64, ARM64", false);

            MenuEngine.ShowMessage("SECURITY", "[green]•[/] No external dependencies beyond system packages\n[green]•[/] Commands executed with current user permissions\n[green]•[/] No network connections or data collection\n[green]•[/] Open source with full code transparency", false);

            MenuEngine.ShowError("TROUBLESHOOTING", "[red]•[/] Use 'status' command to check dependency availability\n[red]•[/] Some features may require udev rules or group membership\n[red]•[/] Check system logs for permission-related errors\n[red]•[/] Ensure NetworkManager is running for WiFi features\n[red]•[/] Ensure BlueZ/bluetooth service is running for Bluetooth features");
            MenuEngine.GeneralMessage("Press any key to continue...");
        }
    }
}
