using System.ComponentModel;
using ReactiveMarbles.PropertyChanged;
using WalletWasabi.Services;

namespace WalletWasabi.Bridge;

public class ExchangeRateProvider : IExchangeRateProvider, INotifyPropertyChanged
{
	public ExchangeRateProvider(WasabiSynchronizer synchronizer)
	{
		Synchronizer = synchronizer;
		BtcToUsdRate = this.WhenChanged(x => x.Synchronizer.UsdExchangeRate);
	}

	public WasabiSynchronizer Synchronizer { get; }

	public IObservable<decimal> BtcToUsdRate { get; }
	public event PropertyChangedEventHandler? PropertyChanged;
}
