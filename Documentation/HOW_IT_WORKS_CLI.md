# UtilitiesManager - CLI Implementation

## CLI Overview
The CLI interface provides comprehensive system control and monitoring capabilities with both direct commands and interactive menus.

## CLI Main Files
1. `Program.cs` - Entry point and environment detection
2. `CLI_UTILMAN.cs` - CLI interface routing and main menu coordination
3. `MenuEngine.cs` - Modern Spectre.Console based menu system with arrow key navigation
4. `DownloadScript.cs` - Package installation with sudo handling
5. `Terminal.cs` - Linux command execution and parsing
6. `Help.cs` - Comprehensive help documentation

## CLI Service Architecture
The CLI has been refactored into a modular service-based architecture:

### Core Service Classes
- `BrightnessService.cs` - Screen brightness control
- `VolumeService.cs` - Audio volume management
- `BatteryService.cs` - Battery status and power profiles
- `WifiService.cs` - WiFi network management
- `PowerService.cs` - Power profile management
- `StatusService.cs` - System status overview
- `SystemMonitoringService.cs` - CPU, memory, disk, network monitoring
- `ServicesService.cs` - System service management
- `UserService.cs` - User account management
- `LogService.cs` - System log viewing
- `FirewallService.cs` - Firewall status and management
- `PackageService.cs` - Package installation and management

### Service Pattern
Each service follows a consistent pattern:
- **Static classes** for stateless operations
- **HandleXYZCommand()** methods for CLI command processing
- **XYZMenu()** methods for interactive menu interfaces
- **Dependency checking** and graceful error handling

## How CLI Mode Starts
1. `Program.cs` checks environment variables (DISPLAY/WAYLAND_DISPLAY)
2. Falls back to CLI mode if GUI not available
3. `CLI_UTILMAN.cs` routes commands to appropriate service classes
4. Service classes handle CLI commands and interactive menus
5. `MenuEngine.cs` provides modern Spectre.Console based navigation with arrow keys

## Interactive Menu System

### Navigation
- **Arrow Keys**: ↑↓ to navigate, ENTER to select, Q to quit
- **Spectre.Console**: Modern terminal UI with colors and styling
- **Fallback**: Numbered input when console redirected (SSH, scripts)
- **Visual**: Enhanced panels and formatted output

### Main Menu Structure
```
=== UTILITIES MANAGER - INTERACTIVE MODE ===
> Brightness Control
  Volume Control
  Battery Status
  WiFi Networks
  Power Profiles
  System Monitoring
  Service Management
  User Management
  Log Management
  Firewall Management
  Package Installation
  Help & Documentation
  Refresh Status
  Quit
```

### Sub-menus
- **System Monitoring**: CPU, Memory, Disk, Network options
- **Service Management**: List, Start, Stop, Restart, Enable, Disable
- **User Management**: View user accounts and login status
- **Log Management**: System, Kernel, Boot logs
- **Firewall Management**: UFW status, rule management
- **Package Installation**: All, Individual, Status, Sensors, Firewall

## CLI Commands

### System Control
```bash
UtilMan brightness [0-100]     # Set brightness percentage
UtilMan volume [0-100]         # Set volume percentage
UtilMan battery                # Show battery status
UtilMan wifi list              # List WiFi networks
UtilMan power get              # Get current power profile
UtilMan power set [profile]    # Set power profile (performance/balanced/power-saver)
```

### Server Monitoring
```bash
UtilMan cpu                    # CPU information, load, temperature
UtilMan memory                 # Memory and swap usage
UtilMan disk                   # Disk usage by filesystem
UtilMan network                # Network interface information
```

### Service Management
```bash
UtilMan services list          # List all services
UtilMan services start [name]  # Start a service
UtilMan services stop [name]   # Stop a service
UtilMan services restart [name] # Restart a service
UtilMan services enable [name]  # Enable service at boot
UtilMan services disable [name] # Disable service at boot
```

### User Management
```bash
UtilMan users                  # Show all user accounts
```

### Log Management
```bash
UtilMan logs                   # Recent system logs
UtilMan logs kernel            # Recent kernel logs
UtilMan logs boot              # Boot logs
```

### Firewall Management
```bash
UtilMan firewall               # Show firewall status
```

### Package Management
```bash
UtilMan install all            # Install all required packages
UtilMan install individual     # Install packages one by one
UtilMan install status         # Check package installation status
UtilMan install sensors        # Setup hardware sensors
UtilMan install firewall       # Configure firewall
```

### Help and Status
```bash
UtilMan help                   # Show detailed help
UtilMan status                 # Show system overview and dependencies
```

## CLI Linux Commands Used

### System Monitoring Commands
1. `ps aux` - Process information for CPU usage
2. `uptime` - System uptime and load average
3. `cat /proc/loadavg` - Detailed load average
4. `sensors` - Hardware temperature sensors
5. `free -h` - Memory usage in human-readable format
6. `df -h` - Disk usage in human-readable format
7. `ip addr show` - Network interface information

### Service Management Commands
1. `systemctl list-units --type=service` - List system services
2. `systemctl start {service}` - Start a service
3. `systemctl stop {service}` - Stop a service
4. `systemctl restart {service}` - Restart a service
5. `systemctl enable {service}` - Enable service at boot
6. `systemctl disable {service}` - Disable service at boot

### User Management Commands
1. `getent passwd` - User account information

### Log Management Commands
1. `journalctl --since "1 hour ago"` - Recent system logs
2. `journalctl -k --since "1 hour ago"` - Recent kernel logs
3. `journalctl -b` - Boot logs

### Firewall Commands
1. `ufw status verbose` - UFW firewall status
2. `iptables -L -n` - iptables rules fallback

### Package Management Commands
1. `which {command}` - Check command availability
2. `dpkg -l | grep {package}` - Check installed packages
3. `apt install {package}` - Package installation
4. `sensors-detect --auto` - Hardware sensor setup
5. `ufw --force enable` - Enable firewall
6. `ufw default deny incoming` - Set default firewall rules

## CLI Features Working
1. **Modular Service Architecture** - Separated concerns into focused service classes
2. **Enhanced Menu Engine** - Modern Spectre.Console based interface with reduced complexity
3. **Arrow Key Navigation** - Modern menu interface with fallback support
4. **System Monitoring** - CPU, memory, disk, network statistics
5. **Service Management** - Full systemd service control
6. **User Management** - User account information display
7. **Log Management** - System, kernel, and boot log viewing
8. **Firewall Management** - UFW and iptables configuration
9. **Package Installation** - Automated dependency installation
10. **Environment Detection** - Automatic GUI/CLI mode selection
11. **Error Handling** - Graceful degradation for missing tools
12. **Cross-Distro Compatibility** - Works on major Linux distributions
13. **Maintainable Code** - Each service has single responsibility
14. **Streamlined Interface** - Reduced CLI bloat with efficient menu engine

## CLI Dependencies

### Core Dependencies
- **brightnessctl** - Screen brightness control
- **pactl** (pulseaudio) - Audio volume control
- **upower** - Battery monitoring
- **nmcli** (NetworkManager) - WiFi management
- **powerprofilesctl** - Power profile management

### Server Dependencies
- **procps** (ps/free) - Basic system monitoring
- **lm-sensors** - Hardware temperature sensors
- **sysstat** - Detailed system statistics
- **systemctl** (systemd) - Service management
- **journalctl** - Log management
- **ufw** - Firewall management
- **fail2ban** - Intrusion prevention
- **bleachbit** - System cleanup
- **ncdu** - Disk usage analysis

### Optional Dependencies
- **iotop** - I/O monitoring
- **nethogs** - Network monitoring per process

## CLI Error Handling
- **Missing Dependencies** - Shows "Not available" and continues
- **Permission Errors** - Suggests sudo or group membership
- **Command Failures** - Shows error messages and continues
- **Input Redirection** - Falls back to numbered input
- **Authentication** - Handles sudo password prompts

## CLI Testing
- **Tested on**: Linux Mint 22.2, Debian 13, Ubuntu 24.04
- **Works in**: Headless servers, SSH sessions, scripted environments
- **Compatible with**: All major terminal emulators
