using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Blockchain.TransactionOutputs;
using WalletWasabi.Fluent.Extensions;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;
using WalletWasabi.Fluent.ViewModels.Wallets;
using WalletWasabi.Fluent.ViewModels.Wallets.Advanced.WalletCoins;
using WalletWasabi.Fluent.ViewModels.Wallets.Send;

namespace WalletWasabi.Fluent.ViewModels.CoinSelection;

[NavigationMetaData(
	Title = "Select Coins",
	Caption = "",
	IconName = "wallet_action_send",
	NavBarPosition = NavBarPosition.None,
	Searchable = false,
	NavigationTarget = NavigationTarget.DialogScreen)]
public partial class SelectCoinsDialogViewModel : DialogViewModelBase<IEnumerable<SmartCoin>>
{
	private readonly IObservable<Unit> _balanceChanged;
	private readonly IEnumerable<SmartCoin>? _usedCoins;
	private readonly WalletViewModel _walletViewModel;
	[AutoNotify] private CoinBasedSelectionViewModel? _coinBasedSelection;
	[AutoNotify] private IObservable<bool> _enoughSelected = Observable.Return(false);
	[AutoNotify] private LabelBasedCoinSelectionViewModel? _labelBasedSelection;
	[AutoNotify] private IObservable<Money> _remainingAmount = Observable.Return(Money.Zero);
	[AutoNotify] private IObservable<Money> _selectedAmount = Observable.Return(Money.Zero);
	[AutoNotify] private IObservable<int> _selectedCount = Observable.Return(0);
	private ReadOnlyObservableCollection<WalletCoinViewModel> _collection;

	public SelectCoinsDialogViewModel(
		WalletViewModel walletViewModel,
		TransactionInfo transactionInfo,
		IObservable<Unit> balanceChanged,
		IEnumerable<SmartCoin>? usedCoins)
	{
		_walletViewModel = walletViewModel;
		_balanceChanged = balanceChanged;
		_usedCoins = usedCoins;
		TargetAmount = transactionInfo.MinimumRequiredAmount == Money.Zero
			? transactionInfo.Amount
			: transactionInfo.MinimumRequiredAmount;

		SetupCancel(enableCancel: false, enableCancelOnEscape: true, enableCancelOnPressed: false);
		EnableBack = true;
	}

	public Money TargetAmount { get; }

	private new ReactiveCommand<Unit, IEnumerable<WalletCoinViewModel>> NextCommand { get; set; }

	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
		var sourceCache = new SourceCache<WalletCoinViewModel, int>(x => x.GetHashCode());

		var coinLists = GetCoins(_balanceChanged);

		sourceCache.RefillFrom(coinLists)
			.DisposeWith(disposables);

		var coinChanges = sourceCache.Connect();

		var viewModels = coinChanges
			.Replay()
			.RefCount();

		viewModels.Bind(out _collection)
			.Subscribe();
		
		var selectedCoins = viewModels
			.AutoRefresh(x => x.IsSelected)
			.ToCollection()
			.Select(items => items.Where(t => t.IsSelected));

		EnoughSelected = selectedCoins
			.Select(coins => coins.Sum(x => x.Amount) >= TargetAmount);

		SelectedAmount = selectedCoins
			.Select(Sum);

		RemainingAmount = SelectedAmount
			.Select(money => Money.Max(TargetAmount - money, Money.Zero));

		SelectedCount = selectedCoins
			.Select(models => models.Count());

		CoinBasedSelection = new CoinBasedSelectionViewModel(viewModels).DisposeWith(disposables);
		LabelBasedSelection = new LabelBasedCoinSelectionViewModel(viewModels).DisposeWith(disposables);

		NextCommand = ReactiveCommand.CreateFromObservable(() => selectedCoins, EnoughSelected);
		NextCommand.Subscribe(models => Close(DialogResultKind.Normal, models.Select(x => x.Coin)));

		base.OnNavigatedTo(isInHistory, disposables);
	}

	private static Money Sum(IEnumerable<WalletCoinViewModel> coinViewModels)
	{
		return coinViewModels.Sum(coinViewModel => coinViewModel.Coin.Amount);
	}

	private IObservable<IEnumerable<WalletCoinViewModel>> GetCoins(IObservable<Unit> balanceChanged)
	{
		var initial = Observable.Return(GetCoinsFromWallet());
		var coinJoinChanged = _walletViewModel.WhenAnyValue(model => model.IsCoinJoining);
		var coinsChanged = balanceChanged.ToSignal().Merge(coinJoinChanged.ToSignal());
		var coins = coinsChanged.Select(_ => GetCoinsFromWallet());
		var concat = initial.Concat(coins);

		return concat;
	}

	private IEnumerable<WalletCoinViewModel> GetCoinsFromWallet()
	{
		return _walletViewModel.Wallet.Coins.ToList().Select(x => new WalletCoinViewModel(x));
	}
}
