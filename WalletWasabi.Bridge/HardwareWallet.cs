using System.Reactive;
using WalletWasabi.Blockchain.Keys;

namespace WalletWasabi.Bridge;

public class HardwareWallet : SoftwareWallet, IHardwareWallet
{
	private readonly IHwiClient _client;

	public HardwareWallet(Wallets.Wallet wallet, IHwiClient client) : base(wallet)
	{
		_client = client;
	}

	public IObservable<Unit> Display(string address)
	{
		// TODO:
		throw new NotImplementedException();
	}
}
