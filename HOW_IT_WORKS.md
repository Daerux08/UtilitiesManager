# UtilitiesManager - How It Works

## What this app is
A simple Linux utility app that acts as a Control Panel for common system settings

## Main files
1. `MainWindow.axaml` - Main window with sliders and buttons
2. `MainWindow.axaml.cs` - Code behind for main window
3. `Terminal.cs` - All Linux command execution and parsing logic
4. `BatteryWindow.axaml/.cs` - Battery status window
5. `WiFiWindow.axaml/.cs` - Wi-Fi networks and connection management
6. `EnterPasswordPopup.axaml/.cs` - WiFi password input dialog

## How it starts
1. `Program.cs` starts Avalonia app
2. `App.axaml.cs` creates main window
3. `MainWindow` constructor calls `InitializeValues()`
4. `InitializeValues()` runs `CheckDependencyCommand.LoadOriginalValuesAsync()`
5. This checks what Linux commands are available and gets current values

## Main window features

### Brightness slider
- Only enabled if `brightnessctl` command is found
- Changes screen brightness using `brightnessctl set {percent}%`

### Volume slider
- Only enabled if `pactl` command is found
- Changes audio volume using `pactl set-sink-volume @DEFAULT_SINK@ {percent}%`

### Wi-Fi button
- Opens `WiFiWindow` when clicked
- Only enabled if `nmcli` command is found
- Shows current connection status in tooltip

### Battery button
- Opens `BatteryWindow` when clicked
- Only enabled if `upower` command is found
- Shows current battery percentage in tooltip

## Battery window

### What it shows
1. Charge percentage
2. Battery state (charging/discharging/fully-charged)
3. Time remaining or time to full
4. Power draw in watts
5. Current power profile with buttons to change

### How it works
1. Window opens → calls `RefreshBatteryDataAsync()`
2. Checks if `upower` is available
3. Runs `upower -e | grep battery` to find battery device
4. Runs `upower -i {device}` to get battery info
5. Parses output for battery stats
6. Updates all TextBlocks and power profile buttons

### Power profile buttons
1. Three buttons: Power Saver, Balanced, Performance
2. Uses `powerprofilesctl set {profile}` command
3. Buttons disabled if `powerprofilesctl` not found
4. Shows current profile in "Profile:" row

## Wi-Fi window

### What it shows
1. List of available Wi-Fi networks in a DataGrid
2. Signal strength, security type, connection status
3. Double-click to connect to networks
4. Refresh button to rescan networks

### How it works
1. Window opens → calls `RefreshWiFiDataAsync()`
2. Runs `nmcli device wifi rescan` to trigger fresh scan
3. Waits 3 seconds for scan to complete
4. Runs `nmcli device wifi list` command
5. Tries 3 different parsing methods to read the output
6. Filters out invalid networks
7. Displays valid networks in DataGrid

### Wi-Fi connection
1. Double-click network → `AttemptWiFiConnectionAsync(ssid)`
2. Tries connection: `nmcli device wifi connect "{ssid}"`
3. If fails with "Secrets required" → opens password popup
4. Retries with password: `nmcli device wifi connect "{ssid}" password "{password}"`
5. Shows success/failure message
6. Refreshes network list to show updated connection status

## Linux commands used

### For display/info
1. `which {command}` - check if command exists
2. `brightnessctl get` - get current brightness
3. `brightnessctl max` - get max brightness
4. `pactl get-sink-volume @DEFAULT_SINK@` - get current volume
5. `upower -e | grep battery` - find battery device
6. `upower -i {device}` - get battery info
7. `nmcli device wifi list` - list Wi-Fi networks
8. `nmcli device wifi rescan` - trigger fresh network scan
9. `powerprofilesctl get` - get current power profile

### For changes
1. `brightnessctl set {percent}%` - set brightness
2. `pactl set-sink-volume @DEFAULT_SINK@ {percent}%` - set volume
3. `powerprofilesctl set {profile}` - set power profile
4. `nmcli device wifi connect "{ssid}"` - connect to open network
5. `nmcli device wifi connect "{ssid}" password "{pass}"` - connect to secured network

## What works now
1. Brightness control - via brightnessctl
2. Volume control - via pactl
3. Battery monitoring - via upower
4. Power profiles - via powerprofilesctl
5. Wi-Fi scanning - robust nmcli parsing with fallbacks
6. Wi-Fi connection - with password support and retry logic
7. Cross-distro compatibility - works where NetworkManager available
8. Error handling - graceful degradation for missing tools

## What could be added
1. Bluetooth management
2. User account management
3. Package manager GUI
4. System monitoring (CPU, memory, disk)
5. Network statistics
6. Custom themes/skins

## Testing
1. Tested on: Linux Mint 22.2, Debian 13, Ubuntu 24.04
2. Dependencies: Requires NetworkManager and standard Linux tools
3. Fallbacks: Graceful handling of missing commands

## Building
1. Development: `dotnet build`
2. Release: `dotnet publish -c Release -r linux-x64 --self-contained`
3. Debian package: `./build-deb.sh`
4. Includes: .NET runtime (no separate install needed)
