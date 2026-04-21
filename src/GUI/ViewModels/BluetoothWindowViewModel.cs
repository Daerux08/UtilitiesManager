using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
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
            set => SetProperty(ref _isScanning, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand ScanCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand PairCommand { get; }
        public ICommand TrustCommand { get; }
        public ICommand CloseCommand { get; }

        public BluetoothWindowViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh);
            ScanCommand = new RelayCommand(ToggleScan);
            ConnectCommand = new RelayCommand(ConnectToSelected, () => SelectedDevice != null && !SelectedDevice.Connected);
            DisconnectCommand = new RelayCommand(DisconnectFromSelected, () => SelectedDevice != null && SelectedDevice.Connected);
            PairCommand = new RelayCommand(PairWithSelected, () => SelectedDevice != null && !SelectedDevice.Paired);
            TrustCommand = new RelayCommand(TrustSelected, () => SelectedDevice != null && !SelectedDevice.Trusted);
            CloseCommand = new RelayCommand(Close);

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

        private async void ToggleScan()
        {
            if (IsScanning)
            {
                // Stop scanning
                IsScanning = false;
                StatusText = "Stopping scan...";
                var success = await _changer.StopBluetoothScanAsync();
                if (success)
                {
                    StatusText = "Scan stopped";
                    await Task.Delay(1000);
                    await RefreshAsync();
                }
                else
                {
                    StatusText = "Failed to stop scan";
                }
            }
            else
            {
                // Start scanning
                IsScanning = true;
                StatusText = "Scanning for devices...";
                var success = await _changer.ScanBluetoothDevicesAsync();
                if (success)
                {
                    StatusText = "Scanning... (will find new devices)";
                    // Wait a bit for scan to discover devices
                    await Task.Delay(5000);
                    await RefreshAsync();
                }
                else
                {
                    IsScanning = false;
                    StatusText = "Failed to start scan";
                }
            }
        }

        private async void ConnectToSelected()
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Connecting to {SelectedDevice.Name}...";
                var success = await _changer.ConnectToDeviceAsync(SelectedDevice.Address);

                if (success)
                {
                    StatusText = $"Connected to {SelectedDevice.Name}";
                    await Task.Delay(2000);
                    await RefreshAsync();
                }
                else
                {
                    StatusText = "Connection failed - device may need pairing";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        private async void DisconnectFromSelected()
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Disconnecting from {SelectedDevice.Name}...";
                var success = await _changer.DisconnectDeviceAsync(SelectedDevice.Address);

                if (success)
                {
                    StatusText = $"Disconnected from {SelectedDevice.Name}";
                    await Task.Delay(2000);
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

        private async void PairWithSelected()
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Pairing with {SelectedDevice.Name}...";
                var success = await _changer.PairDeviceAsync(SelectedDevice.Address);

                if (success)
                {
                    StatusText = $"Paired with {SelectedDevice.Name}";
                    await Task.Delay(2000);
                    await RefreshAsync();
                }
                else
                {
                    // Pairing might need a PIN/passkey
                    StatusText = "PIN or passkey may be required";
                    PinRequested?.Invoke(this, SelectedDevice.Name);
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        // Method to be called from View when PIN is provided
        public async Task PairWithPin(string deviceName, string pin)
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Pairing with {deviceName} using PIN...";
                
                // Use bluetoothctl with PIN - note: this is a simplified approach
                // In practice, bluetoothctl interaction with PINs is complex
                var result = await TerminalCommands.RunCommandWithResultAsync(
                    $"echo -e \"pair {SelectedDevice.Address}\n{pin}\n\" | bluetoothctl"
                );

                if (result.IsSuccess && !result.CombinedOutput.Contains("Failed"))
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

        private async void TrustSelected()
        {
            if (SelectedDevice == null) return;

            try
            {
                StatusText = $"Trusting {SelectedDevice.Name}...";
                var success = await _changer.TrustDeviceAsync(SelectedDevice.Address);

                if (success)
                {
                    StatusText = $"Trusted {SelectedDevice.Name}";
                    await Task.Delay(2000);
                    await RefreshAsync();
                }
                else
                {
                    StatusText = "Trust operation failed";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        private void Close()
        {
            // Stop any ongoing scan before closing
            if (IsScanning)
            {
                _ = _changer.StopBluetoothScanAsync();
            }
            
            // This will be handled by the View - trigger window close
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CloseRequested;
        public event EventHandler<string>? PinRequested;
    }
}
