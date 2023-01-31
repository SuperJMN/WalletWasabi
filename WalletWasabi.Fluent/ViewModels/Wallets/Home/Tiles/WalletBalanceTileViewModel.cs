using System.Reactive.Linq;
using NBitcoin;
using WalletWasabi.Bridge;
using WalletWasabi.Fluent.Extensions;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;

public class WalletBalanceTileViewModel : ActivatableViewModel, IWalletBalanceTileViewModel
{
	public WalletBalanceTileViewModel(IWallet wallet, IExchangeRateProvider rateProvider)
	{
		BalanceBtc = wallet.Balance;
		BalanceFiat = wallet.Balance.ToUsd(rateProvider.BtcToUsdRate);
		HasBalance = wallet.Balance.Select(money => money != Money.Zero);
	}

	public IObservable<bool> HasBalance { get; }

	public IObservable<decimal> BalanceFiat { get; }

	public IObservable<Money> BalanceBtc { get; }
}
