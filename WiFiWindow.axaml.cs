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
        await RefreshWiFiDataAsync();
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
                StatusText = "nmcli is not available.";
            }
        }
        catch (Exception ex)
        {
            WiFiNetworks.Clear();
            StatusText = $"Error loading WiFi networks: {ex.Message}";
        }

        // No need to manually set ItemsSource since it's bound
    }

    private void Refresh_Click(object? sender, RoutedEventArgs e)
    {
        StatusText = "Refreshing...";
        _ = RefreshWiFiDataAsync();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
