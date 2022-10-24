using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Blockchain.TransactionOutputs;
using WalletWasabi.WebClients.PayJoin;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Send;

public partial class TransactionInfo
{
	[AutoNotify] private FeeRate _feeRate = FeeRate.Zero;
	[AutoNotify] private IEnumerable<SmartCoin> _coins = Enumerable.Empty<SmartCoin>();

	public TransactionInfo(BitcoinAddress destination, int anonScoreTarget)
	{
		Destination = destination;
		PrivateCoinThreshold = anonScoreTarget;

		this.WhenAnyValue(x => x.FeeRate)
			.Subscribe(_ => OnFeeChanged());

		this.WhenAnyValue(x => x.Coins)
			.Subscribe(_ => OnCoinsChanged());
	}

	public int PrivateCoinThreshold { get; }

	/// <summary>
	/// In the case when InsufficientBalanceException happens, this amount should be
	/// taken into account when selecting pockets.
	/// </summary>
	public Money MinimumRequiredAmount { get; set; } = Money.Zero;

	public Money Amount { get; init; } = Money.Zero;

	public BitcoinAddress Destination { get; init; }

	public SmartLabel Recipient { get; set; } = SmartLabel.Empty;

	public FeeRate? MaximumPossibleFeeRate { get; set; }

	public TimeSpan ConfirmationTimeSpan { get; set; }

	public IEnumerable<SmartCoin> ChangelessCoins { get; set; } = Enumerable.Empty<SmartCoin>();

	public IPayjoinClient? PayJoinClient { get; set; }

	public bool IsPayJoin => PayJoinClient is { };

	public bool IsOptimized => ChangelessCoins.Any();

	public bool IsCustomFeeUsed { get; set; }

	public bool SubtractFee { get; set; }

	public bool IsOtherPocketSelectionPossible { get; set; }

	public bool IsSelectedCoinModificationEnabled { get; set; } = true;

	public bool IsAutomaticSelectionEnabled { get; set; }

	public bool IsFixedAmount { get; init; }

	private void OnFeeChanged()
	{
		ChangelessCoins = Enumerable.Empty<SmartCoin>();
		MinimumRequiredAmount = Money.Zero;
	}

	private void OnCoinsChanged()
	{
		MaximumPossibleFeeRate = null;
	}

	public TransactionInfo Clone()
	{
		return new TransactionInfo(Destination, PrivateCoinThreshold)
		{
			FeeRate = FeeRate,
			Coins = Coins,
			MinimumRequiredAmount = MinimumRequiredAmount,
			Amount = Amount,
			Destination = Destination,
			Recipient = Recipient,
			MaximumPossibleFeeRate = MaximumPossibleFeeRate,
			ConfirmationTimeSpan = ConfirmationTimeSpan,
			ChangelessCoins = ChangelessCoins,
			PayJoinClient = PayJoinClient,
			IsCustomFeeUsed = IsCustomFeeUsed,
			SubtractFee = SubtractFee,
			IsOtherPocketSelectionPossible = IsOtherPocketSelectionPossible,
			IsSelectedCoinModificationEnabled = IsSelectedCoinModificationEnabled,
			IsAutomaticSelectionEnabled = IsAutomaticSelectionEnabled,
			IsFixedAmount = IsFixedAmount
		};
	}
}
