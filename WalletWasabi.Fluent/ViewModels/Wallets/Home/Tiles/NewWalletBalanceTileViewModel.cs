using System.Reactive.Linq;
using DynamicData.Binding;
using NBitcoin;
using WalletWasabi.Bridge;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;

public interface INewWalletBalanceTileViewModel
{
	IObservable<bool> HasBalance { get; }
	IObservable<decimal> BalanceFiat { get; }
	IObservable<Money> BalanceBtc { get; }
}

public class NewWalletBalanceTileViewModel : ActivatableViewModel, INewWalletBalanceTileViewModel
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
