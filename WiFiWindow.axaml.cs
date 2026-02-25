using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
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
    public event PropertyChangedEventHandler? PropertyChanged;

    private List<WiFiInfo> _wiFiNetworks = new List<WiFiInfo>();
    public List<WiFiInfo> WiFiNetworks
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
        InitializeComponent();
        DataContext = this;
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
                WiFiNetworks = await checker.GetWiFiNetworksAsync();
                StatusText = $"Loaded {WiFiNetworks.Count} WiFi networks.";
            }
            else
            {
                WiFiNetworks = new List<WiFiInfo>();
                StatusText = "nmcli is not available.";
            }
        }
        catch (Exception ex)
        {
            WiFiNetworks = new List<WiFiInfo>();
            StatusText = $"Error loading WiFi networks: {ex.Message}";
        }

        // Update the DataGrid manually to ensure it updates
        if (this.FindControl<DataGrid>("WiFiDataGrid") is DataGrid dataGrid)
        {
            dataGrid.ItemsSource = WiFiNetworks;
        }
    }

    private void Refresh_Click(object? sender, RoutedEventArgs e)
    {
        _ = RefreshWiFiDataAsync();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
