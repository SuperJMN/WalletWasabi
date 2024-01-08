using ReactiveUI;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.Validation;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.Wallets;

[NavigationMetaData(Title = "Rename Wallet", NavigationTarget = NavigationTarget.CompactDialogScreen)]
public partial class WalletRenameViewModel : RoutableViewModel
{
	[AutoNotify] private string _newWalletName;

	private WalletRenameViewModel(IWalletModel wallet)
	{
		_newWalletName = wallet.Name;

		this.ValidateProperty(
			x => x.NewWalletName,
			errors =>
			{
				if (wallet.Name == NewWalletName)
				{
					return;
				}

				if (UiContext.WalletRepository.ValidateWalletName(NewWalletName) is { } error)
				{
					errors.Add(error.Severity, error.Message);
				}
			});

		SetupCancel(enableCancel: true, enableCancelOnEscape: true, enableCancelOnPressed: true);
		var canRename = this.WhenAnyValue(model => model.NewWalletName, selector: _ => !Validations.Any);
		NextCommand = ReactiveCommand.Create(() => OnRename(wallet), canRename);
	}

	private void OnRename(IWalletModel wallet)
	{
		try
		{
			// TODO: Implement the rename operation here
			Navigate().Back();
		}
		catch
		{
			UiContext.Navigate().To().ShowErrorDialog($"The wallet cannot be renamed to {NewWalletName}", "Invalid name", "Cannot rename the wallet", NavigationTarget.CompactDialogScreen);
		}
	}
}