# UtilitiesManager

A Linux utility manager with both a GUI and CLI interface for controlling system settings and monitoring.

Tested on Linux Mint 22.2, Debian 13, Ubuntu 24.04, and headless servers.

<img width="513" height="554" alt="Main menu" src="https://github.com/user-attachments/assets/78e5ab19-095e-4c40-b873-21cb1361b3da" />

*Main menu*

<img width="378" height="398" alt="Battery menu" src="https://github.com/user-attachments/assets/5f7f7de1-11ee-4f0c-9263-1701fba6f9f6" />

*Battery menu — click buttons to select a power profile, click Refresh to rescan*

---

## Features

### GUI & CLI
- Brightness control
- Volume control
- Battery status & power profiles
- WiFi management

### CLI Only
- System monitoring (CPU, memory, disk, network)
- Service management (start/stop/restart)
- User account listing
- System log viewing
- Firewall management (UFW/iptables)
- Auto-install dependencies
- Arrow key navigation

---

## Installation

Download the latest `.deb` from [Releases](../../releases), then:

```bash
sudo dpkg -i UtilitiesManager_0.9.0-pre-alpha-8_amd64.deb
sudo apt-get install -f
```

### Requirements

**No .NET runtime required** — Compiled as native binary with AOT compilation

### Dependencies

**Core:** `brightnessctl` `pactl` `upower` `nmcli` `powerprofilesctl`

**Server features:** `procps` `lm-sensors` `sysstat` `systemctl` `journalctl` `ufw` `fail2ban` `bleachbit` `ncdu`

---

## Usage

### GUI
Launch from your application menu or run:
```bash
UtilMan
```

### CLI — Interactive Mode
```bash
UtilMan  # arrow key navigation, Q to quit
```

### CLI — Direct Commands
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

---

## Building

```bash
dotnet build
./build-deb.sh
```

## Documentation

- `HOW_IT_WORKS.md` — Technical implementation details
- `HOW_IT_WORKS_CLI.md` — CLI features and commands
- `HOW_IT_WORKS_GUI.md` — GUI features and components

---

## Recent Changes

- **v0.9.0**: Ahead-Of-Time (AOT) compilation for native binary execution
  - No .NET runtime required
  - Significantly faster startup and runtime performance
  - Optimized Debian package generation
  - Refactored all CLI services to use MenuEngine for consistent output
- Redesigned CLI using Spectre.Console with modern menu engine
- Refactored codebase into modular service classes
- Simplified dependency checking
- Material design GUI improvements
- Bug fixes: WiFi parsing, service detection, startup reliability

## Planned

- Package negotiation system (support for alternative packages)
- Interactive package selector during installation
- Advanced system monitoring
- User account management
- Theme selector
- RAM optimization
- Backup & recovery tools
