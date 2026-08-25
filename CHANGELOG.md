# Changelog

## [0.9.0-prealpha8] - 2026-08-25

### Major Features
- **Ahead-Of-Time (AOT) Compilation**: Application now compiles to native binaries
  - Optimized the application down to 130-150MB of RAM and 1-1.5% of CPU
  - Eliminates dependency on .NET runtime installation
  - Significantly improved performance and startup time
  - Reduced application footprint and resource usage
  - Compatible with systems without C# .NET runtime

### CLI Improvements
- **MenuEngine Integration**: Refactored all CLI services to use `MenuEngine.cs` methods
  - Replaced raw `Console.WriteLine` calls with consistent menu rendering
  - Improved visual consistency across all CLI services
  - Better support for Spectre.Console formatting
  - Enhanced interactive menu experience

### Affected Services
- BluetoothService.cs
- BrightnessService.cs
- FirewallService.cs
- LogService.cs
- MenuEngine.cs
- PowerService.cs
- ServicesService.cs
- StatusService.cs
- SystemMonitoringService.cs
- UserService.cs
- VolumeService.cs

### Technical Improvements
- **Project Configuration**: Updated project files for AOT compilation
  - UtilitiesManager.csproj: Added PublishAot property
  - UtilitiesManagerCLI.csproj: Added PublishAot property
  - Optimized build-deb.sh for AOT native binary generation
- **Build Process**: Optimized Debian package creation for native binaries
- **ThemeSettingsService.cs**: Improved theme settings initialization and handling

## [0.8.0-prealpha7] - 2026-04-21

### New Features
- **Bluetooth Management**: Complete Bluetooth support added to both GUI and CLI
  - GUI: New Bluetooth window with device scanning, pairing, connection, and trust management
  - CLI: Full `UtilMan bluetooth` command suite with interactive menu
  - Device discovery and status monitoring via bluetoothctl
  - PIN/passkey support for secure device pairing

### GUI Improvements
- **New Bluetooth Window**: Material design interface showing device list with connection status
- **Generic Password Popup**: Refactored EnterPasswordPopup for reuse with WiFi and Bluetooth
- **Main Window Integration**: Bluetooth button added with dependency-based availability

### CLI Improvements  
- **Bluetooth Commands**: `list`, `scan`, `connect`, `disconnect`, `pair`, `power`
- **Interactive Bluetooth Menu**: Arrow-key navigation for device selection and management
- **Help Documentation**: Complete Bluetooth help section with examples and requirements

### Technical Improvements
- **Terminal.cs**: Added BluetoothInfo model and bluetoothctl integration methods
- **MVVM Pattern**: BluetoothWindowViewModel following established architecture
- **Service Architecture**: BluetoothService.cs for CLI operations

## [0.7.0-prealpha6] - 2026-03-30
- **Remade the CLI interface**: Completely redesigned the CLI interface using Spectre.Console for improved usability and aesthetics
- **Reduced the overall size and bloat of the CLI menu, currently using a simple engine**

## [0.6.0-prealpha6] - 2026-03-21

### Major Changes
- **Architecture Refactor**: Completely restructured codebase from monolithic design to modular service classes
- **Code Quality**: Removed unnecessary dependencies between GUI and CLI components

### New Features
- **Enhanced GUI**: Material design improvements with modern card layouts, better spacing, and refined typography
- **Improved Dependency System**: Simplified and robust dependency checking using array-based approach with clear boolean flags

### Bug Fixes
- **WiFi Management**: Fixed an issue where the `UtilMan wifi list` command would not display available networks correctly due to a parsing error in the `nmcli` output
- **WiFi Parsing**: Reinforced the WiFi parsing by making nmcli output send it in machine readable format
- **Service Detection**: Fixed the detection for systemctl and other service detection reliability
- **Startup Issues**: Resolved CLI startup and environment detection problems
- **Package Installation**: Fixed and optimized the install packages script (DownloadScript.cs)
- **Code Architecture**: Refactored CLI_UTILMAN.cs into 12 separate service classes

### Technical Improvements
- **Maintainability**: Extracted 12 service classes from monolithic CLI_UTILMAN.cs
- **Documentation**: Added comprehensive project documentation and governance files

### Package Updates
- Updated version to 0.6.0
- Updated installation documentation

## [0.5.0-prealpha5] - Previous Release
- Terminal-only version implementation
- GUI WiFi window refactoring
- Bug fixes and polish

---

**Note**: This release includes major architectural improvements and enhanced stability.
