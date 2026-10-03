using System.Windows.Input;

namespace UtilitiesManager.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		private readonly EnterBIOSCommand _enterBIOSCommand = new();

		public ICommand EnterBIOSCommand { get; }

		public SettingsViewModel()
		{
			EnterBIOSCommand = new RelayCommand(async () => await _enterBIOSCommand.EnterBIOSAsync());
		}
	}
}