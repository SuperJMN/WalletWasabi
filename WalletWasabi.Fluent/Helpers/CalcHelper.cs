using System.Reactive.Linq;
using NBitcoin;

namespace WalletWasabi.Fluent.Helpers;

public static class CalcHelper
{
	public static IObservable<decimal> UsdBalance(IObservable<Money> btcBalance, IObservable<decimal> btcToUsdExchangeRate)
	{
		return btcBalance.WithLatestFrom(btcToUsdExchangeRate, (money, er) => money.ToDecimal(MoneyUnit.BTC) * er);
	}
}
