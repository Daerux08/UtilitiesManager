using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
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

public partial class WiFiWindow : Window
{
    public List<WiFiInfo> WiFiNetworks { get; private set; } = new List<WiFiInfo>();

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
        var checker = new CheckDependencyCommand();
        await checker.CheckDependenciesAsync();
        
        if (checker.IsNmcliAvailable)
        {
            WiFiNetworks = await checker.GetWiFiNetworksAsync();
        }
        else
        {
            WiFiNetworks = new List<WiFiInfo>();
        }

        // Update the DataGrid
        if (this.FindControl<DataGrid>("WiFiDataGrid") is DataGrid dataGrid)
        {
            dataGrid.ItemsSource = null;
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
