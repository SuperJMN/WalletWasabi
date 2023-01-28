using System.Reactive;
using ReactiveUI;
using WalletWasabi.Blockchain.Analysis.Clustering;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

public interface IReceiveAddressViewModel
{
	bool[,] QrCode { get; }
	ReactiveCommand<Unit, bool[,]> GenerateQrCodeCommand { get; }
	string Address { get; }
	SmartLabel Labels { get; }
}
