using Spectre.Console;
using System;

namespace UtilitiesManager
{
    public static class Help
    {
        public static void ShowAllHelp()
        {
            AnsiConsole.Clear();
            
            // Main title
            AnsiConsole.Write(new FigletText("UTILITIES MANAGER")
                .Centered()
                .Color(Color.Cyan1));
            
            AnsiConsole.MarkupLine("[bold yellow]CLI HELP[/]");
            AnsiConsole.WriteLine();
            
            // Description panel
            var descriptionPanel = new Panel("[italic]A Linux system utility manager for controlling brightness, volume, battery, WiFi, Bluetooth, and power profiles.[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Blue),
                Header = new PanelHeader("[bold]Description[/]")
            };
            AnsiConsole.Write(descriptionPanel);
            AnsiConsole.WriteLine();

            // Usage section
            AnsiConsole.MarkupLine("[bold green]USAGE:[/]");
            var usageTable = new Table()
                .BorderColor(Color.Grey)
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Command[/]", c => c.NoWrap())
                .AddColumn("[bold]Description[/]");
            
            usageTable.AddRow("[cyan]UtilitiesManager [[command]] [[options]][/]", "Run utilities manager");
            usageTable.AddRow("[cyan]UtilitiesManager --cli [[command]] [[options]][/]", "[yellow]Force CLI mode[/]");
            AnsiConsole.Write(usageTable);
            AnsiConsole.WriteLine();

            // Commands section
            AnsiConsole.MarkupLine("[bold underline cyan]COMMANDS[/]");
            AnsiConsole.WriteLine();

            ShowBrightnessHelp();
            ShowVolumeHelp();
            ShowBatteryHelp();
            ShowWiFiHelp();
            ShowBluetoothHelp();
            ShowPowerHelp();
            ShowStatusHelp();
            ShowGeneralHelp();
        }

        private static void ShowBrightnessHelp()
        {
            var brightnessPanel = new Panel(
                "[bold cyan]brightness <percentage>[/]     Set screen brightness (0-100)")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow),
                Header = new PanelHeader("[bold yellow]BRIGHTNESS[/]")
            };
            AnsiConsole.Write(brightnessPanel);
            AnsiConsole.WriteLine();

            // Examples section
            AnsiConsole.MarkupLine("[bold green]Examples:[/]");
            var examplesTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Result[/]");
            
            examplesTable.AddRow("[cyan]brightness 75[/]", "Set brightness to 75%");
            examplesTable.AddRow("[cyan]brightness 0[/]", "Turn off screen");
            examplesTable.AddRow("[cyan]brightness 100[/]", "Maximum brightness");
            AnsiConsole.Write(examplesTable);
            AnsiConsole.WriteLine();

            // Requirements
            var reqPanel = new Panel(
                "[red]•[/] brightnessctl package must be installed\n" +
                "[red]•[/] User must have permission to control backlight")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader("[bold]System Requirements[/]")
            };
            AnsiConsole.Write(reqPanel);
            AnsiConsole.WriteLine();

            // Linux commands
            AnsiConsole.MarkupLine("[bold blue]Linux Commands Used:[/]");
            var commandsTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Purpose[/]");
            
            commandsTable.AddRow("[cyan]brightnessctl get[/]", "Get current brightness value");
            commandsTable.AddRow("[cyan]brightnessctl max[/]", "Get maximum brightness value");
            commandsTable.AddRow("[cyan]brightnessctl set X%[/]", "Set brightness to X percent");
            AnsiConsole.Write(commandsTable);
            AnsiConsole.WriteLine();

            // Dependencies
            var depPanel = new Panel(
                "[yellow]•[/] brightnessctl (usually in brightnessctl package)\n" +
                "[yellow]•[/] Linux kernel backlight support")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Orange1),
                Header = new PanelHeader("[bold]Dependencies[/]")
            };
            AnsiConsole.Write(depPanel);
            AnsiConsole.WriteLine();

            // Notes
            var notesPanel = new Panel(
                "[italic]• Only works on systems with controllable backlight\n" +
                "• May require udev rules for user permissions\n" +
                "• Some laptops have multiple backlight devices[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey),
                Header = new PanelHeader("[bold]Notes[/]")
            };
            AnsiConsole.Write(notesPanel);
            AnsiConsole.WriteLine();
        }

        private static void ShowVolumeHelp()
        {
            var volumePanel = new Panel(
                "[bold cyan]volume <percentage>[/]         Set audio volume (0-100)")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow),
                Header = new PanelHeader("[bold yellow]VOLUME[/]")
            };
            AnsiConsole.Write(volumePanel);
            AnsiConsole.WriteLine();

            // Examples section
            AnsiConsole.MarkupLine("[bold green]Examples:[/]");
            var examplesTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Result[/]");
            
            examplesTable.AddRow("[cyan]volume 50[/]", "Set volume to 50%");
            examplesTable.AddRow("[cyan]volume 0[/]", "Mute audio");
            examplesTable.AddRow("[cyan]volume 100[/]", "Maximum volume");
            AnsiConsole.Write(examplesTable);
            AnsiConsole.WriteLine();

            // Requirements
            var reqPanel = new Panel(
                "[red]•[/] PulseAudio must be running\n" +
                "[red]•[/] pactl package must be installed\n" +
                "[red]•[/] User must be in audio group")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader("[bold]System Requirements[/]")
            };
            AnsiConsole.Write(reqPanel);
            AnsiConsole.WriteLine();

            // Linux commands
            AnsiConsole.MarkupLine("[bold blue]Linux Commands Used:[/]");
            var commandsTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Purpose[/]");
            
            commandsTable.AddRow("[cyan]pactl get-sink-volume @DEFAULT_SINK@[/]", "Get current volume");
            commandsTable.AddRow("[cyan]pactl set-sink-volume @DEFAULT_SINK@ X%[/]", "Set volume to X percent");
            AnsiConsole.Write(commandsTable);
            AnsiConsole.WriteLine();

            // Dependencies
            var depPanel = new Panel(
                "[yellow]•[/] pulseaudio (PulseAudio sound server)\n" +
                "[yellow]•[/] pactl (PulseAudio command-line tool)\n" +
                "[yellow]•[/] libpulse (PulseAudio libraries)")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Orange1),
                Header = new PanelHeader("[bold]Dependencies[/]")
            };
            AnsiConsole.Write(depPanel);
            AnsiConsole.WriteLine();

            // Notes
            var notesPanel = new Panel(
                "[italic]• Controls the default audio sink (output device)\n" +
                "• Works with most modern Linux desktop environments\n" +
                "• Volume is applied system-wide, not per-application[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey),
                Header = new PanelHeader("[bold]Notes[/]")
            };
            AnsiConsole.Write(notesPanel);
            AnsiConsole.WriteLine();
        }

        private static void ShowBatteryHelp()
        {
            var batteryPanel = new Panel(
                "[bold cyan]battery[/]                      Show detailed battery status")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow),
                Header = new PanelHeader("[bold yellow]BATTERY[/]")
            };
            AnsiConsole.Write(batteryPanel);
            AnsiConsole.WriteLine();

            // Examples section
            AnsiConsole.MarkupLine("[bold green]Examples:[/]");
            var examplesTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Result[/]");
            
            examplesTable.AddRow("[cyan]battery[/]", "Show current battery information");
            AnsiConsole.Write(examplesTable);
            AnsiConsole.WriteLine();

            // Information displayed
            var infoPanel = new Panel(
                "[blue]•[/] State: charging/discharging/fully-charged/unknown\n" +
                "[blue]•[/] Percentage: Current charge level\n" +
                "[blue]•[/] Time to Empty: Estimated remaining time\n" +
                "[blue]•[/] Time to Full: Time until fully charged\n" +
                "[blue]•[/] Energy Rate: Current power draw in watts\n" +
                "[blue]•[/] Present: Whether battery is detected")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Blue),
                Header = new PanelHeader("[bold]Information Displayed[/]")
            };
            AnsiConsole.Write(infoPanel);
            AnsiConsole.WriteLine();

            // Requirements
            var reqPanel = new Panel(
                "[red]•[/] upower package must be installed\n" +
                "[red]•[/] UPower service must be running")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader("[bold]System Requirements[/]")
            };
            AnsiConsole.Write(reqPanel);
            AnsiConsole.WriteLine();

            // Linux commands
            AnsiConsole.MarkupLine("[bold blue]Linux Commands Used:[/]");
            var commandsTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Purpose[/]");
            
            commandsTable.AddRow("[cyan]upower -e | grep battery[/]", "Find battery device path");
            commandsTable.AddRow("[cyan]upower -i {device}[/]", "Get battery information");
            AnsiConsole.Write(commandsTable);
            AnsiConsole.WriteLine();

            // Dependencies
            var depPanel = new Panel(
                "[yellow]•[/] upower (power management service)\n" +
                "[yellow]•[/] libupower (UPower libraries)\n" +
                "[yellow]•[/] Linux kernel power management")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Orange1),
                Header = new PanelHeader("[bold]Dependencies[/]")
            };
            AnsiConsole.Write(depPanel);
            AnsiConsole.WriteLine();

            // Notes
            var notesPanel = new Panel(
                "[italic]• Works with most laptop batteries\n" +
                "• UPS devices may also be detected\n" +
                "• Some systems report battery information differently[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey),
                Header = new PanelHeader("[bold]Notes[/]")
            };
            AnsiConsole.Write(notesPanel);
            AnsiConsole.WriteLine();
        }

        private static void ShowWiFiHelp()
        {
            var wifiPanel = new Panel(
                "[bold cyan]wifi list[/]                    List available WiFi networks")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow),
                Header = new PanelHeader("[bold yellow]WIFI[/]")
            };
            AnsiConsole.Write(wifiPanel);
            AnsiConsole.WriteLine();

            // Examples section
            AnsiConsole.MarkupLine("[bold green]Examples:[/]");
            var examplesTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Result[/]");
            
            examplesTable.AddRow("[cyan]wifi list[/]", "Show all available networks");
            AnsiConsole.Write(examplesTable);
            AnsiConsole.WriteLine();

            // Information displayed
            var infoPanel = new Panel(
                "[blue]•[/] SSID: Network name\n" +
                "[blue]•[/] Mode: Infrastructure/Ad-Hoc/AP\n" +
                "[blue]•[/] Chan: WiFi channel number\n" +
                "[blue]•[/] Rate: Connection speed (Mbit/s or Gbit/s)\n" +
                "[blue]•[/] Signal: Signal strength (dBm or percentage)\n" +
                "[blue]•[/] Security: Encryption type (WPA2, WEP, Open, etc.)\n" +
                "[blue]•[/] * marks currently connected network")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Blue),
                Header = new PanelHeader("[bold]Information Displayed[/]")
            };
            AnsiConsole.Write(infoPanel);
            AnsiConsole.WriteLine();

            // Requirements
            var reqPanel = new Panel(
                "[red]•[/] NetworkManager must be running\n" +
                "[red]•[/] nmcli package must be installed\n" +
                "[red]•[/] WiFi adapter must be enabled")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader("[bold]System Requirements[/]")
            };
            AnsiConsole.Write(reqPanel);
            AnsiConsole.WriteLine();

            // Linux commands
            AnsiConsole.MarkupLine("[bold blue]Linux Commands Used:[/]");
            var commandsTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Purpose[/]");
            
            commandsTable.AddRow("[cyan]nmcli device wifi rescan[/]", "Trigger fresh network scan");
            commandsTable.AddRow("[cyan]nmcli device wifi list[/]", "List available networks");
            commandsTable.AddRow("[cyan]nmcli device wifi connect[/]", "Connect to network");
            AnsiConsole.Write(commandsTable);
            AnsiConsole.WriteLine();

            // Dependencies
            var depPanel = new Panel(
                "[yellow]•[/] NetworkManager (network connection manager)\n" +
                "[yellow]•[/] nmcli (NetworkManager CLI tool)\n" +
                "[yellow]•[/] wpa_supplicant (WiFi authentication)\n" +
                "[yellow]•[/] Linux kernel WiFi drivers")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Orange1),
                Header = new PanelHeader("[bold]Dependencies[/]")
            };
            AnsiConsole.Write(depPanel);
            AnsiConsole.WriteLine();

            // Notes
            var notesPanel = new Panel(
                "[italic]• Only lists networks, doesn't connect (GUI feature)\n" +
                "• Uses multiple parsing strategies for compatibility\n" +
                "• Works across different Linux distributions\n" +
                "• May need sudo for some network operations[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey),
                Header = new PanelHeader("[bold]Notes[/]")
            };
            AnsiConsole.Write(notesPanel);
            AnsiConsole.WriteLine();
        }

        private static void ShowBluetoothHelp()
        {
            var bluetoothPanel = new Panel(
                "[bold cyan]bluetooth list[/]                List available Bluetooth devices\n" +
                "[bold cyan]bluetooth connect <device>[/]    Connect to Bluetooth device")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow),
                Header = new PanelHeader("[bold yellow]BLUETOOTH[/]")
            };
            AnsiConsole.Write(bluetoothPanel);
            AnsiConsole.WriteLine();

            // Examples section
            AnsiConsole.MarkupLine("[bold green]Examples:[/]");
            var examplesTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Result[/]");
            
            examplesTable.AddRow("[cyan]bluetooth list[/]", "Show all available devices");
            examplesTable.AddRow("[cyan]bluetooth connect 00:11:22:33:44:55[/]", "Connect to device with specified MAC address");
            AnsiConsole.Write(examplesTable);
            AnsiConsole.WriteLine();

            // Information displayed
            var infoPanel = new Panel(
                "[blue]•[/] Device: Device name\n" +
                "[blue]•[/] Address: Device MAC address\n" +
                "[blue]•[/] RSSI: Signal strength (dBm)\n" +
                "[blue]•[/] Paired: Whether device is paired\n" +
                "[blue]•[/] Connected: Whether device is connected")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Blue),
                Header = new PanelHeader("[bold]Information Displayed[/]")
            };
            AnsiConsole.Write(infoPanel);
            AnsiConsole.WriteLine();

            // Requirements
            var reqPanel = new Panel(
                "[red]•[/] bluez package must be installed\n" +
                "[red]•[/] bluetooth service must be running\n" +
                "[red]•[/] Bluetooth adapter must be enabled")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader("[bold]System Requirements[/]")
            };
            AnsiConsole.Write(reqPanel);
            AnsiConsole.WriteLine();

            // Linux commands
            AnsiConsole.MarkupLine("[bold blue]Linux Commands Used:[/]");
            var commandsTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Purpose[/]");
            
            commandsTable.AddRow("[cyan]bluetoothctl devices[/]", "List available devices");
            commandsTable.AddRow("[cyan]bluetoothctl connect <device>[/]", "Connect to device");
            AnsiConsole.Write(commandsTable);
            AnsiConsole.WriteLine();

            // Dependencies
            var depPanel = new Panel(
                "[yellow]•[/] bluez (Bluetooth daemon)\n" +
                "[yellow]•[/] bluetoothctl (Bluetooth CLI tool)\n" +
                "[yellow]•[/] Linux kernel Bluetooth drivers")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Orange1),
                Header = new PanelHeader("[bold]Dependencies[/]")
            };
            AnsiConsole.Write(depPanel);
            AnsiConsole.WriteLine();

            // Notes
            var notesPanel = new Panel(
                "[italic]• Only lists devices, doesn't connect (GUI feature)\n" +
                "• Uses multiple parsing strategies for compatibility\n" +
                "• Works across different Linux distributions\n" +
                "• May need sudo for some Bluetooth operations[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey),
                Header = new PanelHeader("[bold]Notes[/]")
            };
            AnsiConsole.Write(notesPanel);
            AnsiConsole.WriteLine();
        }

        private static void ShowPowerHelp()
        {
            var powerPanel = new Panel(
                "[bold cyan]power get[/]                    Get current power profile\n" +
                "[bold cyan]power set <profile>[/]          Set power profile")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow),
                Header = new PanelHeader("[bold yellow]POWER[/]")
            };
            AnsiConsole.Write(powerPanel);
            AnsiConsole.WriteLine();

            // Examples section
            AnsiConsole.MarkupLine("[bold green]Examples:[/]");
            var examplesTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Result[/]");
            
            examplesTable.AddRow("[cyan]power get[/]", "Show current power profile");
            examplesTable.AddRow("[cyan]power set performance[/]", "Set performance mode");
            examplesTable.AddRow("[cyan]power set balanced[/]", "Set balanced mode");
            examplesTable.AddRow("[cyan]power set power-saver[/]", "Set power-saver mode");
            AnsiConsole.Write(examplesTable);
            AnsiConsole.WriteLine();

            // Available profiles
            var profilesPanel = new Panel(
                "[green]•[/] [bold]performance[/]: Maximum performance, higher power usage\n" +
                "[green]•[/] [bold]balanced[/]: Balance between performance and power\n" +
                "[green]•[/] [bold]power-saver[/]: Minimum power usage, reduced performance")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Green),
                Header = new PanelHeader("[bold]Available Profiles[/]")
            };
            AnsiConsole.Write(profilesPanel);
            AnsiConsole.WriteLine();

            // Requirements
            var reqPanel = new Panel(
                "[red]•[/] powerprofilesctl package must be installed\n" +
                "[red]•[/] power-profiles-daemon service must be running\n" +
                "[red]•[/] System must support power profiling")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader("[bold]System Requirements[/]")
            };
            AnsiConsole.Write(reqPanel);
            AnsiConsole.WriteLine();

            // Linux commands
            AnsiConsole.MarkupLine("[bold blue]Linux Commands Used:[/]");
            var commandsTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Purpose[/]");
            
            commandsTable.AddRow("[cyan]powerprofilesctl get[/]", "Get current power profile");
            commandsTable.AddRow("[cyan]powerprofilesctl set X[/]", "Set power profile to X");
            AnsiConsole.Write(commandsTable);
            AnsiConsole.WriteLine();

            // Dependencies
            var depPanel = new Panel(
                "[yellow]•[/] power-profiles-daemon (power management daemon)\n" +
                "[yellow]•[/] powerprofilesctl (CLI tool)\n" +
                "[yellow]•[/] Linux kernel power management features")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Orange1),
                Header = new PanelHeader("[bold]Dependencies[/]")
            };
            AnsiConsole.Write(depPanel);
            AnsiConsole.WriteLine();

            // Notes
            var notesPanel = new Panel(
                "[italic]• Only available on systems with power profile support\n" +
                "• Modern laptops with Intel/AMD processors typically support this\n" +
                "• Profiles affect CPU frequency, GPU performance, etc.[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey),
                Header = new PanelHeader("[bold]Notes[/]")
            };
            AnsiConsole.Write(notesPanel);
            AnsiConsole.WriteLine();
        }

        private static void ShowStatusHelp()
        {
            var statusPanel = new Panel(
                "[bold cyan]status[/]                       Show all system status")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Yellow),
                Header = new PanelHeader("[bold yellow]STATUS[/]")
            };
            AnsiConsole.Write(statusPanel);
            AnsiConsole.WriteLine();

            // Examples section
            AnsiConsole.MarkupLine("[bold green]Examples:[/]");
            var examplesTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("[bold]Command[/]")
                .AddColumn("[bold]Result[/]");
            
            examplesTable.AddRow("[cyan]status[/]", "Show complete system overview");
            AnsiConsole.Write(examplesTable);
            AnsiConsole.WriteLine();

            // Information displayed
            var infoPanel = new Panel(
                "[blue]•[/] Battery: Current charge and state\n" +
                "[blue]•[/] Brightness: Current brightness level\n" +
                "[blue]•[/] Volume: Current audio volume\n" +
                "[blue]•[/] Power Profile: Current power management mode\n" +
                "[blue]•[/] Dependencies: Availability of required tools")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Blue),
                Header = new PanelHeader("[bold]Information Displayed[/]")
            };
            AnsiConsole.Write(infoPanel);
            AnsiConsole.WriteLine();

            // Requirements
            var reqPanel = new Panel(
                "[red]•[/] Any combination of supported dependencies\n" +
                "[red]•[/] Gracefully handles missing tools")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader("[bold]System Requirements[/]")
            };
            AnsiConsole.Write(reqPanel);
            AnsiConsole.WriteLine();

            // Dependencies checked
            var depPanel = new Panel(
                "[yellow]•[/] brightnessctl: For brightness control\n" +
                "[yellow]•[/] pactl: For audio volume control\n" +
                "[yellow]•[/] upower: For battery monitoring\n" +
                "[yellow]•[/] nmcli: For WiFi management\n" +
                "[yellow]•[/] bluetoothctl: For Bluetooth management\n" +
                "[yellow]•[/] powerprofilesctl: For power profile management")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Orange1),
                Header = new PanelHeader("[bold]Dependencies Checked[/]")
            };
            AnsiConsole.Write(depPanel);
            AnsiConsole.WriteLine();

            // Notes
            var notesPanel = new Panel(
                "[italic]• Shows \"Not available\" for missing dependencies\n" +
                "• Useful for troubleshooting and system validation\n" +
                "• Works even when some features are unavailable[/]")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey),
                Header = new PanelHeader("[bold]Notes[/]")
            };
            AnsiConsole.Write(notesPanel);
            AnsiConsole.WriteLine();
        }

        private static void ShowGeneralHelp()
        {
            // General Information header
            var generalHeader = new Rule("[bold yellow]GENERAL INFORMATION[/]");
            AnsiConsole.Write(generalHeader);
            AnsiConsole.WriteLine();

            // About section
            var aboutPanel = new Panel(
                "[italic]Utilities Manager is a Linux system utility manager for controlling brightness, volume, battery, WiFi, Bluetooth, and power profiles via command line.[/]")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Blue),
                Header = new PanelHeader("[bold]ABOUT[/]")
            };
            AnsiConsole.Write(aboutPanel);
            AnsiConsole.WriteLine();

            // Compatibility
            var compatPanel = new Panel(
                "[green]•[/] Tested on: Linux Mint 22.2, Debian 13, Ubuntu 24.04\n" +
                "[green]•[/] Requires: NetworkManager and standard Linux tools\n" +
                "[green]•[/] Architecture: Supports x86_64, ARM64")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Green),
                Header = new PanelHeader("[bold]COMPATIBILITY[/]")
            };
            AnsiConsole.Write(compatPanel);
            AnsiConsole.WriteLine();

            // Security
            var securityPanel = new Panel(
                "[green]•[/] No external dependencies beyond system packages\n" +
                "[green]•[/] Commands executed with current user permissions\n" +
                "[green]•[/] No network connections or data collection\n" +
                "[green]•[/] Open source with full code transparency")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Green),
                Header = new PanelHeader("[bold]SECURITY[/]")
            };
            AnsiConsole.Write(securityPanel);
            AnsiConsole.WriteLine();

            // Troubleshooting
            var troublePanel = new Panel(
                "[red]•[/] Use 'status' command to check dependency availability\n" +
                "[red]•[/] Some features may require udev rules or group membership\n" +
                "[red]•[/] Check system logs for permission-related errors\n" +
                "[red]•[/] Ensure NetworkManager is running for WiFi features\n" +
                "[red]•[/] Ensure BlueZ/bluetooth service is running for Bluetooth features")
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader("[bold]TROUBLESHOOTING[/]")
            };
            AnsiConsole.Write(troublePanel);
            AnsiConsole.WriteLine();
        }
    }
}
