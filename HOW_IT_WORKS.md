# UtilitiesManager - How It Works

## What This App Is
A simple Linux utility app that acts as a Control Panel for common system settings

## Main Files
- `MainWindow.axaml` - Main window with sliders and buttons
- `MainWindow.axaml.cs` - Code behind for main window
- `Terminal.cs` - All Linux command execution and parsing logic
- `BatteryWindow.axaml/.cs` - Battery status window
- `WiFiWindow.axaml/.cs` - Wi-Fi networks and connection management
- `EnterPasswordPopup.axaml/.cs` - WiFi password input dialog

## How It Starts
1. `Program.cs` starts Avalonia app
2. `App.axaml.cs` creates main window
3. `MainWindow` constructor calls `InitializeValues()`
4. `InitializeValues()` runs `CheckDependencyCommand.LoadOriginalValuesAsync()`
5. This checks what Linux commands are available and gets current values

## Main Window Features
- Only enabled if `brightnessctl` command is found

### Wi-Fi Button
- Opens `WiFiWindow` when clicked
- Only enabled if `nmcli` command is found
- Shows current connection status in tooltip

### Battery Button
- Opens `BatteryWindow` when clicked
- Only enabled if `upower` command is found
- Shows current battery percentage in tooltip

## Terminal.cs - The Linux Commands

### Data Classes
```csharp
BatteryInfo {
    State: "charging" | "discharging" | "fully-charged" | etc.
    Percentage: 0-100
    TimeToEmpty: "2h 30m"  
    TimeToFull: "1h 15m"
    EnergyRate: Watts
    IsPresent: true/false
}

WiFiInfo {
    SSID: network name
    Mode: "Infra" | "Adhoc"
    Chan: channel number
    Rate: speed in Mb/s
    Signal: signal strength (numeric or bars)
    Security: "WPA2" | "WPA1 WPA2" | "Open" | etc.
    IsActive: currently connected?
}

CommandResult {
    ExitCode: process exit code
    Output: stdout content
    Error: stderr content
    IsSuccess: ExitCode == 0
    CombinedOutput: Output + Error
}
```

### Command Classes

### Battery Window

### What It Shows
- Charge percentage
- Battery state (charging/discharging/fully-charged)
- Time remaining or time to full
- Power draw in watts
- Current power profile with buttons to change

### How It Works
1. Window opens → calls `RefreshBatteryDataAsync()`
2. Checks if `upower` is available
3. Runs `upower -e | grep battery` to find battery device
4. Runs `upower -i {device}` to get battery info
5. Parses output for battery stats
6. Updates all TextBlocks and power profile buttons

### Power Profile Buttons
- Three buttons: Power Saver, Balanced, Performance
- Uses `powerprofilesctl set {profile}` command
- Buttons disabled if `powerprofilesctl` not found
- Shows current profile in "Profile:" row

## Wi-Fi Window

### What It Shows
- List of available Wi-Fi networks in a DataGrid
- Signal strength, security type, connection status
- Double-click to connect to networks
- Refresh button to rescan networks

### How It Works
1. Window opens → calls `RefreshWiFiDataAsync()`
2. Runs `nmcli device wifi rescan` to trigger fresh scan
3. Waits 3 seconds for scan to complete
4. Runs `nmcli device wifi list` command
5. **Robust Multi-Strategy Parsing**:
   - **Column Strategy**: Splits on 2+ whitespace, maps columns
   - **Regex Strategy**: Uses pattern matching for structured output
   - **Flexible Strategy**: Heuristic parsing for edge cases
6. **Validation** filters out:
   - Empty SSIDs
   - Parsing artifacts (SSID="Infra", "Adhoc")
   - Invalid signal strengths
   - Invalid channel numbers
7. Displays valid networks in DataGrid
8. Uses `BoolToStatusConverter` to show "Connected"/"Available"

### WiFi Connection
1. Double-click network → `AttemptWiFiConnectionAsync(ssid)`
2. Tries connection: `nmcli device wifi connect "{ssid}"`
3. If fails with "Secrets required" → opens password popup
4. Retries with password: `nmcli device wifi connect "{ssid}" password "{password}"`
5. Shows success/failure message
6. Refreshes network list to show updated connection status

## Enhanced WiFi Parsing (Current Implementation)

### Multi-Strategy Approach
The app now uses three parsing strategies to handle different `nmcli` output formats:

#### Column Strategy
```csharp
// Splits lines on 2+ whitespace → columns
// Maps: [0] IN-USE, [1] BSSID, [2] SSID, [3] MODE, [4] CHAN, [5] RATE, [6] SIGNAL, [7] BARS, [8+] SECURITY
```

#### Regex Strategy  
```csharp
// Pattern: ^(\*?\s*)([0-9a-fA-F:]+|\s+)\s+([^:]+?)\s+(\w+)\s+(\d+)\s+([\d.]+[MG]?)\s+(\d+)\s+(\W+)\s*(.*)$
// Handles structured nmcli output with proper column matching
```

#### Flexible Strategy
```csharp
// Heuristic parsing for edge cases
// Finds SSID position, then assigns remaining fields by position
// Most robust for unusual output formats
```

### Enhanced Validation
```csharp
// Filters out parsing artifacts:
if (network.SSID == "Infra" || network.SSID == "Adhoc") → Skip
if (network.Signal.Contains("▂") || network.Signal.Contains("▄")) → Valid (signal bars)
if (int.TryParse(network.Signal, out signal) && signal >= 0 && signal <= 100) → Valid (numeric)
```

### Retry Logic
- **3 attempts** with **1-second delays**
- **Console logging** for debugging
- **Graceful degradation** - returns empty list if all strategies fail

## Error Handling
- Most features disable if required Linux command not found
- Shows clear "not available" messages
- Try/catch blocks around all command execution
- No crashes for missing dependencies
- **WiFi parsing** has comprehensive error handling and logging

## Helper Methods & Information Flow

### Command Execution Flow
1. **UI Action** (slider move, button click)
2. **Property Setter** or **Click Handler** runs
3. **Command Method** called (`_changer.SetVolumeAsync()` etc.)
4. **TerminalCommands.RunCommandAsync()** executes Linux command
5. **Process.Start()** runs bash with command
6. **Output/Result** returned back up the chain
7. **UI Updated** with new values or status

### Key Helper Methods

#### GetWiFiNetworksAsync() (Enhanced)
```csharp
// Runs: "nmcli device wifi list" → raw network list
// Tries: 3 different parsing strategies
// Validates: Each parsed network for data integrity
// Retries: Up to 3 times with delays
// Returns: ObservableCollection<WiFiInfo> for UI binding
```

### Property Change Pattern
```csharp
// When slider value changes:
public int SoundLevel {
    set {
        if (_soundLevel != value) {           // Only if actually changed
            _soundLevel = value;             // Store new value
            if (_soundAvailable)              // If command available
                _ = _changer.SetVolumeAsync(value); // Run Linux command
            OnPropertyChanged();             // Update UI text
        }
    }
}
```

## Linux Commands Used

### For Display/Info
- `which {command}` - check if command exists
- `brightnessctl get` - get current brightness
- `brightnessctl max` - get max brightness  
- `pactl get-sink-volume @DEFAULT_SINK@` - get current volume
- `upower -e | grep battery` - find battery device
- `upower -i {device}` - get battery info
- `nmcli device wifi list` - list Wi-Fi networks
- `nmcli device wifi rescan` - trigger fresh network scan
- `powerprofilesctl get` - get current power profile

### For Changes
- `brightnessctl set {percent}%` - set brightness
- `pactl set-sink-volume @DEFAULT_SINK@ {percent}%` - set volume
- `powerprofilesctl set {profile}` - set power profile
- `nmcli device wifi connect "{ssid}"` - connect to open network
- `nmcli device wifi connect "{ssid}" password "{pass}"` - connect to secured network

## Distribution Compatibility

### Excellent Compatibility (NetworkManager-based)
- **Ubuntu/Debian** - NetworkManager pre-installed
- **Fedora/CentOS/RHEL** - NetworkManager default
- **Arch Linux** - NetworkManager commonly used
- **openSUSE** - NetworkManager default
- **Mint/Pop!_OS** - Ubuntu-based, full compatibility

### Variable Compatibility
- **Gentoo** - Depends on user's NetworkManager choice
- **Slackware** - May require manual NetworkManager installation
- **Alpine Linux** - Often uses different network tools

### Requirements
1. **NetworkManager** must be installed and running
2. **nmcli** tool must be available  
3. **User permissions** for network operations
4. **Standard Linux tools**: bash, grep, which

## Security Considerations
- Passwords are only used for WiFi connection
- No password storage or logging
- Commands executed via bash with proper escaping
- No elevation/sudo requirements

## What Works Now
✅ **Brightness control** - via brightnessctl
✅ **Volume control** - via pactl  
✅ **Battery monitoring** - via upower
✅ **Power profiles** - via powerprofilesctl
✅ **WiFi scanning** - robust nmcli parsing with fallbacks
✅ **WiFi connection** - with password support and retry logic
✅ **Cross-distro compatibility** - works where NetworkManager available
✅ **Error handling** - graceful degradation for missing tools

## What Could Be Added
- Bluetooth management
- User account management
- Package manager GUI
- System monitoring (CPU, memory, disk)
- Network statistics
- Custom themes/skins

## Testing
- **Tested on**: Linux Mint 22.2, Debian 13, Ubuntu 24.04
- **Dependencies**: Requires NetworkManager and standard Linux tools
- **Fallbacks**: Graceful handling of missing commands

## Building
- **Development**: `dotnet build`
- **Release**: `dotnet publish -c Release -r linux-x64 --self-contained`
- **Debian package**: `./build-deb.sh`
- **Includes**: .NET runtime (no separate install needed)

## Architecture Notes
- **Terminal-only approach** chosen over DBus for reliability
- **Multi-strategy parsing** handles nmcli output variations
- **Async/await** throughout for responsive UI
- **MVVM pattern** with data binding
- **Error-first design** with comprehensive logging
