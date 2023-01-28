using CSharpFunctionalExtensions;

namespace WalletWasabi.Bridge;

public class HardwareWallet : SoftwareWallet, IHardwareWallet
{
	private readonly IHwiClient _client;

	public HardwareWallet(Wallets.Wallet wallet, IHwiClient client) : base(wallet)
	{
		_client = client;
	}

	public IObservable<Result> Display(IAddress address)
	{
		return _client.Display(address);
	}
}
