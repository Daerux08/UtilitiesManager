using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UtilitiesManager.ViewModels;

namespace UtilitiesManager;

public partial class BatteryWindow : Window
{
    public BatteryWindow()
    {
        InitializeComponent();
        var viewModel = new BatteryWindowViewModel();
        DataContext = viewModel;
        viewModel.CloseRequested += (s, e) => Close();
    }

    private void OpenBatteryInformation(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            var infoWindow = new BatteryInformationWindow();
            infoWindow.Show();
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening BatteryInformationWindow: {ex.Message}");
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}