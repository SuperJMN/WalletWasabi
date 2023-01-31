using System.Reactive.Linq;
using DynamicData.Binding;
using NBitcoin;
using WalletWasabi.Bridge;
using WalletWasabi.Fluent.Helpers;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;

public class NewWalletBalanceTileViewModel : ActivatableViewModel, INewWalletBalanceTileViewModel
{
	public NewWalletBalanceTileViewModel(IWallet wallet, IExchangeRateProvider rateProvider)
	{
		BalanceBtc = wallet.Balance;
		BalanceFiat = CalcHelper.UsdBalance(wallet.Balance, rateProvider.BtcToUsdRate);
		HasBalance = wallet.Balance.Select(money => money != Money.Zero);
	}

	public IObservable<bool> HasBalance { get; }

	public IObservable<decimal> BalanceFiat { get; }

	public IObservable<Money> BalanceBtc { get; }
}
