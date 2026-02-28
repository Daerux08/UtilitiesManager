using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace UtilitiesManager;

public partial class EnterPasswordPopup : Window
{
    private string _ssid = "";

    public EnterPasswordPopup()
    {
        InitializeComponent();
    }

    public EnterPasswordPopup(string ssid) : this()
    {
        _ssid = ssid;
        InitSSIDLabel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitSSIDLabel()
    {
        var label = this.FindControl<TextBlock>("SSIDLabel");
        if (label != null)
        {
            label.Text = $"Enter password for \"{_ssid}\":";
        }

        var textBox = this.FindControl<TextBox>("PasswordTextBox");
        if (textBox != null)
        {
            textBox.Focus();
        }
    }

    private void ConnectButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var passwordTextBox = this.FindControl<TextBox>("PasswordTextBox");
        string password = passwordTextBox?.Text ?? "";
        
        this.Close(password);
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close((string?)null);
    }

    private void PasswordTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            ConnectButton_Click(null, new Avalonia.Interactivity.RoutedEventArgs());
        }
    }
}
