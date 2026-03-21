# Changelog

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
