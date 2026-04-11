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

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}