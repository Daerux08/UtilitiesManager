# UtilitiesManager - GUI Implementation

## GUI Overview
The GUI interface provides an Avalonia-based desktop application with intuitive controls for system utilities.

## GUI Main Files
1. `Program.cs` - Application entry point and environment detection
2. `App.axaml/.cs` - Avalonia application setup
3. `MainWindow.axaml/.cs` - Main window with sliders and buttons
4. `BatteryWindow.axaml/.cs` - Battery status window
5. `WiFiWindow.axaml/.cs` - WiFi management window
6. `EnterPasswordPopup.axaml/.cs` - WiFi password input dialog
7. `Terminal.cs` - Linux command execution and parsing
8. `Help.cs` - Help documentation

## How GUI Mode Starts
1. `Program.cs` checks environment variables (DISPLAY/WAYLAND_DISPLAY)
2. If GUI available, starts Avalonia application
3. `App.axaml.cs` creates and shows MainWindow
4. `MainWindow` constructor calls `InitializeValues()`
5. `CheckDependencyCommand.LoadOriginalValuesAsync()` checks dependencies
6. UI controls are enabled/disabled based on available tools

## MainWindow Features

### Brightness Slider
- **Control**: Slider from 0-100%
- **Enabled when**: `brightnessctl` command is available
- **Command used**: `brightnessctl set {percent}%`
- **Real-time**: Shows current brightness on load

### Volume Slider
- **Control**: Slider from 0-100%
- **Enabled when**: `pactl` command is available
- **Command used**: `pactl set-sink-volume @DEFAULT_SINK@ {percent}%`
- **Real-time**: Shows current volume on load

### WiFi Button
- **Action**: Opens WiFiWindow when clicked
- **Enabled when**: `nmcli` command is available
- **Tooltip**: Shows current connection status
- **Visual**: Changes based on connection state

### Battery Button
- **Action**: Opens BatteryWindow when clicked
- **Enabled when**: `upower` command is available
- **Tooltip**: Shows current battery percentage
- **Visual**: Changes color based on battery level

## Battery Window

### Information Displayed
1. **Charge Percentage** - Current battery level
2. **Battery State** - Charging/discharging/fully-charged/unknown
3. **Time to Empty** - Estimated remaining time
4. **Time to Full** - Time until fully charged
5. **Energy Rate** - Current power draw in watts
6. **Present** - Whether battery is detected

### Power Profile Buttons
- **Power Saver** - `powerprofilesctl set power-saver`
- **Balanced** - `powerprofilesctl set balanced`
- **Performance** - `powerprofilesctl set performance`
- **Enabled when**: `powerprofilesctl` command is available
- **Visual**: Highlights current profile

### How Battery Window Works
1. Window opens → calls `RefreshBatteryDataAsync()`
2. Checks if `upower` is available
3. Runs `upower -e | grep battery` to find battery device
4. Runs `upower -i {device}` to get battery info
5. Parses output for all battery statistics
6. Updates all TextBlocks and power profile buttons

## WiFi Window

### DataGrid Display
- **Columns**: SSID, Mode, Channel, Rate, Signal, Security
- **Sorting**: By signal strength (strongest first)
- **Highlighting**: Current connection marked with `*`
- **Refresh**: Manual refresh button for rescanning

### WiFi Connection Process
1. **Double-click network** → `AttemptWiFiConnectionAsync(ssid)`
2. **Try connection**: `nmcli device wifi connect "{ssid}"`
3. **If fails with "Secrets required"** → opens password popup
4. **Retry with password**: `nmcli device wifi connect "{ssid}" password "{password}"`
5. **Show success/failure message**
6. **Refresh network list** to show updated connection status

### How WiFi Window Works
1. Window opens → calls `RefreshWiFiDataAsync()`
2. Runs `nmcli device wifi rescan` to trigger fresh scan
3. Waits 3 seconds for scan to complete
4. Runs `nmcli device wifi list` command
5. Tries 3 different parsing methods to read the output
6. Filters out invalid networks
7. Displays valid networks in DataGrid

## EnterPasswordPopup

### Purpose
- Secure password input for WiFi networks
- Modal dialog with password field
- Show/hide password toggle
- Connect and Cancel buttons

### How it Works
1. Opens when WiFi connection requires password
2. User enters password (masked by default)
3. Can toggle password visibility
4. Connect button retries WiFi connection with password
5. Cancel button closes popup

## GUI Linux Commands Used

### Display/Info Commands
1. `which {command}` - Check if command exists
2. `brightnessctl get` - Get current brightness
3. `brightnessctl max` - Get maximum brightness
4. `pactl get-sink-volume @DEFAULT_SINK@` - Get current volume
5. `upower -e | grep battery` - Find battery device
6. `upower -i {device}` - Get battery info
7. `nmcli device wifi list` - List WiFi networks
8. `nmcli device wifi rescan` - Trigger fresh network scan
9. `powerprofilesctl get` - Get current power profile

### Control Commands
1. `brightnessctl set {percent}%` - Set brightness
2. `pactl set-sink-volume @DEFAULT_SINK@ {percent}%` - Set volume
3. `powerprofilesctl set {profile}` - Set power profile
4. `nmcli device wifi connect "{ssid}"` - Connect to open network
5. `nmcli device wifi connect "{ssid}" password "{pass}"` - Connect to secured network

## GUI Features Working
1. **Brightness Control** - Via brightnessctl with real-time feedback
2. **Volume Control** - Via pactl with immediate effect
3. **Battery Monitoring** - Comprehensive battery information display
4. **Power Profiles** - Easy profile switching with visual feedback
5. **WiFi Scanning** - Robust nmcli parsing with multiple fallbacks
6. **WiFi Connection** - Secure password handling and retry logic
7. **Cross-Distro Compatibility** - Works where NetworkManager available
8. **Error Handling** - Graceful degradation for missing tools
9. **Real-time Updates** - Current values shown on load
10. **Visual Feedback** - Tooltips and status indicators

## GUI Dependencies

### Required Dependencies
- **brightnessctl** - Screen brightness control
- **pactl** (pulseaudio) - Audio volume control
- **upower** - Battery monitoring
- **nmcli** (NetworkManager) - WiFi management
- **powerprofilesctl** - Power profile management

### System Requirements
- **Display Server** - X11 or Wayland
- **Desktop Environment** - Any Linux desktop
- **NetworkManager** - For WiFi functionality
- **PulseAudio** - For audio control

## GUI Error Handling
- **Missing Tools** - Controls disabled when dependencies unavailable
- **Network Errors** - Shows connection failure messages
- **Permission Issues** - Suggests user/group configuration
- **Parsing Failures** - Multiple fallback strategies for command output
- **WiFi Timeouts** - Handles scan delays and connection timeouts

## GUI Testing
- **Tested on**: Linux Mint 22.2 (Cinnamon), Debian 13 (XFCE), Ubuntu 24.04
- **Display Servers**: X11 and Wayland
- **Desktop Environments**: Cinnamon, XFCE, GNOME, KDE
- **Compatibility**: Works with most Linux desktop environments

## GUI Architecture
- **Framework**: Avalonia UI for cross-platform desktop applications
- **Pattern**: MVVM (Model-View-ViewModel) with code-behind
- **Threading**: Async operations for command execution
- **Localization**: Currently English-only
- **Theme**: System default theme integration
