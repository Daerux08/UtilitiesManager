using Avalonia.Controls;
using Avalonia.Diagnostics;
using UtilitiesManager.ViewModels;

namespace UtilitiesManager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}