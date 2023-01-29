using CSharpFunctionalExtensions;

namespace WalletWasabi.Bridge;

public interface IHardwareInterfaceClient
{
	public IObservable<Result> Display(IAddress address);
}
