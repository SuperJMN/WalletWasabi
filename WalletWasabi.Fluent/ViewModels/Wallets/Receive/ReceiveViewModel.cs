using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Bridge;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;
using WalletWasabi.Wallets;
using IWallet = WalletWasabi.Bridge.IWallet;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

[NavigationMetaData(
	Title = "Receive",
	Caption = "Displays wallet receive dialog",
	IconName = "wallet_action_receive",
	Order = 6,
	Category = "Wallet",
	Keywords = new[] { "Wallet", "Receive", "Action", },
	NavBarPosition = NavBarPosition.None,
	NavigationTarget = NavigationTarget.DialogScreen)]
public partial class ReceiveViewModel : RoutableViewModel
{
	private readonly Wallet _wallet;
	private readonly IWallet _myWallet;
	[AutoNotify] private bool _isExistingAddressesButtonVisible;

	public ReceiveViewModel(Wallet wallet, IWallet myWallet)
	{
		_wallet = wallet;
		_myWallet = myWallet;
		SetupCancel(enableCancel: true, enableCancelOnEscape: true, enableCancelOnPressed: true);

		EnableBack = false;

		SuggestionLabels = new SuggestionLabelsViewModel(wallet.KeyManager, Intent.Receive, 3);

		var nextCommandCanExecute =
			SuggestionLabels
				.WhenAnyValue(x => x.Labels.Count).Select(_ => Unit.Default)
				.Merge(SuggestionLabels.WhenAnyValue(x => x.IsCurrentTextValid).Select(_ => Unit.Default))
				.Select(_ => SuggestionLabels.Labels.Count > 0 || SuggestionLabels.IsCurrentTextValid);

		NextCommand = ReactiveCommand.Create(OnNext, nextCommandCanExecute);

		ShowExistingAddressesCommand = ReactiveCommand.Create(OnShowExistingAddresses);
	}

	public SuggestionLabelsViewModel SuggestionLabels { get; }

	public ICommand ShowExistingAddressesCommand { get; }

	private void OnNext()
	{
		var newAddress = _myWallet.CreateReceiveAddress(SuggestionLabels.Labels);

		SuggestionLabels.Labels.Clear();

		var vm = ViewModelLocator.GetAddressViewModel(_myWallet, newAddress);

		Navigate().To(vm);
	}

	private void OnShowExistingAddresses()
	{
		Navigate().To(new ReceiveAddressesViewModel(_wallet, _myWallet));
	}

	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposable)
	{
		base.OnNavigatedTo(isInHistory, disposable);

		IsExistingAddressesButtonVisible = _wallet.KeyManager.GetKeys(x => !x.Label.IsEmpty && !x.IsInternal && x.KeyState == KeyState.Clean).Any();
	}
}
