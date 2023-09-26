using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Fluent.Extensions;
using WalletWasabi.Fluent.Helpers;
using WalletWasabi.Fluent.Models.UI;
using WalletWasabi.Fluent.ViewModels.Navigation;
using WalletWasabi.Models;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Home.History.Details;

[NavigationMetaData(Title = "Transaction Details")]
public partial class TransactionDetailsViewModel : RoutableViewModel
{
	private readonly WalletViewModel _walletVm;
	[AutoNotify] private string? _amountText = "";
	[AutoNotify] private string? _blockHash;
	[AutoNotify] private int _blockHeight;
	[AutoNotify] private int _confirmations;
	[AutoNotify] private TimeSpan? _confirmationTime;
	[AutoNotify] private string? _dateString;
	[AutoNotify] private bool _isConfirmationTimeVisible;
	[AutoNotify] private bool _isConfirmed;
	[AutoNotify] private bool _isLabelsVisible;
	[AutoNotify] private LabelsArray? _labels;
	[AutoNotify] private string? _transactionId;
	[AutoNotify] private Amount? _amount;

	public TransactionDetailsViewModel(UiContext uiContext, TransactionSummary transactionSummary, WalletViewModel walletVm)
	{
		UiContext = uiContext;
		_walletVm = walletVm;

		NextCommand = ReactiveCommand.Create(OnNext);

		Fee = uiContext.AmountProvider.Create(transactionSummary.GetFee());
		IsFeeVisible = Fee != null && transactionSummary.Amount < Money.Zero;
		DestinationAddresses = transactionSummary.Transaction.GetDestinationAddresses(walletVm.Wallet.Network).ToArray();

		SetupCancel(enableCancel: false, enableCancelOnEscape: true, enableCancelOnPressed: true);

		UpdateValues(transactionSummary);
	}

	public Amount Fee { get; }

	public ICollection<BitcoinAddress> DestinationAddresses { get; }

	public bool IsFeeVisible { get; }

	private void UpdateValues(TransactionSummary transactionSummary)
	{
		DateString = transactionSummary.FirstSeen.ToLocalTime().ToUserFacingString();
		TransactionId = transactionSummary.GetHash().ToString();
		Labels = transactionSummary.Labels;
		BlockHeight = transactionSummary.Height.Type == HeightType.Chain ? transactionSummary.Height.Value : 0;
		Confirmations = transactionSummary.GetConfirmations();

		TransactionFeeHelper.TryEstimateConfirmationTime(_walletVm.Wallet, transactionSummary.Transaction, out var estimate);
		ConfirmationTime = estimate;

		IsConfirmed = Confirmations > 0;

		if (transactionSummary.Amount < Money.Zero)
		{
			Amount = UiContext.AmountProvider.Create(-transactionSummary.Amount - (transactionSummary.GetFee() ?? Money.Zero));
			AmountText = "Outgoing";
		}
		else
		{
			Amount = UiContext.AmountProvider.Create(transactionSummary.Amount);
			AmountText = "Incoming";
		}

		BlockHash = transactionSummary.BlockHash?.ToString();

		IsConfirmationTimeVisible = ConfirmationTime.HasValue && ConfirmationTime != TimeSpan.Zero;
		IsLabelsVisible = Labels.HasValue && Labels.Value.Any();
	}

	private void OnNext()
	{
		Navigate().Clear();
	}

	protected override void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
		base.OnNavigatedTo(isInHistory, disposables);

		_walletVm.UiTriggers.TransactionsUpdateTrigger
			.DoAsync(async _ => await UpdateCurrentTransactionAsync())
			.Subscribe()
			.DisposeWith(disposables);
	}

	private async Task UpdateCurrentTransactionAsync()
	{
		var txRecordList = await Task.Run(() => TransactionHistoryBuilder.BuildHistorySummary(_walletVm.Wallet));

		var currentTransaction = txRecordList.FirstOrDefault(x => x.GetHash().ToString() == TransactionId);

		if (currentTransaction is { })
		{
			UpdateValues(currentTransaction);
		}
	}
}
