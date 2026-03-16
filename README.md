# UtilitiesManager

A Linux utility manager with GUI and CLI interfaces for controlling system settings and monitoring.

Tested on Linux Mint 22.2, Linux Debian 13, Ubuntu 24.04, and headless servers.

## Features

### Original Features (GUI & CLI)
1. Brightness control
2. Volume control  
3. Battery status
4. WiFi management
5. Power profiles

### New CLI Features
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
- brightnessctl, pulseaudio-utils, upower, network-manager, power-profiles-daemon

### Server Features
- procps, lm-sensors, sysstat, systemctl, journalctl, ufw, fail2ban, bleachbit, ncdu


## Installation
```bash
# Install the .deb package
sudo dpkg -i utilitiesmanager_0.4.0~alpha*.deb

# If there are dependency issues, run:
sudo apt-get install -f

# Or install dependencies manually:
utilitiesmanager install all
```

## Building
```bash
# Install .NET SDK 10.0
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0
export PATH="$PATH:$HOME/.dotnet"

# Build and package
dotnet build
./build-deb.sh

# Install the resulting package
sudo dpkg -i utilitiesmanager_*.deb
```

## Documentation
- `HOW_IT_WORKS.md` - Technical implementation details
- `HOW_IT_WORKS_CLI.md` - CLI-specific features and commands
- `HOW_IT_WORKS_GUI.md` - GUI-specific features and components

## Testers
- If you wish to become a tester, you can! Just write an issue (or email me), list your DE(GNOME, XFCE, Headless, etc), your Linux distro (Debian, Mint, Manjaro, etc), and I'll walk you through testing the app!

## Future Updates
- Bluetooth management
- Advanced system monitoring
- User account management
- Backup & recovery tools

## Known Bugs
  1. there are some CLI menus which are not with the arrowkey standart
  2. Not exactly a bug, but CLI_UTILMAN.cs is a Monolithic Class (to be refactored later)

## Recent Changes:
1. 14-03:
A. Fixed package names in installation script
B. Fixed systemctl detection
C. Fixed GUI launch
D. Fixed install command
