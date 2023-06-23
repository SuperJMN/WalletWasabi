using Avalonia;
using Avalonia.Controls.Primitives;

namespace WalletWasabi.Fluent.Controls;
public class DecimalEntry : TemplatedControl
{
	public static readonly StyledProperty<decimal?> ValueProperty = AvaloniaProperty.Register<DecimalEntry, decimal?>("Value");

	public decimal? Value
	{
		get => GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}
}
