using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;

namespace UtilitiesManager;

public class BoolToStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? "Connected" : "Available";
        }
        return "Unknown";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public partial class WiFiWindow : Window, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<WiFiInfo> _wiFiNetworks = new ObservableCollection<WiFiInfo>();
    public ObservableCollection<WiFiInfo> WiFiNetworks
    {
        get => _wiFiNetworks;
        private set
        {
            _wiFiNetworks = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WiFiNetworks)));
        }
    }

    private string _statusText = "Loading WiFi networks...";
    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
        }
    }

    public WiFiWindow()
    {
        DataContext = this;
        InitializeComponent();
        Opened += WiFiWindow_Opened;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void WiFiWindow_Opened(object? sender, EventArgs e)
    {
        try
        {
            await RefreshWiFiDataAsync();
        }
        catch (Exception ex)
        {
            StatusText = $"Error loading WiFi networks: {ex.Message}";
        }
    }

    private async Task RefreshWiFiDataAsync()
    {
        try
        {
            var checker = new CheckDependencyCommand();
            await checker.CheckDependenciesAsync();
            
            if (checker.IsNmcliAvailable)
            {
                // Perform rescan before listing
                await TerminalCommands.RunCommandAsync("nmcli device wifi rescan");
                
                // Wait for rescan to complete
                await Task.Delay(3000);
                
                var newNetworks = await checker.GetWiFiNetworksAsync();
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

    private async void Refresh_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            StatusText = "Refreshing...";
            await RefreshWiFiDataAsync();
        }
        catch (Exception ex)
        {
            StatusText = $"Error refreshing WiFi networks: {ex.Message}";
        }
    }

    private async void WiFiDataGrid_DoubleTapped(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is DataGrid grid && grid.SelectedItem is WiFiInfo selectedNetwork)
            {
                await AttemptWiFiConnectionAsync(selectedNetwork.SSID);
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error connecting to WiFi: {ex.Message}";
        }
    }

    private async Task AttemptWiFiConnectionAsync(string ssid, string? password = null)
    {
        try
        {
            StatusText = "Attempting to connect...";

            // Build the command
            string command = password == null
                ? $"nmcli device wifi connect \"{ssid}\""
                : $"nmcli device wifi connect \"{ssid}\" password \"{password}\"";

            // Execute with result capturing
            var result = await TerminalCommands.RunCommandWithResultAsync(command);

            // Check for success
            if (result.IsSuccess)
            {
                await ShowMessageBox($"Successfully connected to {ssid}");
                StatusText = $"Connected to {ssid}";
                await Task.Delay(2000);
                await RefreshWiFiDataAsync();
                return;
            }

            // Check if secrets are required
            string combinedOutput = result.CombinedOutput;
            if (combinedOutput.Contains("Secrets were required, but not provided", StringComparison.OrdinalIgnoreCase) ||
                combinedOutput.Contains("no-secrets", StringComparison.OrdinalIgnoreCase) ||
                combinedOutput.Contains("Secrets", StringComparison.OrdinalIgnoreCase) ||
                result.ExitCode == 7)
            {
                // Open password popup
                var passwordPopup = new EnterPasswordPopup(ssid);
                var passwordResult = await passwordPopup.ShowDialog<string?>(this);

                if (!string.IsNullOrEmpty(passwordResult))
                {
                    // Retry with password
                    await AttemptWiFiConnectionAsync(ssid, passwordResult);
                }
                else
                {
                    StatusText = "Connection cancelled";
                }
            }
            else
            {
                // Other error
                await ShowMessageBox($"Connection failed:\n{combinedOutput}");
                StatusText = "Connection failed";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
            await ShowMessageBox($"Error connecting to WiFi: {ex.Message}");
        }
    }

    private async Task ShowMessageBox(string message)
    {
        var stackPanel = new StackPanel
        {
            Margin = new Thickness(20)
        };

        var textBlock = new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 20)
        };
        stackPanel.Children.Add(textBlock);

        var button = new Button
        {
            Content = "OK",
            Width = 100,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };
        stackPanel.Children.Add(button);

        var messageBox = new Window
        {
            Title = "WiFi Connection",
            Width = 400,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = stackPanel
        };

        button.Click += (s, e) => messageBox.Close();
        await messageBox.ShowDialog(this);
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
