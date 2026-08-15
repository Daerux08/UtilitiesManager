using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace UtilitiesManager.ViewModels
{
    public class BluetoothWindowViewModel : BaseViewModel
    {
        private readonly CheckDependencyCommand _checker = new();
        private readonly ChangeValueCommand _changer = new();

        private ObservableCollection<BluetoothInfo> _bluetoothDevices = new ObservableCollection<BluetoothInfo>();
        private string _statusText = "Loading Bluetooth devices...";
        private BluetoothInfo? _selectedDevice;
        private bool _isScanning = false;
        private bool _isPowered = false;
        private bool _isDiscoverable = false;

        public ObservableCollection<BluetoothInfo> BluetoothDevices
        {
            get => _bluetoothDevices;
            set => SetProperty(ref _bluetoothDevices, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public BluetoothInfo? SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                if (SetProperty(ref _isScanning, value))
                {
                    OnPropertyChanged(nameof(ScanButtonText));
                }
            }
        }

        public bool IsPowered
        {
            get => _isPowered;
            set
            {
                if (SetProperty(ref _isPowered, value))
                {
                    OnPropertyChanged(nameof(PoweredButtonText));
                    OnPropertyChanged(nameof(PoweredButtonBackground));
                }
            }
        }

        public bool IsDiscoverable
        {
            get => _isDiscoverable;
            set
            {
                if (SetProperty(ref _isDiscoverable, value))
                {
                    OnPropertyChanged(nameof(DiscoverableButtonText));
                    OnPropertyChanged(nameof(DiscoverableButtonBackground));
                }
            }
        }

        public string PoweredButtonText => IsPowered ? "On" : "Off";
        public string DiscoverableButtonText => IsDiscoverable ? "Yes" : "No";
        public string ScanButtonText => IsScanning ? "Stop Scan" : "Scan";
        public IBrush PoweredButtonBackground => IsPowered ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        public IBrush DiscoverableButtonBackground => IsDiscoverable ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);

        public ICommand RefreshCommand { get; }
        public ICommand ActiveBoolCommand { get; }
        public ICommand DiscoverableCommand { get; }
        public ICommand ScanCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand PairSelectedCommand { get; }
        public ICommand ForgetSelectedCommand { get; }
        public ICommand DisconnectSelectedCommand { get; }
        public ICommand TrustSelectedCommand { get; }

        public BluetoothWindowViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh);
            ActiveBoolCommand = new RelayCommand(TogglePower, () => _checker.IsBluetoothctlAvailable);
            DiscoverableCommand = new RelayCommand(ToggleDiscoverable, () => _checker.IsBluetoothctlAvailable);
            ScanCommand = new RelayCommand(ToggleScan, () => _checker.IsBluetoothctlAvailable);
            ConnectCommand = new RelayCommand(ConnectToSelected, () => SelectedDevice != null);
            CloseCommand = new RelayCommand(Close);
            PairSelectedCommand = new RelayCommand(PairSelected, () => SelectedDevice != null);
            ForgetSelectedCommand = new RelayCommand(ForgetSelected, () => SelectedDevice != null);
            DisconnectSelectedCommand = new RelayCommand(DisconnectSelected, () => SelectedDevice != null);
            TrustSelectedCommand = new RelayCommand(TrustSelected, () => SelectedDevice != null);

            _ = RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                StatusText = "Refreshing...";
                _checker.CheckDependencies();

                if (_checker.IsBluetoothctlAvailable)
                {
                    var controllerState = _checker.GetBluetoothControllerState();
                    IsPowered = controllerState.IsPowered;
                    IsDiscoverable = controllerState.IsDiscoverable;

                    var newDevices = await _checker.GetBluetoothDevicesAsync();
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        BluetoothDevices.Clear();
                        foreach (var device in newDevices)
                        {
                            BluetoothDevices.Add(device);
                        }
                    });

                    StatusText = $"Found {newDevices.Count} Bluetooth devices.";
                }
                else
                {
                    BluetoothDevices.Clear();
                    StatusText = "bluetoothctl is not available. Bluetooth functionality requires BlueZ.";
                }
            }
            catch (Exception ex)
            {
                BluetoothDevices.Clear();
                StatusText = $"Error loading Bluetooth devices: {ex.Message}";
            }
        }

        private void Refresh()
        {
            _ = RefreshAsync();
        }

        private async void TogglePower()
        {
            if (!_checker.IsBluetoothctlAvailable)
                return;

            var targetState = !IsPowered;
            StatusText = targetState ? "Enabling Bluetooth..." : "Disabling Bluetooth...";

            var result = await TerminalCommands.RunCommandWithResultAsync(
                targetState ? "bluetoothctl power on" : "bluetoothctl power off"
            );

            if (result.IsSuccess)
            {
                IsPowered = targetState;
                StatusText = targetState ? "Bluetooth enabled" : "Bluetooth disabled";
                await Task.Delay(1000);
                await RefreshAsync();
            }
            else
            {
                StatusText = "Failed to change Bluetooth power state";
            }
        }

        private async void ToggleDiscoverable()
        {
            if (!_checker.IsBluetoothctlAvailable)
                return;

            var targetState = !IsDiscoverable;
            StatusText = targetState ? "Enabling discoverability..." : "Disabling discoverability...";

            var result = await TerminalCommands.RunCommandWithResultAsync(
                targetState ? "bluetoothctl discoverable on" : "bluetoothctl discoverable off"
            );

            if (result.IsSuccess)
            {
                IsDiscoverable = targetState;
                StatusText = targetState ? "Bluetooth is now discoverable" : "Bluetooth is no longer discoverable";
                await Task.Delay(1000);
                await RefreshAsync();
            }
            else
            {
                StatusText = "Failed to change discoverability";
            }
        }

        private async void ToggleScan()
        {
            if (!_checker.IsBluetoothctlAvailable)
                return;

            if (IsScanning)
            {
                IsScanning = false;
                StatusText = "Stopping scan...";

                var result = await TerminalCommands.RunCommandWithResultAsync("bluetoothctl scan off");
                if (result.IsSuccess)
                {
                    StatusText = "Scan stopped";
                    await Task.Delay(1000);
                    await RefreshAsync();
                }
                else
                {
                    StatusText = "Failed to stop Bluetooth scan";
                }

                return;
            }

            IsScanning = true;
            StatusText = "Scanning for devices...";

            var scanResult = await TerminalCommands.RunCommandWithResultAsync(
                "stdbuf -oL bluetoothctl --timeout 30 scan on 2>/dev/null | grep --line-buffered \"Device\""
            );

            if (scanResult.IsSuccess || !string.IsNullOrWhiteSpace(scanResult.CombinedOutput))
            {
                var updates = await _checker.GetBluetoothScanUpdatesAsync();
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var removedAddress in updates.RemovedAddresses)
                    {
                        var match = BluetoothDevices.FirstOrDefault(device =>
                            string.Equals(device.Address, removedAddress, StringComparison.OrdinalIgnoreCase));

                        if (match != null)
                        {
                            BluetoothDevices.Remove(match);
                        }
                    }

                    foreach (var device in updates.Devices)
                    {
                        var existing = BluetoothDevices.FirstOrDefault(item =>
                            string.Equals(item.Address, device.Address, StringComparison.OrdinalIgnoreCase));

                        if (existing != null)
                        {
                            existing.Name = device.Name;
                            existing.Alias = device.Alias;
                            existing.Available = true;
                        }
                        else
                        {
                            BluetoothDevices.Add(device);
                        }
                    }
                });

                StatusText = $"Found {BluetoothDevices.Count} Bluetooth devices.";
                IsScanning = false;
                return;
            }

            IsScanning = false;
            StatusText = "Failed to start Bluetooth scan";
        }

        private async void ConnectToSelected()
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Connecting to {SelectedDevice.Name}...";
                var result = await TerminalCommands.RunCommandWithResultAsync($"bluetoothctl connect {SelectedDevice.Address}");

                if (result.IsSuccess)
                {
                    StatusText = $"Connected to {SelectedDevice.Name}";
                    await Task.Delay(2000);
                    await RefreshAsync();
                    return;
                }

                var output = result.CombinedOutput;
                var requiresAuth = output.Contains("Authentication", StringComparison.OrdinalIgnoreCase)
                    || output.Contains("PIN", StringComparison.OrdinalIgnoreCase)
                    || output.Contains("Passkey", StringComparison.OrdinalIgnoreCase)
                    || output.Contains("pairing", StringComparison.OrdinalIgnoreCase)
                    || output.Contains("Failed to pair", StringComparison.OrdinalIgnoreCase)
                    || output.Contains("not paired", StringComparison.OrdinalIgnoreCase)
                    || output.Contains("needs authentication", StringComparison.OrdinalIgnoreCase)
                    || output.Contains("password", StringComparison.OrdinalIgnoreCase);

                if (requiresAuth)
                {
                    StatusText = "Authentication required for this connection";
                    PinRequested?.Invoke(this, SelectedDevice.Name);
                    return;
                }

                StatusText = "Connection failed";
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        private async void PairSelected()
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Pairing with {SelectedDevice.Name}...";
                var result = await TerminalCommands.RunCommandWithResultAsync($"bluetoothctl pair {SelectedDevice.Address}");

                if (result.IsSuccess)
                {
                    StatusText = $"Paired with {SelectedDevice.Name}";
                    await Task.Delay(2000);
                    await RefreshAsync();
                }
                else
                {
                    var output = result.CombinedOutput;
                    var requiresAuth = output.Contains("Authentication", StringComparison.OrdinalIgnoreCase)
                        || output.Contains("PIN", StringComparison.OrdinalIgnoreCase)
                        || output.Contains("Passkey", StringComparison.OrdinalIgnoreCase)
                        || output.Contains("pairing", StringComparison.OrdinalIgnoreCase)
                        || output.Contains("password", StringComparison.OrdinalIgnoreCase);

                    if (requiresAuth)
                    {
                        StatusText = "Pairing requires authentication";
                        PinRequested?.Invoke(this, SelectedDevice.Name);
                    }
                    else
                    {
                        StatusText = "Pairing failed";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        private async void ForgetSelected()
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Removing {SelectedDevice.Name}...";
                var result = await TerminalCommands.RunCommandWithResultAsync($"bluetoothctl remove {SelectedDevice.Address}");

                if (result.IsSuccess)
                {
                    StatusText = $"Removed {SelectedDevice.Name}";
                    await Task.Delay(1000);
                    await RefreshAsync();
                }
                else
                {
                    StatusText = "Remove failed";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        private async void DisconnectSelected()
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Disconnecting from {SelectedDevice.Name}...";
                var result = await TerminalCommands.RunCommandWithResultAsync($"bluetoothctl disconnect {SelectedDevice.Address}");

                if (result.IsSuccess)
                {
                    StatusText = $"Disconnected from {SelectedDevice.Name}";
                    await Task.Delay(1000);
                    await RefreshAsync();
                }
                else
                {
                    StatusText = "Disconnection failed";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        private async void TrustSelected()
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Trusting {SelectedDevice.Name}...";
                var result = await TerminalCommands.RunCommandWithResultAsync($"bluetoothctl trust {SelectedDevice.Address}");

                if (result.IsSuccess)
                {
                    StatusText = $"Auto-pair enabled for {SelectedDevice.Name}";
                    await Task.Delay(1000);
                    await RefreshAsync();
                }
                else
                {
                    StatusText = "Trust failed";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        public async Task PairWithPin(string deviceName, string pin)
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Pairing with {deviceName} using PIN...";

                var result = await TerminalCommands.RunCommandWithResultAsync(
                    $"echo -e \"pair {SelectedDevice.Address}\n{pin}\n\" | bluetoothctl"
                );

                if (result.IsSuccess && !result.CombinedOutput.Contains("Failed", StringComparison.OrdinalIgnoreCase))
                {
                    StatusText = $"Paired with {deviceName}";
                    await Task.Delay(2000);
                    await RefreshAsync();
                }
                else
                {
                    StatusText = "Pairing with PIN failed";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        private void Close()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CloseRequested;
        public event EventHandler<string>? PinRequested;
    }
}
