using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UtilitiesManager.ViewModels;

namespace UtilitiesManager;

public partial class WiFiWindow : Window
{
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
}
