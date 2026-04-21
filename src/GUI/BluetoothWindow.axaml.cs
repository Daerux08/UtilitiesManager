using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UtilitiesManager.ViewModels;

namespace UtilitiesManager;

public partial class BluetoothWindow : Window
{
    private BluetoothWindowViewModel? ViewModel => DataContext as BluetoothWindowViewModel;

    public BluetoothWindow()
    {
        InitializeComponent();
        var viewModel = new BluetoothWindowViewModel();
        DataContext = viewModel;
        viewModel.CloseRequested += (s, e) => Close();
        viewModel.PinRequested += OnPinRequested;
    }

    private async void OnPinRequested(object? sender, string deviceName)
    {
        if (ViewModel == null) return;

        var popup = EnterPasswordPopup.ForBluetooth(deviceName);
        var pin = await popup.ShowDialog<string?>(this);

        if (!string.IsNullOrEmpty(pin))
        {
            await ViewModel.PairWithPin(deviceName, pin);
        }
        else
        {
            ViewModel.StatusText = "Pairing cancelled - no PIN provided";
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void BluetoothDataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (ViewModel?.SelectedDevice != null)
        {
            // If device is connected, disconnect. Otherwise try to connect.
            if (ViewModel.SelectedDevice.Connected)
            {
                ViewModel.DisconnectCommand.Execute(null);
            }
            else
            {
                ViewModel.ConnectCommand.Execute(null);
            }
        }
    }
}
