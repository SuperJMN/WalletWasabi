using System.Reactive;

namespace WalletWasabi.Bridge;

public class HardwareWallet : Wallet, IHardwareWallet
{
	public HardwareWallet(Wallets.Wallet wallet) : base(wallet)
	{
	}

	public IObservable<Unit> Display(string address)
	{
		// TODO:
		throw new NotImplementedException();
	}
}
