using Avalonia;
using Avalonia.Controls;
using System.ComponentModel;
using Avalonia.Interactivity;
using System.Runtime.CompilerServices;
using System.Globalization;
using Avalonia.Data.Converters;
using System;

namespace UtilitiesManager;

public class BoolToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string param)
        {
            var parts = param.Split('|');
            return boolValue ? parts[1] : parts[0];
        }
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly CheckDependencyCommand _checker = new();
    private readonly ChangeValueCommand _changer = new();
    private int _soundLevel;
    private int _brightness;
    private string _soundLevelText = "";
    private string _brightnessText = "";
    private bool _brightnessAvailable;
    private bool _soundAvailable;
    private bool _batteryAvailable;
    private bool _wifiAvailable;

    public int SoundLevel
    {
        get => _soundLevel;
        set
        {
            if (_soundLevel != value)
            {
                _soundLevel = value;
                SoundLevelText = value.ToString();
                if (_soundAvailable)
                    Task.Run(async () => await _changer.SetVolumeAsync(value)); // Sets the system volume
                OnPropertyChanged();
            }
        }
    }

    public string SoundLevelText
    {
        get => _soundLevelText;
        set
        {
            if (_soundLevelText != value)
            {
                _soundLevelText = value;
                if (int.TryParse(value, out int newValue) && newValue >= 0 && newValue <= 100)
                {
                    SoundLevel = newValue;
                }
                OnPropertyChanged();
            }
        }
    }

    public int Brightness
    {
        get => _brightness;
        set
        {
            if (_brightness != value)
            {
                _brightness = value;
                BrightnessText = value.ToString();
                if (_brightnessAvailable)
                    Task.Run(async () => await _changer.SetBrightnessAsync(value)); // Sets the screen brightness
                OnPropertyChanged();
            }
        }
    }

    public string BrightnessText
    {
        get => _brightnessText;
        set
        {
            if (_brightnessText != value)
            {
                _brightnessText = value;
                if (int.TryParse(value, out int newValue) && newValue >= 0 && newValue <= 100)
                {
                    Brightness = newValue;
                }
                OnPropertyChanged();
            }
        }
    }

    public bool BrightnessAvailable
    {
        get => _brightnessAvailable;
        set
        {
            _brightnessAvailable = value;
            OnPropertyChanged();
        }
    }

    public bool SoundAvailable
    {
        get => _soundAvailable;
        set
        {
            _soundAvailable = value;
            OnPropertyChanged();
        }
    }

    public bool WiFiAvailable
    {
        get => _wifiAvailable;
        set
        {
            _wifiAvailable = value;
            OnPropertyChanged();
        }
    }

    public bool BatteryAvailable
    {
        get => _batteryAvailable;
        set
        {
            _batteryAvailable = value;
            OnPropertyChanged();
        }
    }

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        DataContext = this;
        InitializeValues();
    }

    private async void InitializeValues()
    {
        try
        {
            await _checker.LoadOriginalValuesAsync();
            SoundLevel = _checker.OriginalValueSound;
            Brightness = _checker.OriginalValueLight;
            SoundLevelText = SoundLevel.ToString();
            BrightnessText = Brightness.ToString();
            BrightnessAvailable = _checker.IsBrightnessCtlAvailable;
            SoundAvailable = _checker.IsPactlAvailable;
            BatteryAvailable = _checker.IsUpowerAvailable;
            WiFiAvailable = _checker.IsNmcliAvailable;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing values: {ex.Message}");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Button click handler to open Battery window
    private void OpenBattery_Click(object? sender, RoutedEventArgs e)
    {
        if (!BatteryAvailable)
            return; // Button is disabled when unavailable
        var batteryWindow = new BatteryWindow();
        batteryWindow.Show(this);
    }

    // Button click handler to open Wi-Fi window
    private void OpenWiFi_Click(object? sender, RoutedEventArgs e)
    {
        if (!WiFiAvailable)
            return; // Button is disabled when unavailable
        var wifiWindow = new WiFiWindow();
        wifiWindow.Show(this);
    }
}