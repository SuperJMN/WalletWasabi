using NBitcoin;
using WalletWasabi.Fluent.Helpers;

namespace WalletWasabi.Fluent.ViewModels.Wallets;

public class WalletBalances
{
	public WalletBalances(IObservable<Money> btcBalance, IObservable<decimal> exchangeRate)
	{
		ExchangeRate = exchangeRate;
		BtcBalance = btcBalance;
		UsdBalance = CalcHelper.UsdBalance(btcBalance, exchangeRate);
	}

	public IObservable<Money> BtcBalance { get; }
	public IObservable<decimal> UsdBalance { get; }
	public IObservable<decimal> ExchangeRate { get; }
}
