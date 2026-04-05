using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UtilitiesManager.ViewModels
{
    public class BatteryWindowViewModel : BaseViewModel
    {
        private readonly CheckDependencyCommand _checker = new();
        private readonly ChangeValueCommand _changer = new();
        
        private BatteryInfo _batteryInfo = new();
        private string _currentProfile = "";
        private bool _powerProfilesAvailable;

        public BatteryInfo BatteryInfo
        {
            get => _batteryInfo;
            set => SetProperty(ref _batteryInfo, value);
        }

        public string CurrentProfile
        {
            get => _currentProfile;
            set => SetProperty(ref _currentProfile, value);
        }

        public bool PowerProfilesAvailable
        {
            get => _powerProfilesAvailable;
            set => SetProperty(ref _powerProfilesAvailable, value);
        }

        public string PercentageText => BatteryInfo.Percentage >= 0 ? $"{BatteryInfo.Percentage}%" : "N/A";
        public string StateText => BatteryInfo.State;
        public string TimeText => GetTimeText();
        public string PowerText => BatteryInfo.EnergyRate >= 0 ? $"{BatteryInfo.EnergyRate:F1} W" : "N/A";

        public ICommand RefreshCommand { get; }
        public ICommand SetPowerSaverCommand { get; }
        public ICommand SetBalancedCommand { get; }
        public ICommand SetPerformanceCommand { get; }
        public ICommand CloseCommand { get; }

        public BatteryWindowViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh);
            SetPowerSaverCommand = new RelayCommand(async () => await SetPowerProfile("power-saver"), () => PowerProfilesAvailable);
            SetBalancedCommand = new RelayCommand(async () => await SetPowerProfile("balanced"), () => PowerProfilesAvailable);
            SetPerformanceCommand = new RelayCommand(async () => await SetPowerProfile("performance"), () => PowerProfilesAvailable);
            CloseCommand = new RelayCommand(Close);

            _ = RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                await CheckDependencyCommand.CheckDependenciesAsync();
                BatteryInfo = _CheckDependencyCommand.IsUpowerAvailable ? await CheckDependencyCommand.GetBatteryAsync() : new BatteryInfo { State = "upower not found" };
                CurrentProfile = _CheckDependencyCommand.IsPowerProfilesCtlAvailable ? await CheckDependencyCommand.GetCurrentPowerProfileAsync() : "powerprofilesctl not found";
                PowerProfilesAvailable = _CheckDependencyCommand.IsPowerProfilesCtlAvailable;

                OnPropertyChanged(nameof(PercentageText));
                OnPropertyChanged(nameof(StateText));
                OnPropertyChanged(nameof(TimeText));
                OnPropertyChanged(nameof(PowerText));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing battery data: {ex.Message}");
            }
        }

        private void Refresh()
        {
            _ = RefreshAsync();
        }

        private async Task SetPowerProfile(string profile)
        {
            try
            {
                if (PowerProfilesAvailable)
                {
                    await _changer.SetPowerProfileAsync(profile);
                    await RefreshAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting {profile} profile: {ex.Message}");
            }
        }

        private string GetTimeText()
        {
            if (string.IsNullOrEmpty(BatteryInfo.State))
                return "N/A";

            if (BatteryInfo.State.Contains("discharging", StringComparison.OrdinalIgnoreCase))
                return $"~{BatteryInfo.TimeToEmpty} left";
            
            if (BatteryInfo.State.Contains("charging", StringComparison.OrdinalIgnoreCase))
                return $"~{BatteryInfo.TimeToFull} to full";
            
            return "N/A";
        }

        private void Close()
        {
            // This will be handled by the View
        }
    }
}
