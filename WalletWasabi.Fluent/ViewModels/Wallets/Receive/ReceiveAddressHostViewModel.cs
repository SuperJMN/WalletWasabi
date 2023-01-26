using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(Title = "Receive Address")]
public partial class ReceiveAddressHostViewModel : RoutableViewModel
{
	public ReceiveAddressViewModel ReceiveAddress { get; }
	public HardwareWalletViewModel HardwareWallet { get; }

	public ReceiveAddressHostViewModel(ReceiveAddressViewModel receiveAddress, Maybe<HardwareWalletViewModel> hardwareWallet)
	{
		ReceiveAddress = receiveAddress;
		HardwareWallet = hardwareWallet.GetValueOrDefault();
		EnableBack = true;
		var canExecute = hardwareWallet.Match(vm => vm.IsBusy.Select(b => !b), () => Observable.Return(true));
		CancelCommand = ReactiveCommand.Create(() => Navigate().Clear(), canExecute);
		NextCommand = CancelCommand;
		SetupCancel(enableCancel: false, enableCancelOnEscape: true, enableCancelOnPressed: true);
		hardwareWallet.Execute(vm => vm.IsBusy.BindTo(this, x => x.IsBusy));
	}
}
