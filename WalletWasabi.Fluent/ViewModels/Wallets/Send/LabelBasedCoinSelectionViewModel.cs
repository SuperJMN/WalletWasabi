using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using WalletWasabi.Fluent.Extensions;
using WalletWasabi.Fluent.ViewModels.Wallets.Advanced.WalletCoins;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Send;

public partial class LabelBasedCoinSelectionViewModel : ViewModelBase, IDisposable
{
	private readonly CompositeDisposable _disposables = new();
	private readonly ReadOnlyObservableCollection<CoinGroup> _items;
	[AutoNotify] private string _filter = "";

	public LabelBasedCoinSelectionViewModel(IObservable<IChangeSet<WalletCoinViewModel, int>> coinChanges)
	{
		var filterPredicate = this
			.WhenAnyValue(x => x.Filter)
			.Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
			.DistinctUntilChanged()
			.Select(SearchItemFilterFunc);

		coinChanges
			.Group(x => x.SmartLabel)
			.TransformWithInlineUpdate(group => new CoinGroup(group.Key, group.Cache))
			.Filter(filterPredicate)
			//.DisposeMany()	// Disposal behavior of Filter is unwanted
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out _items)
			.Subscribe()
			.DisposeWith(_disposables);
	}

	public ReadOnlyObservableCollection<CoinGroup> Items => _items;

	public void Dispose()
	{
		_disposables.Dispose();
	}

	private static Func<CoinGroup, bool> SearchItemFilterFunc(string? text)
	{
		return coinGroup =>
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return true;
			}

			var containsLabel = coinGroup.Labels.Any(s => s.Contains(text, StringComparison.InvariantCultureIgnoreCase));
			return containsLabel;
		};
	}
}
