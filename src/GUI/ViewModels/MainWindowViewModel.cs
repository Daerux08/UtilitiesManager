using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;

namespace UtilitiesManager.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
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
                if (SetProperty(ref _soundLevel, value))
                {
                    SoundLevelText = value.ToString();
                    if (_soundAvailable)
                        _changer.SetVolumeAsync(value).ConfigureAwait(false);
                }
            }
        }

        public string SoundLevelText
        {
            get => _soundLevelText;
            set
            {
                if (SetProperty(ref _soundLevelText, value))
                {
                    if (int.TryParse(value, out int newValue) && newValue >= 0 && newValue <= 100)
                    {
                        SoundLevel = newValue;
                    }
                }
            }
        }

        public int Brightness
        {
            get => _brightness;
            set
            {
                if (SetProperty(ref _brightness, value))
                {
                    BrightnessText = value.ToString();
                    if (_brightnessAvailable)
                        _changer.SetBrightnessAsync(value).ConfigureAwait(false);
                }
            }
        }

        public string BrightnessText
        {
            get => _brightnessText;
            set
            {
                if (SetProperty(ref _brightnessText, value))
                {
                    if (int.TryParse(value, out int newValue) && newValue >= 0 && newValue <= 100)
                    {
                        Brightness = newValue;
                    }
                }
            }
        }

        public bool BrightnessAvailable
        {
            get => _brightnessAvailable;
            set => SetProperty(ref _brightnessAvailable, value);
        }

        public bool SoundAvailable
        {
            get => _soundAvailable;
            set => SetProperty(ref _soundAvailable, value);
        }

        public bool WiFiAvailable
        {
            get => _wifiAvailable;
            set 
            { 
                if (SetProperty(ref _wifiAvailable, value))
                {
                    ((RelayCommand)OpenWiFiCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool BatteryAvailable
        {
            get => _batteryAvailable;
            set 
            { 
                if (SetProperty(ref _batteryAvailable, value))
                {
                    ((RelayCommand)OpenBatteryCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OpenBatteryCommand { get; }
        public ICommand OpenWiFiCommand { get; }

        public MainWindowViewModel()
        {
            OpenBatteryCommand = new RelayCommand(OpenBattery, () => BatteryAvailable);
            OpenWiFiCommand = new RelayCommand(OpenWiFi, () => WiFiAvailable);
            
            _ = InitializeValuesAsync();
        }

        private async Task InitializeValuesAsync()
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

        private void OpenBattery()
        {
            if (BatteryAvailable)
            {
                var batteryWindow = new BatteryWindow();
                batteryWindow.Show();
            }
        }

        private void OpenWiFi()
        {
            if (WiFiAvailable)
            {
                var wifiWindow = new WiFiWindow();
                wifiWindow.Show();
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
