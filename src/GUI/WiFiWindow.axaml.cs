using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UtilitiesManager.ViewModels;

namespace UtilitiesManager;

public partial class WiFiWindow : Window
{
    private WiFiWindowViewModel? ViewModel => DataContext as WiFiWindowViewModel;

    public WiFiWindow()
    {
        InitializeComponent();
        var viewModel = new WiFiWindowViewModel();
        DataContext = viewModel;
        viewModel.CloseRequested += (s, e) => Close();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void WiFiDataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (ViewModel?.SelectedNetwork != null)
        {
            ViewModel.ConnectCommand.Execute(null);
        }
    }
}
