using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using ReactiveUI;
using WalletWasabi.Fluent.Behaviors;
using WalletWasabi.Fluent.ViewModels.CoinSelection.Core;

namespace WalletWasabi.Fluent.Controls;

public class SyncTreeNodeExpandStateBehavior : AttachedToVisualTreeBehavior<TreeDataGridExpanderCell>
{
	private TreeNode TreeNode { get; set; }

	protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
	{
		if (AssociatedObject?.DataContext is not TreeNode treeNode)
		{
			return;
		}

		TreeNode = treeNode;

		this.WhenAnyValue(x => x.TreeNode.IsExpanded)
			.BindTo(AssociatedObject, x => x.IsExpanded)
			.DisposeWith(disposable);
	}
}
