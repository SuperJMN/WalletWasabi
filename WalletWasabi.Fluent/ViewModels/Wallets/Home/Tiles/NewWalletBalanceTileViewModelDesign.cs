using System.Reactive.Linq;
using NBitcoin;
using ReactiveMarbles.PropertyChanged;
using ReactiveUI;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;

public partial class NewWalletBalanceTileViewModelDesign : ReactiveObject, INewWalletBalanceTileViewModel
{
	[AutoNotify] private decimal _exchangeRate;
	[AutoNotify] private decimal _balanceBtcValue;

	public NewWalletBalanceTileViewModelDesign()
	{
		BalanceBtc = this.WhenChanged(x => x.BalanceBtcValue).Select(Money.Coins);
		BalanceFiat = BalanceBtc.WithLatestFrom(this.WhenChanged(x => x.ExchangeRate), (money, er) => money.ToDecimal(MoneyUnit.BTC) * er);
		HasBalance = BalanceBtc.Select(x => x != Money.Zero);
	}

	public IObservable<bool> HasBalance { get; }
	public IObservable<decimal> BalanceFiat { get; }
	public IObservable<Money> BalanceBtc { get; }
}
