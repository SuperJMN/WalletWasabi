using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Blockchain.TransactionOutputs;
using WalletWasabi.Fluent.Extensions;
using WalletWasabi.Fluent.Helpers;
using WalletWasabi.Fluent.ViewModels.CoinSelection.Core;
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
	private readonly IEnumerable<SmartCoin> _usedCoins;
	private readonly WalletViewModel _walletViewModel;
	[AutoNotify] private ReactiveCommand<Unit, Unit> _clearCoinSelectionCommand = ReactiveCommand.Create(() => { });
	[AutoNotify] private CoinBasedSelectionViewModel? _coinBasedSelection;
	[AutoNotify] private IObservable<bool> _enoughSelected = Observable.Return(false);
	[AutoNotify] private IObservable<bool> _isSelectionBadlyChosen = Observable.Return(false);
	[AutoNotify] private LabelBasedCoinSelectionViewModel? _labelBasedSelection;
	[AutoNotify] private IObservable<Money> _remainingAmount = Observable.Return(Money.Zero);
	[AutoNotify] private ReactiveCommand<Unit, Unit> _selectAllCoinsCommand = ReactiveCommand.Create(() => { });
	[AutoNotify] private ReactiveCommand<Unit, Unit> _selectAllPrivateCoinsCommand = ReactiveCommand.Create(() => { });
	[AutoNotify] private IObservable<Money> _selectedAmount = Observable.Return(Money.Zero);
	[AutoNotify] private IObservable<int> _selectedCount = Observable.Return(0);
	[AutoNotify] private ReactiveCommand<Unit, Unit> _selectPredefinedCoinsCommand = ReactiveCommand.Create(() => { });
	[AutoNotify] private IObservable<string> _summaryText = Observable.Return("");
	private readonly SmartLabel _transactionLabels;

	public SelectCoinsDialogViewModel(
		WalletViewModel walletViewModel,
		TransactionInfo transactionInfo,
		IObservable<Unit> balanceChanged,
		IEnumerable<SmartCoin> usedCoins)
	{
		_walletViewModel = walletViewModel;
		_balanceChanged = balanceChanged;
		_usedCoins = usedCoins;
		TargetAmount = transactionInfo.MinimumRequiredAmount == Money.Zero
			? transactionInfo.Amount
			: transactionInfo.MinimumRequiredAmount;

		_transactionLabels = transactionInfo.UserLabels;

		SetupCancel(enableCancel: false, enableCancelOnEscape: true, enableCancelOnPressed: false);
		EnableBack = true;
	}

	public Money TargetAmount { get; }

	private new ReactiveCommand<Unit, List<WalletCoinViewModel>> NextCommand { get; set; }

	[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
		var sourceCache = new SourceCache<WalletCoinViewModel, int>(x => x.GetHashCode());
		var coinLists = GetCoins(_balanceChanged)
			.ObserveOn(RxApp.MainThreadScheduler);

		sourceCache.RefillFrom(coinLists)
			.DisposeWith(disposables);

		var viewModels = sourceCache.Connect();

		var selectedCoins = viewModels
			.AutoRefresh(x => x.IsSelected)
			.ToCollection()
			.Select(items => items.Where(t => t.IsSelected).ToList())
			.ReplayLastActive();

		EnoughSelected = selectedCoins.Select(coins => coins.Sum(x => x.Amount) >= TargetAmount);

		IsSelectionBadlyChosen = selectedCoins.Select(IsSelectionBadForPrivacy);

		SelectedAmount = selectedCoins.Select(Sum);

		RemainingAmount = SelectedAmount.Select(money => Money.Max(TargetAmount - money, Money.Zero));

		SelectedCount = selectedCoins.Select(models => models.Count);

		CoinBasedSelection =
			new CoinBasedSelectionViewModel(viewModels)
				.DisposeWith(disposables);
		LabelBasedSelection =
			new LabelBasedCoinSelectionViewModel(viewModels)
				.DisposeWith(disposables);

		SummaryText = RemainingAmount.CombineLatest(
			SelectedCount,
			(remaining, coinList) =>
			{
				var remainingText = remaining == Money.Zero ? "" : $"{remaining.FormattedBtc()} BTC | ";
				var coinCountText = $"{coinList} coin{TextHelpers.AddSIfPlural(coinList)} selected";
				return remainingText + coinCountText;
			});

		NextCommand = ReactiveCommand.CreateFromObservable(() => selectedCoins, EnoughSelected);
		NextCommand.Subscribe(models => Close(DialogResultKind.Normal, models.Select(x => x.Coin)));

		SelectPredefinedCoinsCommand = ReactiveCommand.Create(
			() => sourceCache.Items.ToList().ForEach(x => x.IsSelected = _usedCoins.Any(coin => x.Coin == coin)));

		SelectAllCoinsCommand =
			ReactiveCommand.Create(() => sourceCache.Items.ToList().ForEach(x => x.IsSelected = true));

		ClearCoinSelectionCommand =
			ReactiveCommand.Create(() => sourceCache.Items.ToList().ForEach(x => x.IsSelected = false));

		SelectAllPrivateCoinsCommand = ReactiveCommand.Create(
			() => sourceCache.Items.ToList().ForEach(
				coinViewModel => coinViewModel.IsSelected = coinViewModel.GetPrivacyLevel() == PrivacyLevel.Private));

		SelectPredefinedCoinsCommand.Execute()
			.Subscribe()
			.DisposeWith(disposables);

		base.OnNavigatedTo(isInHistory, disposables);
	}

	private static bool IsSelectionBadForPrivacy(IList<WalletCoinViewModel> selectedCoins, SmartLabel currentLabel)
	{
		if (selectedCoins.All(x => x.SmartLabel == currentLabel))
		{
			return false;
		}

		if (selectedCoins.Any(x => x.AnonymitySet == 1))
		{
			return true;
		}

		if (selectedCoins.GroupBy(x => x.GetPrivacyLevel()).Count() > 1)
		{
			return true;
		}

		if (selectedCoins.GroupBy(x => x.SmartLabel).Count() > 1)
		{
			return true;
		}

		return false;
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
		return _walletViewModel.Wallet.Coins.ToList().Select(x => new WalletCoinViewModel(x, _walletViewModel.Wallet));
	}
}
