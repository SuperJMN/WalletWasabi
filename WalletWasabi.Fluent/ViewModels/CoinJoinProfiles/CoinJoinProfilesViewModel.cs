using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Fluent.ViewModels.AddWallet;
using WalletWasabi.Fluent.ViewModels.Dialogs;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.CoinJoinProfiles;

[NavigationMetaData(Title = "CoinJoin Profiles")]
public partial class CoinJoinProfilesViewModel : RoutableViewModel
{
	[AutoNotify] private CoinJoinProfileViewModelBase? _selectedProfile;

	public CoinJoinProfilesViewModel(KeyManager keyManager)
	{
		NextCommand = ReactiveCommand.Create(() => OnNext(keyManager));
		EnableBack = true;

		var privateProfile = new PrivateCoinJoinProfile();

		Profiles = new()
		{
			privateProfile,
			new SpeedyCoinJoinProfile(),
			new EconomyCoinJoinProfile()
		};

		_selectedProfile = privateProfile;

		ManualSetupCommand = ReactiveCommand.CreateFromTask(async () => await OnManualSetupAsync());
	}

	public ICommand ManualSetupCommand { get; }

	public List<CoinJoinProfileViewModelBase> Profiles { get; }

	public ManualCoinJoinProfile? SelectedManualProfile { get; private set; }

	private async Task OnManualSetupAsync()
	{
		var current = SelectedProfile ?? SelectedManualProfile ?? Profiles.First();
		var dialog = new ManualCoinJoinProfileDialogViewModel(current);

		var dialogResult = await NavigateDialogAsync(dialog, NavigationTarget.CompactDialogScreen);

		if (dialogResult.Result is ManualCoinJoinProfile profile)
		{
			SelectedProfile = null;
			SelectedManualProfile = profile;
		}
	}

	private void OnNext(KeyManager keyManager)
	{
		var selected = SelectedProfile ?? SelectedManualProfile ?? Profiles.First();

		keyManager.SetAnonScoreTargets(selected.MinAnonScoreTarget, selected.MaxAnonScoreTarget, toFile: false);
		keyManager.SetFeeRateAverageTimeFrame(selected.FeeRateAverageTimeFrameHours, toFile: false);

		Navigate().To(new AddedWalletPageViewModel(keyManager));
	}

	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
		base.OnNavigatedTo(isInHistory, disposables);

		var enableCancel = Services.WalletManager.HasWallet();
		SetupCancel(enableCancel: false, enableCancelOnEscape: enableCancel, enableCancelOnPressed: false);
	}
}
