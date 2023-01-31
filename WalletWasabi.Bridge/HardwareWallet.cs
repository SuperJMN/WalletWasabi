using CSharpFunctionalExtensions;

namespace WalletWasabi.Bridge;

public class HardwareWallet : ImprovedWallet, IHardwareWallet
{
	private readonly IHardwareInterfaceClient _client;

	public HardwareWallet(Wallets.Wallet wallet, IHardwareInterfaceClient client) : base(wallet)
	{
		_client = client;
	}

	public IObservable<Result> Display(IAddress address)
	{
		return _client.Display(address);
	}
}
