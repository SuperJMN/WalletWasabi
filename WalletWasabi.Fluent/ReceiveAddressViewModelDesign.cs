using System.Reactive;
using ReactiveUI;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;

namespace WalletWasabi.Fluent;

public class ReceiveAddressViewModelDesign : IReceiveAddressViewModel
{
	public ReceiveAddressViewModelDesign()
	{
		var qrArray = RandomQrDesign.GetRandomQrCode();
		QrCode = qrArray;
		Address = "Some address";
		Labels = new SmartLabel("label1", "label2", "label3");
		GenerateQrCodeCommand = ReactiveCommand.Create(() => new bool[0, 0]);
	}

	public bool[,] QrCode { get; }
	public ReactiveCommand<Unit, bool[,]> GenerateQrCodeCommand { get; }
	public string Address { get; }
	public SmartLabel Labels { get; }
}
