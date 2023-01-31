using NBitcoin;
using WalletWasabi.Fluent.Extensions;

namespace WalletWasabi.Fluent.ViewModels.Wallets;

public class WalletBalances
{
	public WalletBalances(IObservable<Money> btcBalance, IObservable<decimal> exchangeRate)
	{
		ExchangeRate = exchangeRate;
		BtcBalance = btcBalance;
		UsdBalance = btcBalance.ToUsd(exchangeRate);
	}

	public IObservable<Money> BtcBalance { get; }
	public IObservable<decimal> UsdBalance { get; }
	public IObservable<decimal> ExchangeRate { get; }
}
