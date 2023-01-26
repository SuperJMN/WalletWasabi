using CSharpFunctionalExtensions;
using WalletWasabi.Bridge;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;

namespace WalletWasabi.Fluent.ViewModels;

public static class ViewModelLocator
{
	public static RoutableViewModel CreateReceiveAddressHostViewModel(IWallet wallet, IAddress newAddress)
	{
		var ra = new ReceiveAddressViewModel(newAddress, new QrGenerator());

		var hw = Maybe
			.From(wallet as IHardwareWallet)
			.Map(_ => new HardwareWalletViewModel(newAddress, new HardwareInterfaceClient()));

		return new ReceiveAddressHostViewModel(ra, hw);
	}
}
