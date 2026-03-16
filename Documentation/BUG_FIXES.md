# Bug Fixes Applied

## Issues Fixed

### 1. CLI Version Requires Desktop Environment (DE)

**Problem**: The CLI version required GUI dependencies to run, even in headless mode.

**Root Cause**: The main project included Avalonia GUI dependencies regardless of runtime mode.

**Solution**: 
- Created separate CLI-only project (`UtilitiesManagerCLI.csproj`)
- Excludes all Avalonia dependencies
- Targets .NET 10.0 for compatibility
- Uses shared source files from `src/CLI/` directory

**Files Modified**:
- `UtilitiesManagerCLI.csproj` (new)
- `Program_CLI.cs` (new)
- `UtilitiesManagerCLI.sln` (updated)
- `UtilitiesManager.csproj` (excluded CLI entry point)

**Usage**:
```bash
# Build CLI version
dotnet build UtilitiesManagerCLI.csproj

# Run CLI version
dotnet run --project UtilitiesManagerCLI.csproj help
```

### 2. Battery Button Shows When No Battery Present

**Problem**: Battery button was available even when no battery device was present in the system.

**Root Cause**: Detection only checked for `upower` command availability, not actual battery devices.

**Solution**: 
- Enhanced `CheckUpowerAvailable()` method in `Terminal.cs`
- Now verifies:
  1. `upower` command is available
  2. At least one battery device exists (`upower -e | grep battery`)
  3. Battery is present (`battery present: yes`)

**Files Modified**:
- `src/CLI/Terminal.cs` (added `CheckUpowerAvailable()` method)
- Updated `CheckDependenciesAsync()` to use new method

**Behavior**:
- Battery button only appears when actual battery hardware is detected
- Gracefully handles systems without batteries (desktops, servers)
- Maintains compatibility with UPS and other battery devices

## Testing

Both fixes have been tested and verified:

1. **CLI Version**: Successfully runs without GUI dependencies
2. **Battery Detection**: Accurately detects presence/absence of battery hardware

## Build Instructions

### GUI Version (with DE dependencies):
```bash
dotnet build UtilitiesManager.csproj
dotnet run --project UtilitiesManager.csproj
```

### CLI Version (headless):
```bash
dotnet build UtilitiesManagerCLI.csproj
dotnet run --project UtilitiesManagerCLI.csproj
```

The CLI version is ideal for:
- Servers without GUI
- Automation scripts
- Remote SSH sessions
- Container environments
