using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using ReactiveUI;

namespace WalletWasabi.Fluent.Controls;

public class DualCurrencyBox2 : TemplatedControl
{
	private const decimal Tolerance = (decimal)0.00000100;

	public static readonly StyledProperty<decimal?> BtcBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(BtcBoxValue), defaultBindingMode: BindingMode.TwoWay, coerce: (o, newValue) =>
	{
		if (newValue == null)
		{
			return default;
		}

		return Math.Round(newValue.Value, 8);
	});

	public static readonly StyledProperty<decimal?> UsdBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(UsdBoxValue), defaultBindingMode: BindingMode.TwoWay);

	private decimal? _value;

	public static readonly DirectProperty<DualCurrencyBox2, decimal?> ValueProperty = AvaloniaProperty.RegisterDirect<DualCurrencyBox2, decimal?>(nameof(Value), o => o.Value, (o, v) => o.Value = SetCoercedValue(o.Value,v), enableDataValidation: true);

	private static decimal? SetCoercedValue(decimal? currentValue, decimal? newValue)
	{
		if (currentValue is null)
		{
			return newValue;
		}

		if (newValue is null)
		{
			return default;
		}

		var diff = (decimal)(currentValue - newValue);

		if (Math.Abs(diff) <= Tolerance)
		{
			return currentValue;
		}

		return newValue.Value;
	}

	public decimal? Value
	{
		get => _value;
		set => SetAndRaise(ValueProperty, ref _value, value);
	}

	public DualCurrencyBox2()
	{
		this
			.WhenAnyValue(x => x.BtcBoxValue)
			.Do(
				btcValue =>
				{
					Value = btcValue;
					UsdBoxValue = BtcToUsd(btcValue);
				})
			.Subscribe();

		this
			.WhenAnyValue(x => x.UsdBoxValue)
			.Do(x => Value = x / ExchangeRate)
			.Subscribe();

		this.WhenAnyValue(x => x.Value)
			.Do(
				btcValue =>
				{
					BtcBoxValue = btcValue;
					UsdBoxValue = BtcToUsd(btcValue);
				})
			.Subscribe();
	}

	private decimal? BtcToUsd(decimal? x)
	{
		if (x is null)
		{
			return default;
		}

		return Math.Round((decimal) (x * ExchangeRate), 2);
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

	public static readonly StyledProperty<decimal> ExchangeRateProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal>(nameof(ExchangeRate), new decimal(1));

	public decimal ExchangeRate
	{
		get => GetValue(ExchangeRateProperty);
		set => SetValue(ExchangeRateProperty, value);
	}

	protected override void UpdateDataValidation<T>(AvaloniaProperty<T> property, BindingValue<T> value)
	{
		DataValidationErrors.SetError(this, value.Error);
	}
}
