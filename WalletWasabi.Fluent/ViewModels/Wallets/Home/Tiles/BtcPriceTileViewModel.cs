using WalletWasabi.Bridge;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;

public class BtcPriceTileViewModel : ActivatableViewModel
{
	public BtcPriceTileViewModel(IExchangeRateProvider exchangeRateProvider)
	{
		UsdPerBtc = exchangeRateProvider.BtcToUsdRate;
	}

	public IObservable<decimal> UsdPerBtc { get; }
}
