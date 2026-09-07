using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UtilitiesManager.ViewModels;

namespace UtilitiesManager;

public partial class BatteryInformationWindow : Window
{
    public BatteryInformationWindow()
    {
        InitializeComponent();
        var vm = new BatteryInformationViewModel();
        DataContext = vm;
        vm.CloseRequested += (s, e) => Close();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
