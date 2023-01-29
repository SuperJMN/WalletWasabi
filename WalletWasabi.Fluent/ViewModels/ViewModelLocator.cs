using WalletWasabi.Bridge;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive.Address;

namespace WalletWasabi.Fluent.ViewModels;

public static class ViewModelLocator
{
	public static RoutableViewModel CreateAddressViewModel(IWallet wallet, IAddress address)
	{
		if (wallet is IHardwareWallet hw)
		{
			return new HardwareAddressViewModel(new HardwareWalletAddressViewModel(address, hw), new ReceiveAddressViewModel(address, new QrGenerator()));
		}

		return new SoftwareAddressViewModel(address, new QrGenerator());
	}
}
