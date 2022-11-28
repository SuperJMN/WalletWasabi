using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NBitcoin;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Fluent.Helpers;

namespace WalletWasabi.Fluent.ViewModels.CoinControl.Core;

public abstract partial class CoinControlItemViewModelBase : ViewModelBase, ISelectableModel
{
	[AutoNotify] private bool? _isSelected;

	public CoinControlItemViewModelBase(IEnumerable<ISelectableModel> children)
	{
		Selectables = children;
		NestedSelection = new NestedSelection(this);
	}

	public bool IsPrivate => Labels == CoinPocketHelper.PrivateFundsText;

	public bool IsSemiPrivate => Labels == CoinPocketHelper.SemiPrivateFundsText;

	public bool IsNonPrivate => !IsSemiPrivate && !IsPrivate;

	public IEnumerable<CoinControlItemViewModelBase> Children => Selectables.Cast<CoinControlItemViewModelBase>();

	public bool IsConfirmed { get; protected set; }

	public bool IsCoinjoining { get; protected set; }

	public bool IsBanned { get; protected set; }

	public string ConfirmationStatus { get; protected set; } = "";

	public Money Amount { get; protected set; } = Money.Zero;

	public string? BannedUntilUtcToolTip { get; protected set; }

	public int AnonymityScore { get; protected set; }

	public SmartLabel Labels { get; protected set; } = SmartLabel.Empty;

	public DateTimeOffset? BannedUntilUtc { get; protected set; }

	public bool IsExpanded { get; set; } = true;

	public NestedSelection NestedSelection { get; }

	public IEnumerable<ISelectableModel> Selectables { get; }
}
