# UtilitiesManager

A Linux utility manager with GUI and CLI interfaces for controlling system settings and monitoring.

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
sudo dpkg -i UtilitiesManager_0.5.0_amd64-PreAlpha.deb
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
- Bluetooth management
- Advanced system monitoring
- User account management
- Backup & recovery tools

## Known Bugs
  1. there are some CLI menus which are not with the arrowkey standard
