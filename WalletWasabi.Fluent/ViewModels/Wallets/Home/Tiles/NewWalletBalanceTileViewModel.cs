using System.Reactive.Linq;
using NBitcoin;
using WalletWasabi.Bridge;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;

public class NewWalletBalanceTileViewModel : ActivatableViewModel
{
	public NewWalletBalanceTileViewModel(IWallet wallet, IExchangeRateProvider rateProvider)
	{
		BalanceBtc = wallet.Balance;
		BalanceFiat = wallet.Balance.WithLatestFrom(rateProvider.BtcToUsdRate, (money, ur) => (ulong) money / ur);
		HasBalance = wallet.Balance.Select(money => money != Money.Zero);
	}

	public IObservable<bool> HasBalance { get; }

	public IObservable<decimal> BalanceFiat { get; }

	public IObservable<Money> BalanceBtc { get; }
}
