using Avalonia.Data.Converters;

namespace WalletWasabi.Fluent;

public class MyFormatter
{
	public static FuncValueConverter<decimal?, string> Instance { get; } = new(d => d?.ToString("0.#### ####") ?? "");
}
