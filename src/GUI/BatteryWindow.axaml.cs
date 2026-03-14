using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;
using UtilitiesManagerCLI;

namespace UtilitiesManager;

public partial class BatteryWindow : Window
{
    private readonly ChangeValueCommand _changer = new();
    private readonly CheckDependencyCommand _checker = new();

    public BatteryWindow()
    {
        InitializeComponent();
        Opened += BatteryWindow_Opened;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void BatteryWindow_Opened(object? sender, EventArgs e)
    {
        await RefreshBatteryDataAsync();
    }

    private async Task RefreshBatteryDataAsync()
    {
        await _checker.CheckDependenciesAsync(); // Check system dependencies first
        var status = _checker.IsUpowerAvailable ? await _checker.GetBatteryAsync() : new BatteryInfo { State = "upower not found" };
        var profile = _checker.IsPowerProfilesCtlAvailable ? await _checker.GetCurrentPowerProfileAsync() : "powerprofilesctl not found";

        // Enable/disable power profile buttons based on availability
        if (this.FindControl<Button>("PowerSaverButton") is Button powerSaverBtn)
        {
            powerSaverBtn.IsEnabled = _checker.IsPowerProfilesCtlAvailable;
        }
        if (this.FindControl<Button>("BalancedButton") is Button balancedBtn)
        {
            balancedBtn.IsEnabled = _checker.IsPowerProfilesCtlAvailable;
        }
        if (this.FindControl<Button>("PerformanceButton") is Button performanceBtn)
        {
            performanceBtn.IsEnabled = _checker.IsPowerProfilesCtlAvailable;
        }

        if (this.FindControl<TextBlock>("PercentageText") is TextBlock pct)
        {
            pct.Text = status.Percentage >= 0 ? $"{status.Percentage}%" : "N/A";
        }

        if (this.FindControl<TextBlock>("StateText") is TextBlock state)
        {
            state.Text = status.State;
        }

        if (this.FindControl<TextBlock>("TimeText") is TextBlock time && !string.IsNullOrEmpty(status.State))
        {
            time.Text = status.State.Contains("discharging", StringComparison.OrdinalIgnoreCase)
                ? $"~{status.TimeToEmpty} left"
                : status.State.Contains("charging", StringComparison.OrdinalIgnoreCase)
                    ? $"~{status.TimeToFull} to full"
                    : "N/A";
        }

        if (this.FindControl<TextBlock>("PowerText") is TextBlock power)
        {
            power.Text = status.EnergyRate >= 0 ? $"{status.EnergyRate:F1} W" : "N/A";
        }

        if (this.FindControl<TextBlock>("ProfileText") is TextBlock profileText)
        {
            profileText.Text = profile;
        }
    }

    private void Refresh_Click(object? sender, RoutedEventArgs e)
    {
        _ = RefreshBatteryDataAsync();  // Async call to refresh battery data
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async void SetPowerSaver_Click(object? sender, RoutedEventArgs e)
    {
        if (_checker.IsPowerProfilesCtlAvailable)
        {
            await _changer.SetPowerProfileAsync("power-saver");
            await RefreshBatteryDataAsync(); // Refresh to show updated profile
        }
    }

    private async void SetBalanced_Click(object? sender, RoutedEventArgs e)
    {
        if (_checker.IsPowerProfilesCtlAvailable)
        {
            await _changer.SetPowerProfileAsync("balanced");
            await RefreshBatteryDataAsync(); // Refresh to show updated profile
        }
    }

    private async void SetPerformance_Click(object? sender, RoutedEventArgs e)
    {
        if (_checker.IsPowerProfilesCtlAvailable)
        {
            await _changer.SetPowerProfileAsync("performance");
            await RefreshBatteryDataAsync(); // Refresh to show updated profile
        }
    }
}