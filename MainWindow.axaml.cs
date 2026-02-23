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
    private bool _brightnessAvailable;
    private bool _soundAvailable;
    private bool _batteryAvailable;

    public int SoundLevel
    {
        get => _soundLevel;
        set
        {
            if (_soundLevel != value)
            {
                _soundLevel = value;
                if (_soundAvailable)
                    _ = _changer.SetVolumeAsync(value); // ← now actually sets volume
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
                if (_brightnessAvailable)
                    _ = _changer.SetBrightnessAsync(value); // ← now actually sets brightness
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
        await _checker.LoadOriginalValuesAsync();
        SoundLevel = _checker.OriginalValueSound;
        Brightness = _checker.OriginalValueLight;
        BrightnessAvailable = _checker.IsBrightnessCtlAvailable;
        SoundAvailable = _checker.IsPactlAvailable;
        BatteryAvailable = _checker.IsUpowerAvailable;
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // ← New shit: button click opens Battery window
    private void OpenBattery_Click(object? sender, RoutedEventArgs e)
    {
        if (!BatteryAvailable)
            return; // Or show a message, but since button will be disabled, maybe not needed
        var batteryWindow = new BatteryWindow();
        batteryWindow.Show(this);                   // 5
        // batteryWindow.ShowDialog(this);          // ← uncomment if you want modal (blocks main window)
    }
}