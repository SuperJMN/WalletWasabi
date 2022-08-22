using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;

namespace WalletWasabi.Fluent.TreeDataGrid;

public class ObservableTemplate<T, TOutput> : IDataTemplate
{
	private readonly Func<T, IObservable<TOutput>> _build;

	public ObservableTemplate(Func<T, IObservable<TOutput>> build)
	{
		_build = build;
	}

	public IControl Build(object param)
	{
		return new ContentPresenter()
		{
			[!ContentControl.ContentProperty] = _build((T)param).ToBinding(),
		};
	}

	public bool Match(object data)
	{
		return data is T;
	}
}
