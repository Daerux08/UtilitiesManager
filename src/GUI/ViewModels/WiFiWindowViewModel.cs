using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;

namespace UtilitiesManager.ViewModels
{
    public class WiFiWindowViewModel : BaseViewModel
    {
        private readonly CheckDependencyCommand _checker = new();
        
        private ObservableCollection<WiFiInfo> _wiFiNetworks = new ObservableCollection<WiFiInfo>();
        private string _statusText = "Loading WiFi networks...";
        private WiFiInfo? _selectedNetwork;

        public ObservableCollection<WiFiInfo> WiFiNetworks
        {
            get => _wiFiNetworks;
            set => SetProperty(ref _wiFiNetworks, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public WiFiInfo? SelectedNetwork
        {
            get => _selectedNetwork;
            set => SetProperty(ref _selectedNetwork, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand CloseCommand { get; }

        public WiFiWindowViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh);
            ConnectCommand = new RelayCommand(ConnectToSelected, () => SelectedNetwork != null);
            CloseCommand = new RelayCommand(Close);

            _ = RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                StatusText = "Refreshing...";
                await _checker.CheckDependenciesAsync();
                
                if (_checker.IsNmcliAvailable)
                {
                    await TerminalCommands.RunCommandAsync("nmcli device wifi rescan");
                    await Task.Delay(3000);
                    
                    var newNetworks = await _checker.GetWiFiNetworksAsync();
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        WiFiNetworks.Clear();
                        foreach (var network in newNetworks)
                        {
                            WiFiNetworks.Add(network);
                        }
                    });
                    StatusText = $"Found {newNetworks.Count} available networks.";
                }
                else
                {
                    WiFiNetworks.Clear();
                    StatusText = "nmcli is not available. WiFi functionality requires NetworkManager.";
                }
            }
            catch (Exception ex)
            {
                WiFiNetworks.Clear();
                StatusText = $"Error loading WiFi networks: {ex.Message}";
            }
        }

        private void Refresh()
        {
            _ = RefreshAsync();
        }

        private async void ConnectToSelected()
        {
            if (SelectedNetwork == null) return;

            try
            {
                StatusText = "Attempting to connect...";
                string command = $"nmcli device wifi connect \"{SelectedNetwork.SSID}\"";
                var result = await TerminalCommands.RunCommandWithResultAsync(command);

                if (result.IsSuccess)
                {
                    StatusText = $"Connected to {SelectedNetwork.SSID}";
                    await Task.Delay(2000);
                    await RefreshAsync();
                    return;
                }

                string combinedOutput = result.CombinedOutput;
                if (combinedOutput.Contains("Secrets were required", StringComparison.OrdinalIgnoreCase) ||
                    combinedOutput.Contains("no-secrets", StringComparison.OrdinalIgnoreCase) ||
                    combinedOutput.Contains("Secrets", StringComparison.OrdinalIgnoreCase) ||
                    result.ExitCode == 7)
                {
                    // Request password from view
                    StatusText = "Password required for this network";
                    // The view will need to handle the password popup
                }
                else
                {
                    StatusText = "Connection failed";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        private void Close()
        {
            // This will be handled by the View
        }

        // Method to be called from the View when password is provided
        public async Task ConnectWithPassword(string ssid, string password)
        {
            try
            {
                StatusText = "Attempting to connect...";
                string command = $"nmcli device wifi connect \"{ssid}\" password \"{password}\"";
                var result = await TerminalCommands.RunCommandWithResultAsync(command);

                if (result.IsSuccess)
                {
                    StatusText = $"Connected to {ssid}";
                    await Task.Delay(2000);
                    await RefreshAsync();
                }
                else
                {
                    StatusText = "Connection failed";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }
    }
}
