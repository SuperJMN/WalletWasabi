using CSharpFunctionalExtensions;

namespace WalletWasabi.Bridge;

public interface IHardwareWallet : IWallet
{
	public IObservable<Result> Display(IAddress address);
}
