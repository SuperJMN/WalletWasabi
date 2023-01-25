using System.Reactive;

namespace WalletWasabi.Bridge;

public interface IHardwareWallet : IWallet
{
	public IObservable<Unit> Display(string address);
}
