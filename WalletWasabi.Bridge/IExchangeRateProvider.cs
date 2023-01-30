namespace WalletWasabi.Bridge;

public interface IExchangeRateProvider
{
	IObservable<decimal> BtcToUsdRate { get; }
}
