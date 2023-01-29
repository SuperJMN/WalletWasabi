using System.Reactive.Linq;
using ReactiveUI;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive.Address;

[NavigationMetaData(Title = "Receive Address")]
public partial class HardwareAddressViewModel : RoutableViewModel
{
	public HardwareAddressViewModel(HardwareWalletAddressViewModel hardwareWalletAddressViewModel, ReceiveAddressViewModel receiveAddress)
	{
		EnableBack = true;
		ReceiveAddress = receiveAddress;
		HardwareWallet = hardwareWalletAddressViewModel;
		var canExecute = hardwareWalletAddressViewModel.IsBusy.Select(b => !b);
		CancelCommand = ReactiveCommand.Create(() => Navigate().Clear(), canExecute);
		NextCommand = CancelCommand;
		SetupCancel(enableCancel: false, enableCancelOnEscape: true, enableCancelOnPressed: true);
	}

	public ReceiveAddressViewModel ReceiveAddress { get; }
	public HardwareWalletAddressViewModel HardwareWallet { get; }
}
