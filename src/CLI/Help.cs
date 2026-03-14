using System;

namespace UtilitiesManagerCLI
{
    public static class Help
    {
        public static void ShowAllHelp()
        {
            Console.WriteLine("=== UTILITIES MANAGER - CLI HELP ===");
            Console.WriteLine();
            Console.WriteLine("A Linux system utility manager for controlling brightness, volume, battery, WiFi, and power profiles.");
            Console.WriteLine();
            Console.WriteLine("USAGE:");
            Console.WriteLine("  UtilitiesManager [command] [options]");
            Console.WriteLine("  UtilitiesManager --cli [command] [options]  (Force CLI mode)");
            Console.WriteLine();
            Console.WriteLine("ENVIRONMENT DETECTION:");
            Console.WriteLine("  - Automatically detects headless environments (no DISPLAY/WAYLAND_DISPLAY)");
            Console.WriteLine("  - Falls back to CLI mode when GUI is unavailable");
            Console.WriteLine("  - Use --cli flag to force CLI mode even with GUI available");
            Console.WriteLine();
            Console.WriteLine("=== COMMANDS ===");
            Console.WriteLine();

            ShowBrightnessHelp();
            ShowVolumeHelp();
            ShowBatteryHelp();
            ShowWiFiHelp();
            ShowPowerHelp();
            ShowStatusHelp();
            ShowGeneralHelp();
        }

        private static void ShowBrightnessHelp()
        {
            Console.WriteLine("BRIGHTNESS:");
            Console.WriteLine("  brightness <percentage>     Set screen brightness (0-100)");
            Console.WriteLine();
            Console.WriteLine("  Examples:");
            Console.WriteLine("    brightness 75            Set brightness to 75%");
            Console.WriteLine("    brightness 0             Turn off screen");
            Console.WriteLine("    brightness 100           Maximum brightness");
            Console.WriteLine();
            Console.WriteLine("  System Requirements:");
            Console.WriteLine("    - brightnessctl package must be installed");
            Console.WriteLine("    - User must have permission to control backlight");
            Console.WriteLine();
            Console.WriteLine("  Linux Commands Used:");
            Console.WriteLine("    - brightnessctl get       Get current brightness value");
            Console.WriteLine("    - brightnessctl max       Get maximum brightness value");
            Console.WriteLine("    - brightnessctl set X%    Set brightness to X percent");
            Console.WriteLine();
            Console.WriteLine("  Dependencies:");
            Console.WriteLine("    - brightnessctl (usually in brightnessctl package)");
            Console.WriteLine("    - Linux kernel backlight support");
            Console.WriteLine();
            Console.WriteLine("  Notes:");
            Console.WriteLine("    - Only works on systems with controllable backlight");
            Console.WriteLine("    - May require udev rules for user permissions");
            Console.WriteLine("    - Some laptops have multiple backlight devices");
            Console.WriteLine();
        }

        private static void ShowVolumeHelp()
        {
            Console.WriteLine("VOLUME:");
            Console.WriteLine("  volume <percentage>         Set audio volume (0-100)");
            Console.WriteLine();
            Console.WriteLine("  Examples:");
            Console.WriteLine("    volume 50                 Set volume to 50%");
            Console.WriteLine("    volume 0                  Mute audio");
            Console.WriteLine("    volume 100                Maximum volume");
            Console.WriteLine();
            Console.WriteLine("  System Requirements:");
            Console.WriteLine("    - PulseAudio must be running");
            Console.WriteLine("    - pactl package must be installed");
            Console.WriteLine("    - User must be in audio group");
            Console.WriteLine();
            Console.WriteLine("  Linux Commands Used:");
            Console.WriteLine("    - pactl get-sink-volume @DEFAULT_SINK@    Get current volume");
            Console.WriteLine("    - pactl set-sink-volume @DEFAULT_SINK@ X%  Set volume to X percent");
            Console.WriteLine();
            Console.WriteLine("  Dependencies:");
            Console.WriteLine("    - pulseaudio (PulseAudio sound server)");
            Console.WriteLine("    - pactl (PulseAudio command-line tool)");
            Console.WriteLine("    - libpulse (PulseAudio libraries)");
            Console.WriteLine();
            Console.WriteLine("  Notes:");
            Console.WriteLine("    - Controls the default audio sink (output device)");
            Console.WriteLine("    - Works with most modern Linux desktop environments");
            Console.WriteLine("    - Volume is applied system-wide, not per-application");
            Console.WriteLine();
        }

        private static void ShowBatteryHelp()
        {
            Console.WriteLine("BATTERY:");
            Console.WriteLine("  battery                      Show detailed battery status");
            Console.WriteLine();
            Console.WriteLine("  Examples:");
            Console.WriteLine("    battery                    Show current battery information");
            Console.WriteLine();
            Console.WriteLine("  Information Displayed:");
            Console.WriteLine("    - State: charging/discharging/fully-charged/unknown");
            Console.WriteLine("    - Percentage: Current charge level");
            Console.WriteLine("    - Time to Empty: Estimated remaining time");
            Console.WriteLine("    - Time to Full: Time until fully charged");
            Console.WriteLine("    - Energy Rate: Current power draw in watts");
            Console.WriteLine("    - Present: Whether battery is detected");
            Console.WriteLine();
            Console.WriteLine("  System Requirements:");
            Console.WriteLine("    - upower package must be installed");
            Console.WriteLine("    - UPower service must be running");
            Console.WriteLine();
            Console.WriteLine("  Linux Commands Used:");
            Console.WriteLine("    - upower -e | grep battery    Find battery device path");
            Console.WriteLine("    - upower -i {device}          Get battery information");
            Console.WriteLine();
            Console.WriteLine("  Dependencies:");
            Console.WriteLine("    - upower (power management service)");
            Console.WriteLine("    - libupower (UPower libraries)");
            Console.WriteLine("    - Linux kernel power management");
            Console.WriteLine();
            Console.WriteLine("  Notes:");
            Console.WriteLine("    - Works with most laptop batteries");
            Console.WriteLine("    - UPS devices may also be detected");
            Console.WriteLine("    - Some systems report battery information differently");
            Console.WriteLine();
        }

        private static void ShowWiFiHelp()
        {
            Console.WriteLine("WIFI:");
            Console.WriteLine("  wifi list                    List available WiFi networks");
            Console.WriteLine();
            Console.WriteLine("  Examples:");
            Console.WriteLine("    wifi list                  Show all available networks");
            Console.WriteLine();
            Console.WriteLine("  Information Displayed:");
            Console.WriteLine("    - SSID: Network name");
            Console.WriteLine("    - Mode: Infrastructure/Ad-Hoc/AP");
            Console.WriteLine("    - Chan: WiFi channel number");
            Console.WriteLine("    - Rate: Connection speed (Mbit/s or Gbit/s)");
            Console.WriteLine("    - Signal: Signal strength (dBm or percentage)");
            Console.WriteLine("    - Security: Encryption type (WPA2, WEP, Open, etc.)");
            Console.WriteLine("    - * marks currently connected network");
            Console.WriteLine();
            Console.WriteLine("  System Requirements:");
            Console.WriteLine("    - NetworkManager must be running");
            Console.WriteLine("    - nmcli package must be installed");
            Console.WriteLine("    - WiFi adapter must be enabled");
            Console.WriteLine();
            Console.WriteLine("  Linux Commands Used:");
            Console.WriteLine("    - nmcli device wifi rescan     Trigger fresh network scan");
            Console.WriteLine("    - nmcli device wifi list       List available networks");
            Console.WriteLine("    - nmcli device wifi connect   Connect to network");
            Console.WriteLine();
            Console.WriteLine("  Dependencies:");
            Console.WriteLine("    - NetworkManager (network connection manager)");
            Console.WriteLine("    - nmcli (NetworkManager CLI tool)");
            Console.WriteLine("    - wpa_supplicant (WiFi authentication)");
            Console.WriteLine("    - Linux kernel WiFi drivers");
            Console.WriteLine();
            Console.WriteLine("  Notes:");
            Console.WriteLine("    - Only lists networks, doesn't connect (GUI feature)");
            Console.WriteLine("    - Uses multiple parsing strategies for compatibility");
            Console.WriteLine("    - Works across different Linux distributions");
            Console.WriteLine("    - May need sudo for some network operations");
            Console.WriteLine();
        }

        private static void ShowPowerHelp()
        {
            Console.WriteLine("POWER:");
            Console.WriteLine("  power get                    Get current power profile");
            Console.WriteLine("  power set <profile>          Set power profile");
            Console.WriteLine();
            Console.WriteLine("  Examples:");
            Console.WriteLine("    power get                  Show current power profile");
            Console.WriteLine("    power set performance      Set performance mode");
            Console.WriteLine("    power set balanced         Set balanced mode");
            Console.WriteLine("    power set power-saver      Set power-saver mode");
            Console.WriteLine();
            Console.WriteLine("  Available Profiles:");
            Console.WriteLine("    - performance: Maximum performance, higher power usage");
            Console.WriteLine("    - balanced: Balance between performance and power");
            Console.WriteLine("    - power-saver: Minimum power usage, reduced performance");
            Console.WriteLine();
            Console.WriteLine("  System Requirements:");
            Console.WriteLine("    - powerprofilesctl package must be installed");
            Console.WriteLine("    - power-profiles-daemon service must be running");
            Console.WriteLine("    - System must support power profiling");
            Console.WriteLine();
            Console.WriteLine("  Linux Commands Used:");
            Console.WriteLine("    - powerprofilesctl get      Get current power profile");
            Console.WriteLine("    - powerprofilesctl set X   Set power profile to X");
            Console.WriteLine();
            Console.WriteLine("  Dependencies:");
            Console.WriteLine("    - power-profiles-daemon (power management daemon)");
            Console.WriteLine("    - powerprofilesctl (CLI tool)");
            Console.WriteLine("    - Linux kernel power management features");
            Console.WriteLine();
            Console.WriteLine("  Notes:");
            Console.WriteLine("    - Only available on systems with power profile support");
            Console.WriteLine("    - Modern laptops with Intel/AMD processors typically support this");
            Console.WriteLine("    - Profiles affect CPU frequency, GPU performance, etc.");
            Console.WriteLine();
        }

        private static void ShowStatusHelp()
        {
            Console.WriteLine("STATUS:");
            Console.WriteLine("  status                       Show all system status");
            Console.WriteLine();
            Console.WriteLine("  Examples:");
            Console.WriteLine("    status                     Show complete system overview");
            Console.WriteLine();
            Console.WriteLine("  Information Displayed:");
            Console.WriteLine("    - Battery: Current charge and state");
            Console.WriteLine("    - Brightness: Current brightness level");
            Console.WriteLine("    - Volume: Current audio volume");
            Console.WriteLine("    - Power Profile: Current power management mode");
            Console.WriteLine("    - Dependencies: Availability of required tools");
            Console.WriteLine();
            Console.WriteLine("  System Requirements:");
            Console.WriteLine("    - Any combination of supported dependencies");
            Console.WriteLine("    - Gracefully handles missing tools");
            Console.WriteLine();
            Console.WriteLine("  Dependencies Checked:");
            Console.WriteLine("    - brightnessctl: For brightness control");
            Console.WriteLine("    - pactl: For audio volume control");
            Console.WriteLine("    - upower: For battery monitoring");
            Console.WriteLine("    - nmcli: For WiFi management");
            Console.WriteLine("    - powerprofilesctl: For power profile management");
            Console.WriteLine();
            Console.WriteLine("  Notes:");
            Console.WriteLine("    - Shows \"Not available\" for missing dependencies");
            Console.WriteLine("    - Useful for troubleshooting and system validation");
            Console.WriteLine("    - Works even when some features are unavailable");
            Console.WriteLine();
        }

        private static void ShowGeneralHelp()
        {
            Console.WriteLine("=== GENERAL INFORMATION ===");
            Console.WriteLine();
            Console.WriteLine("ABOUT:");
            Console.WriteLine("  Utilities Manager is a Linux system utility manager that provides");
            Console.WriteLine("  both GUI and CLI interfaces for controlling common system settings.");
            Console.WriteLine();
            Console.WriteLine("  Originally designed as a GUI application, it now supports");
            Console.WriteLine("  headless CLI operation for servers and automation.");
            Console.WriteLine();
            Console.WriteLine("COMPATIBILITY:");
            Console.WriteLine("  - Tested on: Linux Mint 22.2, Debian 13, Ubuntu 24.04");
            Console.WriteLine("  - Requires: NetworkManager and standard Linux tools");
            Console.WriteLine("  - Architecture: Supports x86_64, ARM64 (with appropriate .NET runtime)");
            Console.WriteLine();
            Console.WriteLine("BUILDING:");
            Console.WriteLine("  - Development: dotnet build");
            Console.WriteLine("  - Release: dotnet publish -c Release -r linux-x64 --self-contained");
            Console.WriteLine("  - Debian package: ./build-deb.sh");
            Console.WriteLine("  - Includes: .NET 8.0 runtime (no separate install needed)");
            Console.WriteLine();
            Console.WriteLine("TECHNICAL DETAILS:");
            Console.WriteLine("  - Framework: .NET 10.0");
            Console.WriteLine("  - CLI Library: System.CommandLine");
            Console.WriteLine("  - Architecture: MVVM pattern with shared backend logic");
            Console.WriteLine("  - Error Handling: Graceful degradation for missing dependencies");
            Console.WriteLine();
            Console.WriteLine("SECURITY:");
            Console.WriteLine("  - No external dependencies beyond system packages");
            Console.WriteLine("  - Commands executed with current user permissions");
            Console.WriteLine("  - No network connections or data collection");
            Console.WriteLine("  - Open source with full code transparency");
            Console.WriteLine();
            Console.WriteLine("TROUBLESHOOTING:");
            Console.WriteLine("  - Use 'status' command to check dependency availability");
            Console.WriteLine("  - Some features may require udev rules or group membership");
            Console.WriteLine("  - Check system logs for permission-related errors");
            Console.WriteLine("  - Ensure NetworkManager is running for WiFi features");
            Console.WriteLine();
            Console.WriteLine("FUTURE FEATURES:");
            Console.WriteLine("  - Bluetooth management");
            Console.WriteLine("  - User account management");
            Console.WriteLine("  - Package manager integration");
            Console.WriteLine("  - System monitoring (CPU, memory, disk)");
            Console.WriteLine("  - Network statistics and monitoring");
            Console.WriteLine();
        }
    }
}
