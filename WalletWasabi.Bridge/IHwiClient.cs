using CSharpFunctionalExtensions;

namespace WalletWasabi.Bridge;

public interface IHwiClient
{
	public IObservable<Result> Display(IAddress address);
}
