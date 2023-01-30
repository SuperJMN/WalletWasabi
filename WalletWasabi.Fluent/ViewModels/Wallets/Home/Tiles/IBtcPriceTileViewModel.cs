namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;

public interface IBtcPriceTileViewModel
{
	IObservable<decimal> UsdPerBtc { get; }
}
