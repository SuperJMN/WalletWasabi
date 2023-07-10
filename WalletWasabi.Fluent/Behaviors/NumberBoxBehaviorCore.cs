using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using WalletWasabi.Fluent.Extensions;
using WalletWasabi.Helpers;

namespace WalletWasabi.Fluent.Behaviors;

public class TextBoxNumberController : IDisposable
{
	private static readonly string Separator = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
	private readonly CompositeDisposable _disposables = new();
	private readonly int _maxDecimalPlaces;

	public TextBoxNumberController(TextBox textBox, int maxDecimalPlaces)
	{
		_maxDecimalPlaces = maxDecimalPlaces;
		textBox.OnEvent(InputElement.TextInputEvent, RoutingStrategies.Tunnel)
			.Select(x => x.EventArgs)
			.Do(x => InsertImplicitZero(x, textBox, x.Text ?? ""))
			.Do(x => Filter(x, textBox, x.Text))
			.Subscribe()
			.DisposeWith(_disposables);

		textBox.OnEvent(TextBox.PastingFromClipboardEvent)
			.SelectMany(async x => (x.EventArgs, Text: await GetClipboardTextAsync()))
			.Do(x => PrependZeroOnPaste(x.EventArgs, textBox, x.Text))
			.Do(pasting => Filter(pasting.EventArgs, textBox, pasting.Text))
			.Subscribe()
			.DisposeWith(_disposables);
	}

	public void Dispose()
	{
		_disposables.Dispose();
	}

	private static async Task<string> GetClipboardTextAsync()
	{
		if (Application.Current is { Clipboard: { } clipboard })
		{
			return (string?) await clipboard.GetTextAsync() ?? "";
		}

		return "";
	}

	private static void PrependZeroOnPaste(RoutedEventArgs pastingEventArgs, TextBox textBox, string toPaste)
	{
		if (textBox.SelectedText == textBox.Text && toPaste.StartsWith(Separator))
		{
			pastingEventArgs.Handled = true;
			textBox.Text = "0" + toPaste;
			textBox.ClearSelection();
			textBox.CaretIndex = textBox.Text.Length;
			return;
		}

		if (textBox.Text.Contains(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator) ||
		    !toPaste.StartsWith('.'))
		{
			return;
		}

		if (textBox.CaretIndex != 0)
		{
			return;
		}

		pastingEventArgs.Handled = true;
		textBox.Text = "0" + toPaste;
	}

	private static string SimulateNextText(string? text, TextBox tb)
	{
		var start = Math.Min(tb.SelectionStart, tb.SelectionEnd);
		var end = Math.Max(tb.SelectionStart, tb.SelectionEnd);

		return tb.Text[..start] + text + tb.Text[end..];
	}

	private static void InsertImplicitZero(TextInputEventArgs textInputEventArgs, TextBox textBox, string text)
	{
		if (textBox.Text is null)
		{
			return;
		}

		if (textBox.SelectedText == textBox.Text && text.StartsWith(Separator))
		{
			textInputEventArgs.Text = "0" + text;
			return;
		}

		if (textBox.Text.Contains(Separator) || !text.StartsWith(Separator))
		{
			return;
		}

		if (textBox.CaretIndex != 0)
		{
			return;
		}

		var finalText = "0" + text;
		if (!decimal.TryParse(finalText, out _))
		{
			return;
		}

		textInputEventArgs.Text = finalText;
	}

	private void Filter(RoutedEventArgs arg, TextBox tb, string? newText)
	{
		if (tb.Text is null)
		{
			return;
		}

		arg.Handled = !IsValid(SimulateNextText(newText, tb), tb.Text);
	}

	private bool IsValid(string str, string currentText)
	{
		if (currentText == "" && str == CultureInfo.CurrentUICulture.NumberFormat.NegativeSign)
		{
			return true;
		}

		if (str.Any(char.IsWhiteSpace))
		{
			return false;
		}

		var wasParsed = decimal.TryParse(str, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.CurrentUICulture, out var d);

		return wasParsed && d.CountDecimalPlaces() <= _maxDecimalPlaces;
	}
}
