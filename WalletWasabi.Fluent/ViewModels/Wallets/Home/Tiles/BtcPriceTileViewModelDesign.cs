using ReactiveMarbles.PropertyChanged;
using ReactiveUI;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.Tiles;

public partial class BtcPriceTileViewModelDesign : ReactiveObject, IBtcPriceTileViewModel
{
	[AutoNotify] private decimal _usdPerBtcValue;

	public BtcPriceTileViewModelDesign()
	{
		UsdPerBtc = this.WhenChanged(x => x.UsdPerBtcValue);
	}

	public IObservable<decimal> UsdPerBtc { get; }
}
