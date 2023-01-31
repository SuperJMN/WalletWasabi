using NBitcoin;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;

public interface INewWalletBalanceTileViewModel
{
	IObservable<bool> HasBalance { get; }
	IObservable<decimal> BalanceFiat { get; }
	IObservable<Money> BalanceBtc { get; }
}
