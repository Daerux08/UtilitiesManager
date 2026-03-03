# UtilitiesManager - How It Works

## What This App Is
A simple Linux utility app that tries to act as a Control Panel

## Main Files
- `MainWindow.axaml` - Main window with sliders and buttons
- `MainWindow.axaml.cs` - Code behind for main window
- `Terminal.cs` - All the Linux command stuff
- `BatteryWindow.axaml/.cs` - Battery status window
- `WiFiWindow.axaml/.cs` - Wi-Fi networks window

## How It Starts
1. `Program.cs` starts the Avalonia app
2. `App.axaml.cs` creates the main window
3. `MainWindow` constructor calls `InitializeValues()`
4. `InitializeValues()` runs `CheckDependencyCommand.LoadOriginalValuesAsync()`
5. This checks what Linux commands are available and gets current values

## Main Window Features

### Sound Slider
- Slider bound to `SoundLevel` property
- When moved, calls `_changer.SetVolumeAsync(value)`
- Runs `pactl set-sink-volume @DEFAULT_SINK@ {value}%`
- Only enabled if `pactl` command is found

### Brightness Slider  
- Slider bound to `Brightness` property
- When moved, calls `_changer.SetBrightnessAsync(value)`
- Runs `brightnessctl set {value}%`
- Only enabled if `brightnessctl` command is found

### Wi-Fi Button
- Opens `WiFiWindow` when clicked
- Only enabled if `nmcli` command is found
- Uses `BoolToStringConverter` for tooltip text

### Battery Button
- Opens `BatteryWindow` when clicked
- Only enabled if `upower` command is found
- Uses `BoolToStringConverter` for tooltip text

## Terminal.cs - The Linux Commands

### Data Classes
```csharp
BatteryInfo {
    State: "charging" | "discharging" | etc.
    Percentage: 0-100
    TimeToEmpty: "2h 30m"  
    TimeToFull: "1h 15m"
    EnergyRate: Watts
    IsPresent: true/false
}

WiFiInfo {
    SSID: network name
    Mode: "Infrastructure" | "Ad-hoc"
    Chan: channel number
    Rate: speed
    Signal: signal strength
    Security: "WPA2" | "Open" | etc.
    IsActive: connected?
}
```

### Command Classes

#### TerminalCommands (static)
- `RunCommandAsync(string command)` - runs a Linux command, returns output
- `RunCommandWithResultAsync(string command)` - returns CommandResult with exit code

#### ChangeValueCommand
- `SetBrightnessAsync(percent)` - changes screen brightness
- `SetVolumeAsync(percentage)` - changes volume
- `SetPowerProfileAsync(profile)` - changes power profile

#### CheckDependencyCommand
- Checks if commands exist with `which {command}`
- Gets current values for brightness, volume, battery, wifi
- Properties: `IsBrightnessCtlAvailable`, `IsPactlAvailable`, etc.

## Battery Window

### What It Shows
- Charge percentage
- Battery state (charging/discharging)
- Time remaining or time to full
- Power draw in watts
- Current power profile

### How It Works
1. Window opens → calls `RefreshBatteryDataAsync()`
2. Checks if `upower` is available
3. Runs `upower -e | grep battery` to find battery device
4. Runs `upower -i {device}` to get battery info
5. Parses the output for battery stats
6. Updates all the TextBlocks

### Power Profile Buttons
- Three buttons: Power Saver, Balanced, Performance
- Uses `powerprofilesctl set {profile}` command
- Buttons disabled if `powerprofilesctl` not found
- Shows current profile in "Profile:" row

## Wi-Fi Window

### What It Shows
- List of available Wi-Fi networks
- Signal strength, security type, connection status
- Double-Click to connect/disconnect (not fully implemented)

### How It Works
1. Runs `nmcli device wifi list` command
2. Parses the output (whitespace-separated columns)
3. Creates `WiFiInfo` objects for each network
4. Displays in DataGrid
5. Uses `BoolToStatusConverter` to show "Connected"/"Available"

## Error Handling
- Most features just disable if required Linux command not found
- Shows "N/A" or "not found" messages
- Try/catch blocks around command execution
- No crashes for missing dependencies

## Helper Methods & Information Flow

### Command Execution Flow
1. **UI Action** (slider move, button click)
2. **Property Setter** or **Click Handler** runs
3. **Command Method** called (`_changer.SetVolumeAsync()` etc.)
4. **TerminalCommands.RunCommandAsync()** executes Linux command
5. **Process.Start()** runs bash with the command
6. **Output/Result** returned back up the chain
7. **UI Updated** with new values or status

### Key Helper Methods

#### TerminalCommands.RunCommandAsync()
```csharp
// Takes: string command like "brightnessctl set 50%"
// Returns: string output (trimmed)
// Process: Creates bash process, runs command, waits, returns output
```

#### CheckCommandAvailable()
```csharp
// Takes: command name like "brightnessctl"
// Returns: true/false if command exists
// Process: Runs "which {command}" and checks if output is empty
```

#### GetBrightnessPercentAsync()
```csharp
// Runs: "brightnessctl get" → current value
// Runs: "brightnessctl max" → maximum value  
// Calculates: (current / max) * 100 = percentage
// Returns: 0-100 or -1 if error
```

#### GetVolumeAsync()
```csharp
// Runs: "pactl get-sink-volume @DEFAULT_SINK@"
// Parses: Finds first "(\d+)%" pattern with regex
// Returns: the percentage number or -1 if error
```

#### GetBatteryAsync()
```csharp
// Runs: "upower -e | grep -i -m 1 battery" → find device path
// Runs: "upower -i {device}" → get battery info
// Parses line by line:
//   "battery present: yes" → IsPresent = true
//   "state: charging" → State = "charging"  
//   "percentage: 85%" → Percentage = 85
//   "time to empty: 2h 30m" → TimeToEmpty = "2h 30m"
//   "energy-rate: 15.5" → EnergyRate = 15.5
// Returns: BatteryInfo object with parsed data
```

#### GetWiFiNetworksAsync()
```csharp
// Runs: "nmcli device wifi list" → raw network list
// Skips: First line (header)
// Splits: Each line on 2+ whitespace → 8+ columns
// Maps columns to WiFiInfo:
//   [0] IN-USE (contains "*" = active)
//   [1] BSSID (ignored)
//   [2] SSID → network.SSID
//   [3] MODE → network.Mode
//   [4] CHAN → network.Chan
//   [5] RATE → network.Rate
//   [6] SIGNAL → network.Signal
//   [7] BARS (ignored)
//   [8+] SECURITY → network.Security
// Returns: ObservableCollection<WiFiInfo> for UI binding
```

#### GetCurrentPowerProfileAsync()
```csharp
// Runs: "powerprofilesctl get"
// Returns: Profile name like "balanced" or "power-saver"
// Error handling: Returns "Unknown" if command fails
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

### Window Opening Pattern
```csharp
// Battery Window:
OpenBattery_Click() → new BatteryWindow() → Show()
↓
BatteryWindow_Opened() → RefreshBatteryDataAsync()
↓
CheckDependenciesAsync() → GetBatteryAsync() → Update UI

// Wi-Fi Window:  
OpenWiFi_Click() → new WiFiWindow() → Show()
↓
WiFiWindow_Opened() → LoadWiFiNetworksAsync()
↓
GetWiFiNetworksAsync() → Update DataGrid
```

### Error Recovery
- **Command not found**: Dependency check fails → button disabled
- **Parse fails**: Try/catch returns default values (-1, "N/A", "Unknown")
- **No battery**: BatteryInfo.IsPresent = false, shows "N/A"
- **No Wi-Fi adapter**: nmcli fails → WiFi button disabled

## UI Stuff

### Converters
- `BoolToStringConverter` - converts bool to tooltip text
- `BoolToStatusConverter` - converts bool to "Connected"/"Available"

### Styles
- Custom `clickable-label` button style (looks like text, acts like button)
- Hover effects (blue underline)
- Disabled state (gray text)

### Data Binding
- MainWindow is its own DataContext
- Sliders bound to properties with `INotifyPropertyChanged`
- Buttons enabled/disabled based on dependency availability

## Linux Commands Used

### For Display/Info
- `which {command}` - check if command exists
- `brightnessctl get` - get current brightness
- `brightnessctl max` - get max brightness  
- `pactl get-sink-volume @DEFAULT_SINK@` - get current volume
- `upower -e | grep battery` - find battery device
- `upower -i {device}` - get battery info
- `nmcli device wifi list` - list Wi-Fi networks
- `powerprofilesctl get` - get current power profile

### For Changes
- `brightnessctl set {percent}%` - set brightness
- `pactl set-sink-volume @DEFAULT_SINK@ {percent}%` - set volume
- `powerprofilesctl set {profile}` - set power profile

## What Doesn't Work Yet
- Wi-Fi connecting (just shows networks)
- User management window
- Bluetooth management
- Package manager GUI
- Better edge case handling

## Testing
- Works on Linux Mint 22.2 Cinnamon
- Works on Debian 13 XFCE
- Some features depend on having the right Linux tools installed

## Building
- `dotnet publish -c Release -r linux-x64 --self-contained`
- Debian package via `build-deb.sh`
- Includes .NET runtime so no separate install needed
