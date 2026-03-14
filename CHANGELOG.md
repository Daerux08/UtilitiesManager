# Changelog

## v0.4.0~alpha (2025-03-14)

### 🐛 Bug Fixes
- **GUI Launch Fixed** - Application now properly launches GUI when display environment is available
- **systemctl Detection Fixed** - Service management commands now work reliably by properly initializing dependency checks
- **Package Installation Fixed** - Install commands now work with correct package names

### 📦 Package Updates
- Updated package names to use correct Debian packages:
  - `pactl` → `pulseaudio-utils`
  - `nmcli` → `network-manager` 
  - `powerprofilesctl` → `power-profiles-daemon`
- All package operations now use `apt-get` instead of `apt`

### 🔧 Improvements
- Better environment detection for GUI/CLI mode selection
- Graceful fallback from GUI to CLI mode when GUI fails
- Improved error handling for missing dependencies
- Updated documentation to reflect current state

### 📚 Documentation
- Updated README.md with current package names and installation instructions
- Updated CLI documentation with fixes and new package names
- Added changelog to track changes
- Marked fixed bugs as resolved in README

## Previous Versions
- v0.4.0~alpha202603142001 - Initial working version with known issues
- v0.4.0~alpha202603142201 - Package name fixes applied
- v0.4.0~alpha202603142216 - systemctl detection fix applied  
- v0.4.0~alpha202603142220 - GUI launch fix applied
- v0.4.0~alpha202603142222 - Final stable version with all fixes
