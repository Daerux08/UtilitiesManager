# UtilitiesManager - How It Works

## What this app is
A Linux utility manager that provides both GUI and CLI interfaces for controlling system settings and monitoring server resources.

## Architecture Overview
UtilitiesManager uses a dual-interface approach:
- **GUI Mode**: Avalonia desktop application for interactive use
- **CLI Mode**: Command-line interface with interactive menus for servers and automation

## Environment Detection
The application automatically detects the appropriate mode:
1. Checks for DISPLAY/WAYLAND_DISPLAY environment variables
2. Falls back to CLI mode in headless environments
3. Can force CLI mode with command-line arguments

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
8. `BluetoothWindow.axaml/.cs` - Bluetooth device management window
9. `EnterPasswordPopup.axaml/.cs` - Generic password/PIN dialog for WiFi and Bluetooth

### CLI Components
9. `CLI_UTILMAN.cs` - CLI interface routing and main menu coordination
10. `MenuEngine.cs` - Modern Spectre.Console based menu system with arrow key navigation
11. `DownloadScript.cs` - Package installation with sudo handling
12. **Service Classes** - Refactored modular service-based architecture:
    - `BrightnessService.cs` - Screen brightness control
    - `VolumeService.cs` - Audio volume management
    - `BatteryService.cs` - Battery status and power profiles
    - `WifiService.cs` - WiFi network management
    - `BluetoothService.cs` - Bluetooth device management
    - `PowerService.cs` - Power profile management
    - `StatusService.cs` - System status overview
    - `SystemMonitoringService.cs` - CPU, memory, disk, network monitoring
    - `ServicesService.cs` - System service management
    - `UserService.cs` - User account management
    - `LogService.cs` - System log viewing
    - `FirewallService.cs` - Firewall status and management
    - `PackageService.cs` - Package installation and management

## Documentation Structure
- `HOW_IT_WORKS.md` - This file (general overview)
- `HOW_IT_WORKS_GUI.md` - GUI-specific implementation details
- `HOW_IT_WORKS_CLI.md` - CLI-specific implementation details

## Shared Features
Both GUI and CLI interfaces share:
- **Command execution** via Terminal.cs
- **Dependency checking** with simplified array-based approach and clear boolean flags
- **Linux command parsing** with multiple fallbacks
- **Cross-distribution compatibility**
- **Service-based architecture** for maintainable code (CLI)
- **Enhanced GUI** with material design cards and modern styling
- **Modern CLI** with Spectre.Console based menus and reduced bloat

## What works now
1. Brightness control - via brightnessctl
2. Volume control - via pactl
3. Battery monitoring - via upower
4. Power profiles - via powerprofilesctl
5. Wi-Fi scanning - robust nmcli parsing with fallbacks
6. Wi-Fi connection - with password support and retry logic
7. Bluetooth device discovery - via bluetoothctl
8. Bluetooth pairing and connection - with PIN/passkey support
9. Cross-distro compatibility - works where NetworkManager available
10. Error handling - graceful degradation for missing tools
11. System monitoring - CPU, memory, disk, network statistics (CLI only)
12. Service management - start, stop, restart systemd services (CLI only)
13. User management - view user accounts and login status (CLI only)
14. Log management - system, kernel, and boot log viewing (CLI only)
15. Firewall management - UFW and iptables configuration (CLI only)
16. Package installation - automated dependency installation (CLI only)
17. Arrow key navigation - modern CLI menu interface with Spectre.Console (CLI only)
18. Environment detection - automatic GUI/CLI mode selection
19. **Enhanced Menu Engine** - streamlined CLI interface with reduced complexity

## Future Enhancements
1. ~~Bluetooth management~~ ✅ Added in v0.8.0
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
