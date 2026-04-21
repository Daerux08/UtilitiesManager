using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace UtilitiesManager;

public partial class EnterPasswordPopup : Window
{
    private string _deviceName = "";
    private string _promptText = "password";
    private string _buttonText = "Connect";

    public EnterPasswordPopup()
    {
        InitializeComponent();
    }

    public EnterPasswordPopup(string deviceName) : this()
    {
        _deviceName = deviceName;
        InitLabel();
    }

    public EnterPasswordPopup(string deviceName, string promptText, string buttonText) : this()
    {
        _deviceName = deviceName;
        _promptText = promptText;
        _buttonText = buttonText;
        InitLabel();
        UpdateButtonText();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitLabel()
    {
        var label = this.FindControl<TextBlock>("SSIDLabel");
        if (label != null)
        {
            label.Text = $"Enter {_promptText} for \"{_deviceName}\":";
        }

        var textBox = this.FindControl<TextBox>("PasswordTextBox");
        if (textBox != null)
        {
            textBox.Focus();
        }
    }

    private void UpdateButtonText()
    {
        var button = this.FindControl<Button>("ConnectButton");
        if (button != null)
        {
            button.Content = _buttonText;
        }
    }

    private void ConnectButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var passwordTextBox = this.FindControl<TextBox>("PasswordTextBox");
        string password = passwordTextBox?.Text ?? "";
        
        this.Close(password);
    }

    // Static helpers for common use cases
    public static EnterPasswordPopup ForWiFi(string ssid)
    {
        return new EnterPasswordPopup(ssid, "password", "Connect");
    }

    public static EnterPasswordPopup ForBluetooth(string deviceName)
    {
        return new EnterPasswordPopup(deviceName, "PIN or passkey", "Pair");
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
