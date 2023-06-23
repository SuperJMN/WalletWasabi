using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using ReactiveUI;

namespace WalletWasabi.Fluent.Controls;

public class DualCurrencyBox2 : TemplatedControl
{
	private const decimal Tolerance = (decimal)0.00000001;

	public static readonly StyledProperty<decimal?> BtcBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(BtcBoxValue), defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<decimal?> UsdBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(UsdBoxValue), defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<decimal?> ValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(Value), defaultBindingMode: BindingMode.TwoWay,
		coerce: (obj, newValue) =>
		{
			var instance = (DualCurrencyBox2) obj;

			if (instance.Value is null)
			{
				return newValue;
			}

			if (newValue is null)
			{
				return default;
			}

			var dcbValue = (decimal)(instance.Value - newValue);

			if (Math.Abs(dcbValue) > Tolerance)
			{
				return newValue;
			}

			return instance.Value;
		});

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
