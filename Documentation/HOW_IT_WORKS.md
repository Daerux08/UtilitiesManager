# UtilitiesManager - How It Works

## What this app is
A Linux utility manager that provides both GUI and CLI interfaces for controlling system settings and monitoring server resources.

## Recent Updates (v0.4.0~alpha)
- **Fixed GUI Launch** - Application now properly launches GUI when display is available
- **Fixed systemctl Detection** - Service management commands now work reliably
- **Updated Package Names** - Using correct Debian package names for dependencies
- **Improved Package Installation** - All operations now use `apt-get` instead of `apt`
- **Better Error Handling** - Graceful fallback from GUI to CLI mode when needed

## Architecture Overview
UtilitiesManager uses a dual-interface approach:
- **GUI Mode**: Avalonia desktop application for interactive use
- **CLI Mode**: Command-line interface with interactive menus for servers and automation

## Environment Detection
The application automatically detects the appropriate mode:
1. Checks for command-line arguments (CLI mode forced)
2. Checks UTILITIES_MANAGER_CLI environment variable
3. Checks for DISPLAY environment variable
4. Falls back to CLI mode in headless environments
5. Attempts GUI launch with fallback to CLI on failure

## Main Components

### Core Files
1. `Program.cs` - Application entry point and environment detection
2. `Terminal.cs` - Linux command execution and parsing logic
3. `Help.cs` - Comprehensive help documentation

### GUI Components
4. `App.axaml/.cs` - Avalonia application setup
5. `MainWindow.axaml/.cs` - Main GUI window
6. `BatteryWindow.axaml/.cs` - Battery status window
7. `WiFiWindow.axaml/.cs` - WiFi management window
8. `EnterPasswordPopup.axaml/.cs` - WiFi password dialog

### CLI Components
9. `CLI_UTILMAN.cs` - CLI interface and interactive menus
10. `MenuHelper.cs` - Arrow key navigation and menu system
11. `DownloadScript.cs` - Package installation with sudo handling

## Documentation Structure
- `HOW_IT_WORKS.md` - This file (general overview)
- `HOW_IT_WORKS_GUI.md` - GUI-specific implementation details
- `HOW_IT_WORKS_CLI.md` - CLI-specific implementation details

## Shared Features
Both GUI and CLI interfaces share:
- **Command execution** via Terminal.cs
- **Dependency checking** and error handling
- **Linux command parsing** with multiple fallbacks
- **Cross-distribution compatibility**

## What works now
1. Brightness control - via brightnessctl
2. Volume control - via pactl
3. Battery monitoring - via upower
4. Power profiles - via powerprofilesctl
5. Wi-Fi scanning - robust nmcli parsing with fallbacks
6. Wi-Fi connection - with password support and retry logic
7. Cross-distro compatibility - works where NetworkManager available
8. Error handling - graceful degradation for missing tools
9. System monitoring - CPU, memory, disk, network statistics (CLI only)
10. Service management - start, stop, restart systemd services (CLI only)
11. User management - view user accounts and login status (CLI only)
12. Log management - system, kernel, and boot log viewing (CLI only)
13. Firewall management - UFW and iptables configuration (CLI only)
14. Package installation - automated dependency installation (CLI only)
15. Arrow key navigation - modern CLI menu interface (CLI only)
16. Environment detection - automatic GUI/CLI mode selection

## Future Enhancements
1. Bluetooth management
2. Advanced user account management (create/modify/delete)
3. Package manager GUI
4. Real-time system monitoring with graphs
5. Network statistics and monitoring
6. Custom themes/skins
7. Backup & recovery tools
8. Remote access management

## Testing
1. Tested on: Linux Mint 22.2, Debian 13, Ubuntu 24.04
2. Dependencies: Requires NetworkManager and standard Linux tools
3. Fallbacks: Graceful handling of missing commands
4. Modes: Works in both GUI and CLI environments

## Building
1. Development: `dotnet build`
2. Release: `dotnet publish -c Release -r linux-x64 --self-contained`
3. Debian package: `./build-deb.sh`
4. Includes: .NET runtime (no separate install needed)
