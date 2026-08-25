# Bugs Fixed:

## Version 0.9.0-prealpha8
- **CLI Menu Consistency**: Refactored all CLI services to use `MenuEngine.cs` for consistent output
  - Fixed inconsistent menu formatting across services
  - Improved Spectre.Console integration for better visual presentation
- **AOT Compilation**: Debugged and optimized the AOT (Ahead-Of-Time) compilation process
  - Fixed compilation errors in StatusService.cs and VolumeService.cs
  - Resolved ThemeSettingsService.cs initialization issues for AOT compatibility
  - Optimized build-deb.sh for native binary generation
- **Performance**: Significantly improved startup time and runtime performance with AOT binaries
- **Compatibility**: Eliminated .NET runtime dependency for broader system compatibility

## Version 0.7.0-prealpha6
- **Enhanced CLI Interface**: Completely redesigned CLI using Spectre.Console for improved usability
- **Reduced CLI Bloat**: Streamlined menu system using simple, efficient engine
- **Improved Menu Navigation**: Modern arrow key navigation with enhanced visual feedback

## Version 0.6.0-prealpha6
- Fixed an issue where the `UtilMan wifi list` command would not display available networks correctly due to a parsing error in the `nmcli` output.
- Removed the dependency of the GUI for the CLI version
- reinforced the WiFi Parsing by making nmcli output send it in machine readable format
- fixed the detection for systemctl, 
- fixed and optimize the install packages script (DownloadScript.cs)
- Refactored CLI_UTILMAN.cs into 12 separate service classes