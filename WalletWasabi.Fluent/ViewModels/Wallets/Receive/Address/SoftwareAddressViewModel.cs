using ReactiveUI;
using WalletWasabi.Bridge;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive.Address;

[NavigationMetaData(Title = "Receive Address")]
public partial class SoftwareAddressViewModel : RoutableViewModel
{
	public SoftwareAddressViewModel(IAddress address, IQrCodeGenerator qrCodeGenerator)
	{
		ReceiveAddress = new ReceiveAddressViewModel(address, qrCodeGenerator);
		CancelCommand = ReactiveCommand.Create(() => Navigate().Clear());
		NextCommand = CancelCommand;
		SetupCancel(enableCancel: false, enableCancelOnEscape: true, enableCancelOnPressed: true);
	}

	public ReceiveAddressViewModel ReceiveAddress { get; }
}
