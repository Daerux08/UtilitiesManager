using Avalonia.Controls;
using UtilitiesManager.ViewModels;

namespace UtilitiesManager;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
    }
}
