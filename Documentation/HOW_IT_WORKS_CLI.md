# UtilitiesManager - CLI Implementation

## CLI Overview
The CLI interface provides comprehensive system control and monitoring capabilities with both direct commands and interactive menus.

## CLI Main Files
1. `Program.cs` - Entry point and environment detection
2. `CLI_UTILMAN.cs` - CLI interface and interactive menus
3. `MenuHelper.cs` - Arrow key navigation and menu system
4. `DownloadScript.cs` - Package installation with sudo handling
5. `Terminal.cs` - Linux command execution and parsing
6. `Help.cs` - Comprehensive help documentation

## How CLI Mode Starts
1. `Program.cs` checks environment variables (DISPLAY/WAYLAND_DISPLAY)
2. Falls back to CLI mode if GUI not available
3. `CLI_UTILMAN.cs` handles command-line arguments or interactive mode
4. `MenuHelper.cs` provides arrow key navigation with numbered fallback

## Interactive Menu System

### Navigation
- **Arrow Keys**: ↑↓ to navigate, ENTER to select, Q to quit
- **Fallback**: Numbered input when console redirected (SSH, scripts)
- **Visual**: `> Selected Item` vs `  Normal Item`

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
1. **Arrow Key Navigation** - Modern menu interface with fallback
2. **System Monitoring** - CPU, memory, disk, network statistics
3. **Service Management** - Full systemd service control
4. **User Management** - User account information display
5. **Log Management** - System, kernel, and boot log viewing
6. **Firewall Management** - UFW and iptables configuration
7. **Package Installation** - Automated dependency installation
8. **Environment Detection** - Automatic GUI/CLI mode selection
9. **Error Handling** - Graceful degradation for missing tools
10. **Cross-Distro Compatibility** - Works on major Linux distributions

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
