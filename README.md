# UtilitiesManager

A Linux utility manager with GUI and CLI interfaces for controlling system settings and monitoring.
A comprehensive Linux utility manager with GUI and CLI interfaces for controlling system settings and monitoring.

Tested on Linux Mint 22.2, Linux Debian 13, Ubuntu 24.04, and headless servers.

## Features

### Original Features (GUI & CLI)
1. **Brightness control**
2. **Volume control**
3. **Battery status**
4. **WiFi management**
5. **Power profiles**
6. **System Monitoring** - CPU, memory, disk, network stats
7. **Service Management** - Start/stop/restart services
8. **User Management** - View user accounts
9. **Log Management** - System logs viewing
10. **Firewall Management** - UFW/iptables configuration
11. **Package Installation** - Auto-install all dependencies
12. **Arrow Key Navigation** - Modern menu interface

<img width="513" height="554" alt="image" src="https://github.com/user-attachments/assets/78e5ab19-095e-4c40-b873-21cb1361b3da" />  --main menu
<img width="378" height="398" alt="image" src="https://github.com/user-attachments/assets/5f7f7de1-11ee-4f0c-9263-1701fba6f9f6" /> -- battery menu: Click on the buttons to select a power profile, click refresh to force another scan






## Usage

### Interactive Mode
```bash
UtilMan  # Arrow key navigation, Q to quit
```

### CLI Commands
```bash
# System control
UtilMan brightness 75
UtilMan volume 50
UtilMan battery
UtilMan wifi list
UtilMan power get
UtilMan power set performance

# Server monitoring  
UtilMan cpu
UtilMan memory
UtilMan disk
UtilMan network
UtilMan services list
UtilMan users
UtilMan logs
UtilMan firewall

# Package management
UtilMan install all
UtilMan install status
UtilMan install sensors
UtilMan install firewall

# Help
UtilMan help
UtilMan status
```

## Dependencies

### Core
- brightnessctl, pactl, upower, nmcli, powerprofilesctl

### Server Features
- procps, lm-sensors, sysstat, systemctl, journalctl, ufw, fail2ban, bleachbit, ncdu

## Installation
```bash
sudo dpkg -i UtilitiesManager_0.7.0-prealpha6_amd64.deb
sudo dpkg -i UtilitiesManager_0.9.0-beta_amd64.deb
sudo apt-get install -f
```

## Building
```bash
dotnet build
./build-deb.sh
```

## Documentation
- `HOW_IT_WORKS.md` - Technical implementation details
- `HOW_IT_WORKS_CLI.md` - CLI-specific features and commands
- `HOW_IT_WORKS_GUI.md` - GUI-specific features and components

## Future Updates
- Package negotiation system - Support for multiple alternative packages instead of hard requirements
- Enhanced download script with package selector - Interactive package selection during installation
- Advanced system monitoring
- User account management
- Backup & recovery tools

## Recent Updates
- **Enhanced CLI Interface**: Completely redesigned CLI using Spectre.Console with modern menu engine
- **Reduced CLI Bloat**: Streamlined menu system using simple, efficient engine
- **Improved Architecture**: Refactored codebase into modular service classes
- **Enhanced Dependency System**: Simplified and robust dependency checking
- **Better GUI**: Material design improvements with modern styling
- **Bug Fixes**: WiFi parsing, service detection, and startup reliability improvements
