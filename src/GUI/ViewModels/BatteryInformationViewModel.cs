using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UtilitiesManager.ViewModels
{
    public class BatteryInformationViewModel : BaseViewModel
    {
        private readonly CheckDependencyCommand _checker = new();
        private readonly ChangeValueCommand _changer = new();

        private BatteryInfo _batteryInfo = new();
        private System.Collections.ObjectModel.ObservableCollection<string> _availableProfiles = new();
        private string _selectedProfile = "";

        public BatteryInfo BatteryInfo
        {
            get => _batteryInfo;
            set => SetProperty(ref _batteryInfo, value);
        }

        public string Model => BatteryInfo?.Model ?? "";
        public string Vendor => BatteryInfo?.Vendor ?? "";
        public string CapacityText => BatteryInfo?.Capacity >= 0 ? $"{BatteryInfo.Capacity}%" : "N/A";

        public string PercentageText => BatteryInfo.Percentage >= 0 ? $"{BatteryInfo.Percentage}%" : "N/A";
        public string StateText => BatteryInfo.State;
        public string TimeText => GetTimeText();
        public string PowerText => BatteryInfo.EnergyRate >= 0 ? $"{BatteryInfo.EnergyRate:F1} W" : "N/A";

        public System.Collections.ObjectModel.ObservableCollection<string> AvailableProfiles
        {
            get => _availableProfiles;
            set => SetProperty(ref _availableProfiles, value);
        }

        // Read-only in the information window — changes should be made from the main Battery window
        public string SelectedProfile
        {
            get => _selectedProfile;
            private set => SetProperty(ref _selectedProfile, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand CloseCommand { get; }

        public BatteryInformationViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh);
            CloseCommand = new RelayCommand(Close);

            _ = RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                _checker.CheckDependencies();
                BatteryInfo = _checker.IsUpowerAvailable ? _checker.GetBattery() : new BatteryInfo { State = "upower not found" };

                // Populate profiles if available
                if (_checker.IsPowerProfilesCtlAvailable)
                {
                    var current = await _checker.GetCurrentPowerProfileAsync();
                    AvailableProfiles = new System.Collections.ObjectModel.ObservableCollection<string>(new[] { "power-saver", "balanced", "performance" });
                    SelectedProfile = current ?? "";
                }

                OnPropertyChanged(nameof(Model));
                OnPropertyChanged(nameof(Vendor));
                OnPropertyChanged(nameof(CapacityText));
                OnPropertyChanged(nameof(PercentageText));
                OnPropertyChanged(nameof(StateText));
                OnPropertyChanged(nameof(TimeText));
                OnPropertyChanged(nameof(PowerText));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing battery information: {ex.Message}");
            }
        }

        private void Refresh()
        {
            _ = RefreshAsync();
        }

        private async Task SetPowerProfileAsync(string profile)
        {
            try
            {
                if (_checker.IsPowerProfilesCtlAvailable)
                {
                    await _changer.SetPowerProfileAsync(profile);
                    await RefreshAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting power profile: {ex.Message}");
            }
        }

        private string GetTimeText()
        {
            if (string.IsNullOrEmpty(BatteryInfo.State))
                return "N/A";

            if (BatteryInfo.IsDischarging)
                return $"~{BatteryInfo.TimeToEmpty} left";

            if (BatteryInfo.IsCharging)
                return $"~{BatteryInfo.TimeToFull} to full";

            return "N/A";
        }

        private void Close()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CloseRequested;
    }
}
