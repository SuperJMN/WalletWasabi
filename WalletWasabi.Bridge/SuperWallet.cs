using System.Reactive.Linq;
using DynamicData;
using NBitcoin;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Wallets;

namespace WalletWasabi.Bridge;

public class SuperWallet : IWallet
{
	private readonly TransactionHistoryBuilder _historyBuilder;
	private readonly Wallet _wallet;

	public SuperWallet(Wallet wallet)
	{
		_wallet = wallet;
		_historyBuilder = new TransactionHistoryBuilder(_wallet);

		Transactions = Summaries
			.ToObservableChangeSet(x => x.TransactionId)
			.Transform(ts => (ITransaction) new Transaction(ts));
	}

	private IObservable<TransactionSummary> Summaries
	{
		get
		{
			var fromEvents = Observable
				.FromEventPattern(_wallet, nameof(_wallet.WalletRelevantTransactionProcessed))
				.SelectMany(_ => BuildSummary());

			var fromInitial = Observable.Defer(() => BuildSummary().ToObservable());

			return fromInitial.Merge(fromEvents);
		}
	}

	public string Name => _wallet.WalletName;
	public IObservable<IChangeSet<ITransaction, uint256>> Transactions { get; }

	public IAddress CreateReceiveAddress(IEnumerable<string> destinationLabels)
	{
		if (_wallet.KeyManager.MasterFingerprint == null)
		{
			throw new InvalidOperationException("Master fingerprint should not be null");
		}

		var hdPubKey = _wallet.KeyManager.GetNextReceiveKey(new SmartLabel(destinationLabels));
		var address = Address.From(hdPubKey.PubKey, hdPubKey.FullKeyPath, hdPubKey.Label, _wallet);
		return address;
	}

	public IEnumerable<Address> Addresses
	{
		get
		{
			return _wallet.KeyManager
				.GetKeys(hdPubKey => !hdPubKey.Label.IsEmpty && !hdPubKey.IsInternal && hdPubKey.KeyState == KeyState.Clean)
				.Select(key => Address.From(key.PubKey, key.FullKeyPath, key.Label, _wallet));
		}
	}

	private IEnumerable<TransactionSummary> BuildSummary()
	{
		return _historyBuilder.BuildHistorySummary();
	}
}
