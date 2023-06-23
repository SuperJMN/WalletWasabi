using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using ReactiveUI;

namespace WalletWasabi.Fluent.Controls;

public class DualCurrencyBox2 : TemplatedControl
{
	public static readonly StyledProperty<decimal?> BtcBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(BtcBoxValue), defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<decimal?> UsdBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(UsdBoxValue), defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<decimal?> ValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

	public DualCurrencyBox2()
	{
		this
			.WhenAnyValue(x => x.BtcBoxValue)
			.Do(
				x =>
				{
					Value = x;
					UsdBoxValue = x * ExchangeRate;
				})
			.Subscribe();

		this
			.WhenAnyValue(x => x.UsdBoxValue)
			.Do(x => Value = x / ExchangeRate)
			.Subscribe();

		this.WhenAnyValue(x => x.Value)
			.Do(
				x =>
				{
					BtcBoxValue = x;
					UsdBoxValue = x * ExchangeRate;
				})
			.Subscribe();
	}

	public decimal? BtcBoxValue
	{
		get => GetValue(BtcBoxValueProperty);
		set => SetValue(BtcBoxValueProperty, value);
	}

	public decimal? UsdBoxValue
	{
		get => GetValue(UsdBoxValueProperty);
		set => SetValue(UsdBoxValueProperty, value);
	}

	public decimal? Value
	{
		get => GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}

	public static readonly StyledProperty<decimal> ExchangeRateProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal>(nameof(ExchangeRate), new decimal(1));

	public decimal ExchangeRate
	{
		get => GetValue(ExchangeRateProperty);
		set => SetValue(ExchangeRateProperty, value);
	}
}
