using CSharpFunctionalExtensions;
using DynamicData;
using NBitcoin;

namespace WalletWasabi.Bridge;

public interface IWallet
{
	public string Name { get; }
	IObservable<IChangeSet<ITransaction, uint256>> Transactions { get; }
	IEnumerable<Address> Addresses { get; }
	IAddress CreateReceiveAddress(IEnumerable<string> destinationLabels);
	IObservable<Money> Balance { get; }

	// TODO: Can we leave this simple??
	IObservable<Result> Send(Money amount, IAddress destination);
}
